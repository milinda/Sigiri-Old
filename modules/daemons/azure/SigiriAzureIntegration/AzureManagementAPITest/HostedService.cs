using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace AzureManagementAPITest
{
    internal class Constants
    {
        public const string ServiceManagementNS = "http://schemas.microsoft.com/windowsazure";
        public const string OperationTrackingIdHeader = "x-ms-request-id";
        public const string VersionHeaderName = "x-ms-version";
        public const string VersionHeaderContent = "2009-10-01";
        public const string VersionHeaderContent20100401 = "2010-04-01";
        public const string VersionHeaderContent20101028 = "2010-10-28";
        public const string PrincipalHeader = "x-ms-principal-id";

        public const string WebMethodPost = "POST";
        public const string ContentTypeApplicationXML = "application/xml";
    }

    internal class HostedService
    {
        private readonly string _subscriptionId;
        private readonly string _name;
        private readonly string _label;
        private readonly X509Certificate2 _mgtCert;
        private readonly CloudBlobClient _blobClient;

        public HostedService(string subscriptionId, string name, string label, X509Certificate2 azureManagementCert)
        {
            _subscriptionId = subscriptionId;
            _name = name;
            _label = label;
            _mgtCert = azureManagementCert;
            _blobClient =
                CloudStorageAccount.Parse(ConfigurationManager.AppSettings["DataConnectionString"]).
                    CreateCloudBlobClient();
        }

        public void CreateHostedService()
        {
            var formData = Encoding.UTF8.GetBytes(GetCreateHostedServiceInputXML());
            var requestUri = "https://management.core.windows.net/" + _subscriptionId + "/services/hostedservices";
            var webRequest = CreateWebRequest(requestUri, formData);

            Trace.TraceInformation("Creating Hosted Service: " + _name);

            try
            {
                var state = new RequestState()
                                {
                                    Request = webRequest
                                };
                var response = webRequest.BeginGetResponse(RespCallback, state);
            }
            catch (Exception e)
            {
                Trace.TraceError("Error occurred during hosted service creation:\n" + e.Message);
            }
        }

        public void CreateDeployment()
        {
            var blobUri = UploadWorkerRolePackageToBlobStorage();
            var requestUrl = "https://management.core.windows.net/" + _subscriptionId + "/services/hostedservices/" +
                             _name + "/deploymentslots/staging";
            var webRequest = CreateWebRequest(requestUrl, Encoding.UTF8.GetBytes(GetCreateDeploymentInputXML(blobUri)));
            
            Trace.TraceInformation("Creating Worker Role Deployment..");
            try
            {
                var state = new RequestState() {Request = webRequest};
                var response = webRequest.BeginGetResponse(RespCallback, state);
            }
            catch (Exception e)
            {
                Trace.TraceError("Error occurred during deployment creation:\n" + e.Message);
            }

            Console.ReadKey();
        }


        private String UploadWorkerRolePackageToBlobStorage()
        {
            var blobContainer = _blobClient.GetContainerReference("workerrolepackages");
            blobContainer.CreateIfNotExist();

            var workerRolePackageBlob = blobContainer.GetBlobReference("simpleworker.cspkg");
            workerRolePackageBlob.UploadFile(ConfigurationManager.AppSettings["WorkerRolePkg"]);

            return workerRolePackageBlob.Uri.ToString();
        }

        private string GetCreateDeploymentInputXML(string blobUrl)
        {
            var sbRequestXML = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sbRequestXML.Append("<CreateDeployment xmlns=\"http://schemas.microsoft.com/windowsazure\">");
            sbRequestXML.AppendFormat("<Name>{0}</Name>", "testdeployment");
            sbRequestXML.AppendFormat("<PackageUrl>{0}</PackageUrl>", blobUrl);
            sbRequestXML.AppendFormat("<Label>{0}</Label>", EncodeToBase64String("testdeployment"));
            sbRequestXML.AppendFormat("<Configuration>{0}</Configuration>", EncodeToBase64String(GetWorkerRoleConfigurationFileAsString()));
            sbRequestXML.Append("<StartDeployment>true</StartDeployment>");
            sbRequestXML.Append("<TreatWarningsAsError>false</TreatWarningsAsError>");
            sbRequestXML.Append("</CreateDeployment>");
            return sbRequestXML.ToString();
        }

        private string GetWorkerRoleConfigurationFileAsString()
        {
            var fileReader = new StreamReader(ConfigurationManager.AppSettings["WorkerRoleConfig"]);
            var content = fileReader.ReadToEnd();
            fileReader.Close();

            return content;
        }

        private string GetCreateHostedServiceInputXML()
        {
            var sbRequestXML = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sbRequestXML.Append("<CreateHostedService xmlns=\"http://schemas.microsoft.com/windowsazure\">");
            sbRequestXML.AppendFormat("<ServiceName>{0}</ServiceName>", _name);
            sbRequestXML.AppendFormat("<Label>{0}</Label>", EncodeToBase64String(_label));
            sbRequestXML.Append("<Location>Anywhere US</Location>");
            sbRequestXML.Append("</CreateHostedService>");

            return sbRequestXML.ToString();
        }

        private HttpWebRequest CreateWebRequest(string uri, byte[] formData)
        {
            var webRequest =
                (HttpWebRequest) WebRequest.Create(
                    new Uri("https://management.core.windows.net/" + _subscriptionId + "/services/hostedservices"));

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


    /// <summary>
    /// CreateHostedService contract
    /// </summary>
    [DataContract(Name = "CreateHostedService", Namespace = Constants.ServiceManagementNS)]
    public class CreateHostedServiceInput : IExtensibleDataObject
    {
        [DataMember(Order = 1)]
        public string ServiceName { get; set; }

        [DataMember(Order = 2)]
        public string Label { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string Location { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false)]
        public string AffinityGroup { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
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