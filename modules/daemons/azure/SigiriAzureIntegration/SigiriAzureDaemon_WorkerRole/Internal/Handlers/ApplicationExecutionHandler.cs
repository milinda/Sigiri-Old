using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace SigiriAzureDaemon_WorkerRole.Internal.Handlers
{
    class ApplicationExecutionHandler:Handler
    {
        public const string HandlerName = "ApplicationExecutionHandler";

        private CloudQueueClient _queueClient;

        public override void Init(HandlerDescription handlerDescription, SigiriAzureDaemonConfiguration daemonConfiguration)
        {
            _queueClient = CloudStorageAccount.Parse(daemonConfiguration.DataConnectionString).CreateCloudQueueClient();
        }

        public override void Invoke(JobSubmissionContext azureDaemonContext)
        {
            var messageInQueue =
                _queueClient.GetQueueReference(String.Format("{0}inqueue", azureDaemonContext.ApplicationId.ToLower()));
            messageInQueue.CreateIfNotExist();
            messageInQueue.AddMessage(
                new CloudQueueMessage(String.Format("<SigiriAzureInMessage><AppId>{0}</AppId><JobId>{1}</JobId></SigiriAzureInMessage>", azureDaemonContext.ApplicationId, azureDaemonContext.JobId)));
        }

        public override string Name()
        {
            return HandlerName;
        }
    }
}
