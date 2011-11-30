using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    internal class JobNotificationManager : IWorker
    {
        private readonly CloudQueueClient _queueClient;
        private ApplicationStore _applicationStore;

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
                // TODO: Search for notifications in Storage Queue for all the currently available applications.
            }
        }
    }
}