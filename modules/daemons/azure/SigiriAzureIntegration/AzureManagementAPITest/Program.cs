using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AzureManagementAPITest
{
    class Program
    {
        static void Main(string[] args)
        {
            var cert = new X509Certificate2(ConfigurationManager.AppSettings["CertificateFile"]);
            var hostedService = new HostedService("95ce769c-6cfb-48d9-9546-62c219cb0ee7", "TestCreation",
                                                  "TestCreationName", cert);
            hostedService.CreateHostedService();
            hostedService.CreateDeployment();
        }
    }
}
