using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SigiriAzureDaemon_WorkerRole.Internal.Deployment;

namespace SigiriAzureDaemon_WorkerRole.Internal.Handlers
{
    class WorkerRoleSetupHandler:Handler
    {
        public const string HandlerName = "WorkerRoleSetupHandler";

        private WorkerRoleDeploymentManager _workerRoleDeploymentManager;

        public override void Init(HandlerDescription handlerDescription)
        {
            Trace.TraceInformation(String.Format("Inializing {0}...", HandlerName));

            _workerRoleDeploymentManager = new WorkerRoleDeploymentManager();
        }

        public override void Invoke(JobSubmissionContext azureDaemonContext)
        {
            /*
             * Worker Role Setup Algorithm
             *  - Get the application ID from context
             *  - Check for active workers for that applications
             *  - If there aren't any active workers go to worker role deployment
             *  - If there are active workers skip this method
             */
            var applicationId = azureDaemonContext.ApplicationId;

            if (!_workerRoleDeploymentManager.IsWorkerRoleActiveForApplication(applicationId))
            {
                
            }
            Trace.TraceInformation(String.Format("There are active worker roles for application {0}. Ignoring worker role setup step.", applicationId));
            return;
        }

        public override string Name()
        {
            return HandlerName;
        }
    }
}
