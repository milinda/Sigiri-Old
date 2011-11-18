using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Ionic.Zip;
using Microsoft.WindowsAzure.StorageClient;
using System.Xml;

namespace SigiriAzureDaemon
{
    /**
     * Responsible for managing applications registered by users to run as jobs
     * in Azure Compute. Users can register applications via Sigiri Web service.
     * This will store applications registered for Azure Compute in Azure Blob 
     * Storage. These applications can then be used by SigiriWorkerRole to execute
     * the jobs submitted by user.
     */
    class ApplicationManager
    {
        private readonly CloudBlobClient _blobClient;
        
        public ApplicationManager(CloudBlobClient blobClient)
        {
            _blobClient = blobClient;
        }

        public void Initialize()
        {
            try
            {
                var applicationContainer =
                    _blobClient.GetContainerReference(ConfigurationManager.AppSettings["ApplicationStorage"]);
                applicationContainer.CreateIfNotExist();
            }catch(WebException e)
            {
                throw new WebException("Error when accessing ApplicationStorage container from Azure Blob storage.", e);
            }
        }

        /**
         * Store application in blob storage.
         */
        public void RegisterApplication()
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
