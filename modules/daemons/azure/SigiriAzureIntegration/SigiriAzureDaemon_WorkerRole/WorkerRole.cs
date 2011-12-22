using System.Diagnostics;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;
using SigiriAzureDaemon_WorkerRole.Internal;

namespace SigiriAzureDaemon_WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private SigiriAzureDaemon _azureDaemon;

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("$projectname$ entry point called", "Information");

            while (true)
            {
                Thread.Sleep(10000);
                Trace.WriteLine("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            var daemonConfiguration = new SigiriAzureDaemonConfiguration()
                                          {
                                              AzureSubscriptionId =
                                                  RoleEnvironment.GetConfigurationSettingValue("AzureSubscriptionId"),
                                              WorkerRoleConfigurationTemplate =
                                                  RoleEnvironment.GetConfigurationSettingValue(
                                                      "Sigiri.WorkerRole.Configuration.Template"),
                                              WorkerRolePakcageBlobUrl =
                                                  RoleEnvironment.GetConfigurationSettingValue(
                                                      "Sigiri.WorkerRole.Package"),
                                              DataConnectionString =
                                                  RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"),
                                              AzureManagementCertificate =
                                                  new X509Certificate2(
                                                  DownloadManagementCertificateFromBlob(
                                                      RoleEnvironment.GetConfigurationSettingValue(
                                                          "AzureManagementCertificateLocation")))
                                          };
            _azureDaemon = new SigiriAzureDaemon(daemonConfiguration);
            return base.OnStart();
        }

        private byte[] DownloadManagementCertificateFromBlob(string managementCertificateLocation)
        {
            return new byte[10];
        }
    }
}
