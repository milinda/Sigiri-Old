using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    abstract class Handler
    {
        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();

        private HandlerDescription _handlerDescription;
 
        public abstract void Init(HandlerDescription handlerDescription);

        public abstract void Invoke(SigiriAzureDaemonContext azureDaemonContext);

        public string GetParameter(string name)
        {
            return _parameters[name];
        }

        public abstract string Name();

    }
}
