using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace Sigiri_WorkerRole
{
    /**
     * Retrieve the application archive from the blog storage and place
     * it in the local hard disk of the worker role to use with job execution.
     */
    class ApplicationLoader
    {
        private readonly CloudBlobClient _blobClient;

        public ApplicationLoader(CloudBlobClient blobClient)
        {
            _blobClient = blobClient;
        }

        public void LoadApplication(String appId)
        {
            var applicationContainer =
                _blobClient.GetContainerReference(RoleEnvironment.GetConfigurationSettingValue("ApplicationStorage"));
            foreach (var appBlockBlob in applicationContainer.ListBlobs().OfType<CloudBlockBlob>().Where(appBlockBlob => appBlockBlob.Name.StartsWith(String.Format("app_{0}_", appId))))
            {
                // This should return only one item or no items. AppID should be unique to each app.

            }
        }
    }
}
