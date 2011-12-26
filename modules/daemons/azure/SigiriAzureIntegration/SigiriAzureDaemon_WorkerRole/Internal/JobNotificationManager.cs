using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.WindowsAzure.StorageClient;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    internal class JobNotificationManager : IWorker
    {
        private readonly CloudQueueClient _queueClient;
        private readonly ApplicationStore _applicationStore;

        public JobNotificationManager(CloudQueueClient queueClient, ApplicationStore applicationStore)
        {
            _applicationStore = applicationStore;
            _queueClient = queueClient;
        }

        public void OnStart()
        {
            Trace.TraceInformation("Job Notification Manager Starting....");
        }

        public void OnStop()
        {
            Trace.TraceInformation("Job Notification Manager Shuting Down....");
        }

        public void Run()
        {
            while (true)
            {
                var applications = _applicationStore.GetApplications();
                foreach (var outMessageQueue in applications.Select(application => _queueClient.GetQueueReference(String.Format("{0}outqueue", application.ToLower()))))
                {
                    outMessageQueue.CreateIfNotExist();
                    
                    // TODO: Make message count configurable
                    var outMessages = outMessageQueue.GetMessages(20);
                    foreach (var outMessage in outMessages.Select(cloudQueueMessage => SigiriAzureOutMessage.CreateSigiriAzureOutMessageFromXML(cloudQueueMessage.AsString)))
                    {
                        Trace.TraceInformation(String.Format("Job {0} of application {1} completed with status {2}.", outMessage.JobId, outMessage.ApplicationId, outMessage.Status));
                    }
                }

                // TODO: Make this configurable
                Thread.Sleep(400);
            }
        }
    }
}