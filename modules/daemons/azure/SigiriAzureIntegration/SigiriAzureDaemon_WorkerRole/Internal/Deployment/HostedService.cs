using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace SigiriAzureDaemon_WorkerRole.Internal.Deployment
{
    internal class HostedService
    {
        private readonly CloudBlobClient _blobClient;
        private const string Location = "Anywhere US";
        private readonly ManualResetEvent _allDone = new ManualResetEvent(false);

        public string SubscriptionId { set; get; }
        public string Name { set; get; }
        public string Label { set; get; }
        public string ApplicationId { set; get; }
        public X509Certificate2 MgtCert { set; get; }

        public enum DeploymentStatus
        {
            Running,
            Suspended,
            RunningTransitioning,
            SuspendedTransitioning,
            Starting,
            Suspending,
            Deploying,
            Deleting
        }

        public HostedService()
        {
            _blobClient =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString")).
                    CreateCloudBlobClient();
        }

        public string Create()
        {
            var requestUri = "https://management.core.windows.net/" + SubscriptionId + "/services/hostedservices";
            var webRequest = CreateWebRequest(requestUri, Encoding.UTF8.GetBytes(GetCreateHostedServiceInputXML()));

            Trace.TraceInformation("Creating Hosted Service: " + Name);

            try
            {
                var state = new RequestState()
                                {
                                    Request = webRequest
                                };
                webRequest.BeginGetResponse(RespCallback, state);

                // Waiting till the callback finishes it work.
                _allDone.WaitOne();

                // Closing the response
                if (state.Response != null)
                {
                    state.Response.Close();
                }

                if (state.StatusCode.Equals("Created"))
                {
                    return state.RequestId;
                }
                else
                {
                    Trace.TraceError("Failed to create hosted service. Status Code:" + state.StatusCode);
                    throw new Exception("Failed to create hosted service.");
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error occurred during hosted service creation:\n" + e.Message);
                throw;
            }
        }

        public void Delete()
        {
        }

        public string CreateDeployment()
        {
            var requestUrl = "https://management.core.windows.net/" + SubscriptionId + "/services/hostedservices/" +
                             Name + "/deploymentslots/staging";
            var workerRolePackagePath = RoleEnvironment.GetConfigurationSettingValue("Sigiri.WorkerRole.Package");
            var webRequest = CreateWebRequest(requestUrl, Encoding.UTF8.GetBytes(GetCreateDeploymentInputXML(String.Format("{0}deployment", ApplicationId.ToLower()), GetWorkerRolePackageUrl(_blobClient, workerRolePackagePath))));

            Trace.TraceInformation("Creating Worker Role Deployment..");
            try
            {
                var state = new RequestState() { Request = webRequest };
                webRequest.BeginGetResponse(RespCallback, state);
                _allDone.WaitOne();

                // Closing the response
                if (state.Response != null)
                {
                    state.Response.Close();
                }

                if (state.StatusCode.Equals("200"))
                {
                    return state.RequestId;
                }
                else
                {
                    Trace.TraceError(String.Format("Failed to create deployment {0}. Status Code:{1}", String.Format("{0}deployment", ApplicationId), state.StatusCode));
                    throw new Exception(String.Format("Failed to create deployment {0}.", String.Format("{0}deployment", ApplicationId)));
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error occurred during deployment creation:\n" + e.Message);
                throw;
            }
        }

        public DeploymentStatus GetDeploymentStatus()
        {
            return DeploymentStatus.Running;
        }

        public void SuspendDeployment()
        {
        }

        public void ResumeDeployment()
        {
        }

        public void DeleteDeployment()
        {
        }

        private string GetCreateHostedServiceInputXML()
        {
            var requestXML = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            requestXML.Append("<CreateHostedService xmlns=\"http://schemas.microsoft.com/windowsazure\">");
            requestXML.AppendFormat("<ServiceName>{0}</ServiceName>", Name);
            requestXML.AppendFormat("<Label>{0}</Label>", EncodeToBase64String(Label));
            requestXML.AppendFormat("<Location>{0}</Location>", Location);
            requestXML.Append("</CreateHostedService>");

            return requestXML.ToString();
        }

        private string GetCreateDeploymentInputXML(string deploymentName, string workerRolePackageBlobUrl)
        {
            var requestXML = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            requestXML.Append("<CreateDeployment xmlns=\"http://schemas.microsoft.com/windowsazure\">");
            requestXML.AppendFormat("<Name>{0}</Name>", deploymentName);
            requestXML.AppendFormat("<PackageUrl>{0}</PackageUrl>", workerRolePackageBlobUrl);
            requestXML.AppendFormat("<Label>{0}</Label>", EncodeToBase64String("testdeploymentlabel"));
            requestXML.AppendFormat("<Configuration>{0}</Configuration>",
                                    EncodeToBase64String(GetWorkerRoleConfigurationFileAsString(_blobClient,
                                                                                                ApplicationId,
                                                                                                RoleEnvironment.
                                                                                                    GetConfigurationSettingValue
                                                                                                    ("DataConnectionString"))));
            requestXML.Append("<StartDeployment>true</StartDeployment>");
            requestXML.Append("</CreateDeployment>");
            return requestXML.ToString();
        }

        private static string GetWorkerRolePackageUrl(CloudBlobClient blobClient, string packagePathConfigurationString)
        {
            var blobContainer = GetBlobContainerFromConfigurationString(packagePathConfigurationString);
            var blobName = GetBlobNameFromConfigurationString(packagePathConfigurationString);

            var blobContainerRef = blobClient.GetContainerReference(blobContainer);
            var blob = blobContainerRef.GetBlobReference(blobName);

            return blob.Uri.ToString();
        }
        private static string GetWorkerRoleConfigurationFileAsString(CloudBlobClient blobClient, string applicationId,
                                                                     string dataConnectionString)
        {
            var serivceConfigurationFilePath =
                RoleEnvironment.GetConfigurationSettingValue("Sigiri.WorkerRole.Configuration.Template");
            var blobContainer = GetBlobContainerFromConfigurationString(serivceConfigurationFilePath);
            var blobName = GetBlobNameFromConfigurationString(serivceConfigurationFilePath);

            var blobContainerRef = blobClient.GetContainerReference(blobContainer);
            var blob = blobContainerRef.GetBlobReference(blobName);

            var configurationFileContent = blob.DownloadText();
            var configurationFileWithCorrectConnectionString =
                configurationFileContent.Replace("{data-connection-string}", dataConnectionString);

            return configurationFileWithCorrectConnectionString.Replace("{application-id}", applicationId);
        }

        private static string GetBlobContainerFromConfigurationString(string configurationString)
        {
            var splits = configurationString.Split(';');
            foreach (var split in splits.Where(split => split.StartsWith("container")))
            {
                return split.Substring(10);
            }

            throw new Exception("Configuration string formatting error.");
        }

        private static string GetBlobNameFromConfigurationString(string configurationString)
        {
            var splits = configurationString.Split(';');
            foreach (var split in splits.Where(split => split.StartsWith("blob")))
            {
                return split.Substring(5);
            }

            throw new Exception("Configuration string formatting error.");
        }


        private HttpWebRequest CreateWebRequest(string uri, byte[] formData)
        {
            var webRequest =
                (HttpWebRequest) WebRequest.Create(
                    new Uri(uri));

            webRequest.Method = Constants.WebMethodPost;
            webRequest.ClientCertificates.Add(MgtCert);
            webRequest.ContentType = Constants.ContentTypeApplicationXML;
            webRequest.ContentLength = formData.Length;
            webRequest.Headers.Add(Constants.VersionHeaderName, Constants.VersionHeaderContent20101028);

            using (var post = webRequest.GetRequestStream())
            {
                post.Write(formData, 0, formData.Length);
            }

            return webRequest;
        }

        private void RespCallback(IAsyncResult result)
        {
            var state = (RequestState) result.AsyncState; // Grab the custom state object
            var request = (WebRequest) state.Request;

            state.Response =
                (HttpWebResponse) request.EndGetResponse(result); // Get the Response

            state.StatusCode = state.Response.StatusCode.ToString();

            // A value that uniquely identifies a request made against the Management service. 
            // For an asynchronous operation, 
            // you can call get operation status with the value of the header to determine whether 
            // the operation is complete, has failed, or is still in progress.
            state.RequestId = state.Response.GetResponseHeader("x-ms-request-id");
            _allDone.Set();
        }

        public static string EncodeToBase64String(string original)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(original));
        }
    }

    public class RequestState
    {
        public WebRequest Request;
        public HttpWebResponse Response;
        public string RequestId;
        public string StatusCode;

        public Decoder StreamDecode = Encoding.UTF8.GetDecoder(); // Create Decoder for appropriate enconding type.

        public RequestState()
        {
            Request = null;
            Response = null;
            RequestId = "";
            StatusCode = "";
        }
    }
}