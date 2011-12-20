using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;

namespace SigiriAzureDaemon_WorkerRole.Internal.Deployment
{
    class HostedService
    {
        private readonly string _name;
        private readonly string _label;
        private readonly X509Certificate2 _mgtCert;
        private readonly CloudBlobClient _blobClient;

        public HostedService(string name, string label, X509Certificate2 mgtCert)
        {
            _name = name;
            _label = label;
            _mgtCert = mgtCert;
        }
    }
}
