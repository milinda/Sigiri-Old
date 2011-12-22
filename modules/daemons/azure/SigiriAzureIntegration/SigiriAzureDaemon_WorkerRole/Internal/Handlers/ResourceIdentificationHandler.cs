using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal.Handlers
{
    class ResourceIdentificationHandler:Handler
    {
        public const string HandlerName = "ResourceIdentificationHandler";

        public override void Init(HandlerDescription handlerDescription, SigiriAzureDaemonConfiguration daemonConfiguration)
        {
            throw new NotImplementedException();
        }

        public override void Invoke(JobSubmissionContext azureDaemonContext)
        {
            // TODO: Improve to detect resource type based on job information.
            // TODO: Current implementation only supports worker roles.
            azureDaemonContext.AddParameter("ResourceType", "WorkerRole");
        }

        public override string Name()
        {
            return HandlerName;
        }
    }
}
