using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    class JobNotificationManager:IWorker
    {
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
            throw new NotImplementedException();
        }
    }
}
