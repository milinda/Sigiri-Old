using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    class JobSubmissionManager: IWorker
    {
        private JobStore _jobStore;

        private Dictionary<string, Handler> _handlers; 

        public JobSubmissionManager(JobStore jobStore)
        {
            _jobStore = jobStore;
            _handlers = new Dictionary<string, Handler>();
        }


        public void OnStart()
        {
            Trace.TraceInformation("Job Submission Manager Starting....");
        }

        public void OnStop()
        {
            Trace.TraceInformation("Job Submission Manager Shuting Down....");
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}
