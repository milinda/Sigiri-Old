using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    class SigiriAzureDaemonConfiguration
    {
        public string AzureSubscriptionId { set; get; }
        public X509Certificate2 AzureManagementCertificate { set; get; }
        public string WorkerRoleConfigurationTemplate { set; get; }
        public string WorkerRolePakcageBlobUrl { set; get; }
        public string DataConnectionString { set; get; }
    }
}
