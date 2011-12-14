using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;

namespace AzureManagementAPITest
{
    class Constants
    {
        public const string ServiceManagementNS = "http://schemas.microsoft.com/windowsazure";
        public const string OperationTrackingIdHeader = "x-ms-request-id";
        public const string VersionHeaderName = "x-ms-version";
        public const string VersionHeaderContent = "2009-10-01";
        public const string VersionHeaderContent20100401 = "2010-04-01";
        public const string VersionHeaderContent20101028 = "2010-10-28";
        public const string PrincipalHeader = "x-ms-principal-id";
    }

    class HostedService
    {
        public void CreateHostedService()
        {
            using(var cf = new WebChannelFactory<IServiceManagement>("WindowsAzureEndPoint"))
            {
                cf.Endpoint.Behaviors.Add(new ClientOutputMessageInspector());
                cf.Credentials.ClientCertificate.Certificate =
                    new X509Certificate2(ConfigurationManager.AppSettings["CertificateFile"]);
                IServiceManagement channel = cf.CreateChannel();

                CreateHostedServiceInput input = new CreateHostedServiceInput()
                {
                    ServiceName = HostedServiceName,
                    Label = EncodeToBase64String(CSManageCommand.Label),
                    Description = CSManageCommand.Description
                };

            }
        }

        public static string EncodeToBase64String(string original)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(original));
        }
    }

    [ServiceContract]
    public interface IServiceManagement
    {

        #region CreateHostedService
        /// <summary>
        /// Creates a hosted service
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/services/hostedservices")]
        IAsyncResult BeginCreateHostedService(string subscriptionId, CreateHostedServiceInput input, AsyncCallback callback, object state);
        void EndCreateHostedService(IAsyncResult asyncResult);
        #endregion
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

    public class ClientOutputMessageInspector : IClientMessageInspector, IEndpointBehavior
    {
        #region IClientMessageInspector Members

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState) { }
        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            var property = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            if (property.Headers[Constants.VersionHeaderName] == null)
            {
                property.Headers.Add(Constants.VersionHeaderName, Constants.VersionHeaderContent20101028);
            }
            return null;
        }

        #endregion

        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

        public void Validate(ServiceEndpoint endpoint) { }

        #endregion

    }
}
