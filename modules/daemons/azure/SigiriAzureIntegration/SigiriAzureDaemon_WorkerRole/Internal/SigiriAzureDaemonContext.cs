using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    class SigiriAzureDaemonContext
    {
        public string ApplicationId { set; get; }
        public string JobId { set; get; }

        public enum JobType
        {
            FileMovement,
            ApplicationExecution
        }

        public JobType Type { set; get; }

        private readonly Dictionary<string, Object> _parameters;

        public SigiriAzureDaemonContext()
        {
            _parameters = new Dictionary<string, object>();
        }

        public void AddParameter(string name, Object value)
        {
            _parameters.Add(name, value);
        }

        public Object GetParameterValue(string name)
        {
            return _parameters[name];
        }
    }
}
