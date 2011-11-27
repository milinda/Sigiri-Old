using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Ionic.Zip;
using Microsoft.WindowsAzure.StorageClient;

namespace SigiriWorkerRoleTest
{
    internal class Utils
    {
        /**
         * Select application archive from the store and upload it to Azure Blob Storage.
         * @param name="applicationArchivePath" Application archive location
         */

        public static void SetupApplication(CloudBlobClient blobClient, string applicationArchivePath)
        {
            var applicationStorage =
                blobClient.GetContainerReference("sigiriapplications");
            applicationStorage.CreateIfNotExist();

            var appArchiveBlob = applicationStorage.GetBlockBlobReference(String.Format("app_{0}_{1}",
                                                                   GetApplicationIdFromApplicationArchive(
                                                                       applicationArchivePath),
                                                                   GetApplicationArchiveName(applicationArchivePath)));
            appArchiveBlob.UploadFromStream(new FileStream(applicationArchivePath, FileMode.Open));
            
            appArchiveBlob.Properties.ContentType = "application/zip";
            appArchiveBlob.SetProperties();
        }

        public static void ListApplications(CloudBlobClient blobClient)
        {
            var appStorage = blobClient.GetContainerReference("sigiriapplications");
            Console.WriteLine(appStorage.ListBlobs().Count());
        }

        public static void PutMessageToApplicationInQueue(CloudQueueClient queueClient, string applicationId, string message)
        {
            var inQueue = queueClient.GetQueueReference(String.Format("{0}inqueue", applicationId.ToLower()));
            inQueue.CreateIfNotExist();

            inQueue.AddMessage(new CloudQueueMessage(message));
        }

        public static void ReadJobCompletionNotificationsFromOutQueue(CloudQueueClient queueClient, string applicationId)
        {
            var outQueue = queueClient.GetQueueReference(String.Format("{0}outqueue", applicationId.ToLower()));
            outQueue.CreateIfNotExist();
            var message = outQueue.GetMessage();
            if(message != null)
            {
                Console.WriteLine(message.AsString);
            }

            // Should delete message from queue after reading. Otherwise it will reside in the queue infinitely.
            outQueue.DeleteMessage(message);
        }

        private static string GetApplicationArchiveName(String applicationArchivePath)
        {
            var appArchiveInfo = new FileInfo(applicationArchivePath);
            return appArchiveInfo.Name;
        }

        private static string GetApplicationIdFromApplicationArchive(string applicationArchivePath)
        {
            using (var appArchive = ZipFile.Read(applicationArchivePath))
            {
                foreach (
                    var zipEntry in appArchive.Where(zipEntry => zipEntry.FileName.Equals("application-config.xml")))
                {
                    var appConfigDoc = new XmlDocument();
                    var ms = new MemoryStream();

                    zipEntry.Extract(@"c:\tmp", ExtractExistingFileAction.OverwriteSilently);
                    appConfigDoc.Load(@"c:\tmp\application-config.xml");

                    return appConfigDoc.GetElementsByTagName("Id")[0].InnerText;
                }
            }

            return "";
        }
    }
}