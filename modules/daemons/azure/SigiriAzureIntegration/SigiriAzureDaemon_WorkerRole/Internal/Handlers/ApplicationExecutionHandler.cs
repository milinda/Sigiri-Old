using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal.Handlers
{
    class ApplicationExecutionHandler:Handler
    {
        public const string HandlerName = "ApplicationExecutionHandler";
        public override void Init(HandlerDescription handlerDescription)
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
