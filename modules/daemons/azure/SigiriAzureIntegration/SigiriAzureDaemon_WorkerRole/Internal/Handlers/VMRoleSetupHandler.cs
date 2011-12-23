using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal.Handlers
{
    class VMRoleSetupHandler:Handler
    {
        public const string HandlerName = "VMRoleSetupHandler";
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
            return HandlerName;
        }
    }
}
