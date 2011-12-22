using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal.Deployment
{
    /// <summary>
    /// Manage the deployment of worker roles for applications.
    /// </summary>
    class WorkerRoleDeploymentManager
    {
        private readonly Dictionary<string, bool> _activeWorkerRoles = new Dictionary<string, bool>();
        
        // Application ID to HostedService map. 
        private readonly Dictionary<string, HostedService> _hostedServices = new Dictionary<string, HostedService>(); 

        private readonly SigiriAzureDaemonConfiguration _daemonConfiguration;

        public WorkerRoleDeploymentManager(SigiriAzureDaemonConfiguration daemonConfiguration)
        {
            _daemonConfiguration = daemonConfiguration;
        }

        /// <summary>
        /// Sync the status of azure account to status of deployment manager. If there are active worker 
        /// roles, we will create hosted service instances and populate it with necessary details.
        /// </summary>
        public void Initialize()
        {
            
        }

        public bool IsWorkerRoleActiveForApplication(string applicationId)
        {
            return _activeWorkerRoles[applicationId];
        }

        public void SetupWorkerRoleForApplication(string applicationId)
        {
            // If the following conditions satisfies, that means there are active worker roles for 
            // this application.
            if (_hostedServices[applicationId] != null && _hostedServices[applicationId].Active) return;

            var hostedService = new HostedService()
                                    {
                                        SubscriptionId = _daemonConfiguration.AzureSubscriptionId,
                                        Name = String.Format("hostedservicefor{0}", applicationId.ToLower()),
                                        Label = String.Format("hostedservicefor{0}", applicationId.ToLower()),
                                        ApplicationId = applicationId,
                                        MgtCert = _daemonConfiguration.AzureManagementCertificate,
                                        DataConnectionString = _daemonConfiguration.DataConnectionString
                                    };
            // We don't need to check the status of create operation, because we can know whether the
            // operation is successful or not from the first response.
            // TODO: But we need to handle the exeception correctly and report the error back to user.
            hostedService.Create();

            // Now we have created hosted service for the application. It's time to deploy worker role
            // for the execution of jobs. In this case we need to monitor the operation status. We can 
            // ignore the status check step and go ahead, but we don't have a way to check whether the 
            // worker role initialization successfull. If worker role initialization failed, there will 
            // be messages lying in in-queue for that worker role. We need implement proper error handling 
            // mechanism for those situations.
            // TODO: For the moment I am ignoring status check. In this case worker role will 
            // TODO: pick the input message whenever it becomes ready.
            hostedService.CreateDeployment();
        }

        public void UnDeployWorkerRoleForApplication(string applicationId)
        {
            
        }
    }
}
