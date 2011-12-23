using System;
using System.Diagnostics;

namespace SigiriAzureDaemon_WorkerRole.Internal.Handlers
{
    class CredentialManagementHandler:Handler
    {
        public const string HandlerName = "CredentialManagementHandler";
        public override void Init(HandlerDescription handlerDescription, SigiriAzureDaemonConfiguration daemonConfiguration)
        {
            throw new NotImplementedException();
        }

        public override void Invoke(JobSubmissionContext azureDaemonContext)
        {
            // TODO: Currently not required
            Trace.TraceInformation(String.Format("CredentialManagementHandler invoked for Job {0} of App {1}", azureDaemonContext.JobId, azureDaemonContext.ApplicationId));
        }

        public override string Name()
        {
            return HandlerName;
        }
    }
}
