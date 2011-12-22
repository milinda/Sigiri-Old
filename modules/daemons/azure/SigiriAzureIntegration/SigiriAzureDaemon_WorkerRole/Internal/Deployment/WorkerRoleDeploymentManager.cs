using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal.Deployment
{
    class WorkerRoleDeploymentManager
    {
        private readonly Dictionary<string, bool> _activeWorkerRoles = new Dictionary<string, bool>();

        public bool IsWorkerRoleActiveForApplication(string applicationId)
        {
            return _activeWorkerRoles[applicationId];
        }

        public void SetupWorkerRoleForApplication(string applicationId)
        {
            
        }

        public void UnDeployWorkerRoleForApplication(string applicationId)
        {
            
        }
    }
}
