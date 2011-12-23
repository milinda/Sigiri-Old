using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal.Handlers
{
    class InputDataMovementHandler:Handler
    {
        public const string HandlerName = "InputDataMovementHandler";
        public override void Init(HandlerDescription handlerDescription, SigiriAzureDaemonConfiguration daemonConfiguration)
        {
            throw new NotImplementedException();
        }

        public override void Invoke(JobSubmissionContext azureDaemonContext)
        {
            // TODO: Need to implement support or input data movement. 
            Trace.TraceInformation(String.Format("InputDataMovementHandler Invoked for Application {0} Job {1}.", azureDaemonContext.ApplicationId, azureDaemonContext.JobId));
        }

        public override string Name()
        {
            return HandlerName;
        }
    }
}
