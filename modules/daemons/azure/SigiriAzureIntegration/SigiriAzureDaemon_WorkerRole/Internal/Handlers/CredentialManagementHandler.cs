using System;

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
            throw new NotImplementedException();
        }

        public override string Name()
        {
            throw new NotImplementedException();
        }
    }
}
