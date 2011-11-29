using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using Ionic.Zip;
using Microsoft.WindowsAzure.StorageClient;
using System.Xml;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    /**
     * Responsible for managing applications registered by users to run as jobs
     * in Azure Compute. Users can register applications via Sigiri Web service.
     * This will store applications registered for Azure Compute in Azure Blob 
     * Storage. These applications can then be used by SigiriWorkerRole to execute
     * the jobs submitted by user.
     */
    class ApplicationStore
    {
        private readonly CloudBlobClient _blobClient;

        private Dictionary<string, SigiriApplicationConfiguration> _applications;  
        
        public ApplicationStore(CloudBlobClient blobClient)
        {
            _blobClient = blobClient;
            _applications = new Dictionary<string, SigiriApplicationConfiguration>();
        }

        public void Initialize()
        {
            try
            {
                var applicationContainer =
                    _blobClient.GetContainerReference(ConfigurationManager.AppSettings["ApplicationStorage"]);
                applicationContainer.CreateIfNotExist();

                LoadExistingApplications();
            }catch(WebException e)
            {
                throw new WebException("Error when accessing ApplicationStorage container from Azure Blob storage.", e);
            }
        }

        public bool Contains(string application)
        {
            return false;
        }

        // Load the existing applications already stored in Azure Blob Storage
        private void LoadExistingApplications()
        {
            
        }

        /**
         * Store application in blob storage.
         */
        public void StoreApplicationInBlobStorage()
        {
            /**
             * Current implementation doesn't get application archives from 
             * database. Currently just using a hard coded file path pointing
             * to application archive.
             */
            const string applicationArchivePath = @"D:\Milinda\Workspace\Azure\Design-Workflow\Executable\MatrixMultiplier.zip";
            string applicationId;

            using(var ms = new MemoryStream())
            {
                using(var applicationArchive = ZipFile.Read(applicationArchivePath))
                {
                    var executableConfigurations = applicationArchive["executable-config.xml"];
                    executableConfigurations.Extract(ms);
                }
                var executableConfigurationDoc = new XmlDocument();
                executableConfigurationDoc.Load(ms);
                var idElements = executableConfigurationDoc.GetElementsByTagName("Id");
                var idElement = idElements[0];
                applicationId = idElement.InnerText;
            }

            var appArchive = new FileInfo(applicationArchivePath);

            var applicatiobBlobName = String.Format("{0}/app_{1}_{2}",
                                                       ConfigurationManager.AppSettings["ApplicationStorage"],
                                                       applicationId, appArchive.Name);
            var applicationArchiveBlob = _blobClient.GetBlockBlobReference(applicatiobBlobName);
            applicationArchiveBlob.Properties.ContentType = "application/zip"; // We don't neet to find this. We know app archive is a zip.
            applicationArchiveBlob.UploadFromStream(File.Create(applicationArchivePath));
        }

        public void DeregisterApplication()
        {
            
        }
            
    }
}
