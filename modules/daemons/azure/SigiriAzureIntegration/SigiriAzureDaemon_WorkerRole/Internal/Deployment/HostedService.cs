using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace SigiriAzureDaemon_WorkerRole.Internal.Deployment
{
    internal class HostedService
    {
        private readonly string _subscriptionId;
        private readonly string _name;
        private readonly string _label;
        private readonly X509Certificate2 _mgtCert;
        private readonly CloudBlobClient _blobClient;
        private const string Location = "Anywhere US";

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

        public HostedService(string subscriptionId, string name, string label, X509Certificate2 mgtCert)
        {
            _subscriptionId = subscriptionId;
            _name = name;
            _label = label;
            _mgtCert = mgtCert;
            _blobClient =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString")).
                    CreateCloudBlobClient();
        }

        public void Create()
        {
            var requestUri = "https://management.core.windows.net/" + _subscriptionId + "/services/hostedservices";
            var webRequest = CreateWebRequest(requestUri, Encoding.UTF8.GetBytes(GetCreateHostedServiceInputXML()));

            Trace.TraceInformation("Creating Hosted Service: " + _name);

            try
            {
                var state = new RequestState()
                                {
                                    Request = webRequest
                                };
                webRequest.BeginGetResponse(RespCallback, state);
            }catch(Exception e)
            {
                Trace.TraceError("Error occurred during hosted service creation:\n" + e.Message);
            }
        }

        public void Delete()
        {
        }

        public void CreateDeployment()
        {
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
            requestXML.AppendFormat("<ServiceName>{0}</ServiceName>", _name);
            requestXML.AppendFormat("<Label>{0}</Label>", EncodeToBase64String(_label));
            requestXML.AppendFormat("<Location>{0}</Location>", Location);
            requestXML.Append("</CreateHostedService>");

            return requestXML.ToString();
        }

        private string GetCreateDeploymentInputXML(string deploymentName, string workerRolePackageBlobUrl,
                                                   string workerRoleConfigBlobUrl)
        {
            var requestXML = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            requestXML.Append("<CreateDeployment xmlns=\"http://schemas.microsoft.com/windowsazure\">");
            requestXML.AppendFormat("<Name>{0}</Name>", deploymentName);
            requestXML.AppendFormat("<PackageUrl>{0}</PackageUrl>", workerRolePackageBlobUrl);
            requestXML.AppendFormat("<Label>{0}</Label>", EncodeToBase64String("testdeploymentlabel"));
            requestXML.AppendFormat("<Configuration>{0}</Configuration>",
                                    EncodeToBase64String(GetWorkerRoleConfigurationFileAsString()));
            requestXML.Append("<StartDeployment>true</StartDeployment>");
            requestXML.Append("</CreateDeployment>");
            return requestXML.ToString();
        }

        private static string GetWorkerRoleConfigurationFileAsString()
        {
            // TODO: Implement liquid template base configuration file generation
            return "";
        }


        private HttpWebRequest CreateWebRequest(string uri, byte[] formData)
        {
            var webRequest =
                (HttpWebRequest) WebRequest.Create(
                    new Uri(uri));

            webRequest.Method = Constants.WebMethodPost;
            webRequest.ClientCertificates.Add(_mgtCert);
            webRequest.ContentType = Constants.ContentTypeApplicationXML;
            webRequest.ContentLength = formData.Length;
            webRequest.Headers.Add(Constants.VersionHeaderName, Constants.VersionHeaderContent20101028);

            using (var post = webRequest.GetRequestStream())
            {
                post.Write(formData, 0, formData.Length);
            }

            return webRequest;
        }

        private static void RespCallback(IAsyncResult result)
        {
            var state = (RequestState) result.AsyncState; // Grab the custom state object
            var request = (WebRequest) state.Request;

            var response =
                (HttpWebResponse) request.EndGetResponse(result); // Get the Response

            var statusCode = response.StatusCode.ToString();

            // A value that uniquely identifies a request made against the Management service. 
            // For an asynchronous operation, 
            // you can call get operation status with the value of the header to determine whether 
            // the operation is complete, has failed, or is still in progress.
            var reqId = response.GetResponseHeader("x-ms-request-id");

            Trace.TraceInformation("Creation Return Value: " + statusCode);
            Trace.TraceInformation("RequestId: " + reqId);
        }

        public static string EncodeToBase64String(string original)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(original));
        }
    }

    public class RequestState
    {
        private const int BufferSize = 1024;
        public StringBuilder RequestData;
        public byte[] BufferRead;
        public WebRequest Request;
        public Stream ResponseStream;

        public Decoder StreamDecode = Encoding.UTF8.GetDecoder(); // Create Decoder for appropriate enconding type.

        public RequestState()
        {
            BufferRead = new byte[BufferSize];
            RequestData = new StringBuilder(String.Empty);
            Request = null;
            ResponseStream = null;
        }
    }
}