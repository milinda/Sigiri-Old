using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal.Handlers
{
    class WorkerRoleSetupHandler:Handler
    {
        public const string HandlerName = "WorkerRoleSetupHandler";


        public override void Init(HandlerDescription handlerDescription)
        {
            throw new NotImplementedException();
        }

        public override void Invoke(SigiriAzureDaemonContext azureDaemonContext)
        {
            /*
             * Worker Role Setup Algorithm
             *  - Get the application ID from context
             *  - Check for active workers for that applications
             *  - If there aren't any active workers go to worker role deployment
             *  - If there are active workers skip this method
             */
            var applicationId = azureDaemonContext.ApplicationId;
            throw new NotImplementedException();
        }

        public override string Name()
        {
            return HandlerName;
        }
    }
}
