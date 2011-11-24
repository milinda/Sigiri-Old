using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Ionic.Zip;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace Sigiri_WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private CloudQueueClient _queueClient;
        private CloudBlobClient _blobClient;

        private SigiriApplicationConfiguration _currentAppConfiguration;

        private AppRunner _appRunner;

        private CloudQueue _messageInQueue;
        private CloudQueue _messageOutQueue;

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("$projectname$ entry point called", "Information");

            while (true)
            {
                try
                {
                    /**
                     * Once Azure Daemon picks a job from a client, daemon will put a
                     * XML message in to the queue. Here we are picking that message and
                     * hand it over to AppRunner after pre-processing.
                     * 
                     * In Message Format(This could change):
                     *   <SigiriAzureInMessage>
                     *     <AppId>MatirxMul</AppId>
                     *     <JobId>urn:uuid:19939393949493</JobId>
                     *     ....[add new config elements based on requirements.]
                     *   </SigiriAzureInMessage>
                     */
                    var inMessage = _messageInQueue.GetMessage();
                    if(inMessage != null)
                    {  
                        if(_appRunner != null)
                        {
                            _appRunner.RunJob(SigiriAzureInMessage.CreateSigiriAzureInMessageFromXml(inMessage.AsString));

                            // Once we execute the job we need to remove the message from queue.
                            // TODO: Handle failure cases.
                            _messageInQueue.DeleteMessage(inMessage);
                        } else
                        {
                            var errorQ = _queueClient.GetQueueReference("error");
                            errorQ.CreateIfNotExist();
                            errorQ.AddMessage(new CloudQueueMessage("Erorrr"));
                            Trace.TraceError("Cannot find AppRunner instance. Please check worker role initialization logs for possible errors.");
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }

                }
                catch (StorageClientException e)
                {
                    Trace.TraceError("Exception when processing queue item. Message: '{0}'", e.Message);
                    Thread.Sleep(100);
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Setup the blob and queue clients.
            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));
            _blobClient = storageAccount.CreateCloudBlobClient();
            _queueClient = storageAccount.CreateCloudQueueClient();

            var localStorageResource = RoleEnvironment.GetLocalResource("ApplicationsStorage");
            var appId = RoleEnvironment.GetConfigurationSettingValue("ApplicationId");

            _messageInQueue = _queueClient.GetQueueReference(String.Format("{0}inqueue", appId.ToLower()));
            _messageInQueue.CreateIfNotExist();

            _messageOutQueue = _queueClient.GetQueueReference(String.Format("{0}outqueue", appId.ToLower()));
            _messageOutQueue.CreateIfNotExist();

            // Create application sotrage directory
            System.IO.Directory.CreateDirectory(String.Format(@"{0}\apptemp", localStorageResource.RootPath));

            // Load the content for the application application assigned for the worker role from cloud blob storage.
            // TODO: Handle application loading errors
            var appLocation = LoadApplication(appId, _blobClient, localStorageResource);
            if (appLocation != null)
            {
                _currentAppConfiguration =
                    SigiriApplicationConfiguration.CreateSigiriApplicationConfiguration(appId, appLocation,
                                                                                        String.Format(
                                                                                            @"{0}/application-config.xml",
                                                                                            appLocation));

                _appRunner = new AppRunner(_currentAppConfiguration, _blobClient, _messageOutQueue);
            }else
            {
                Trace.TraceInformation("Applicaton not found.");
            }

            return base.OnStart();
        }

        /**
         * Download application archive from the Cloud Blob Storage and copy it to Worker Role's 
         * local file system.
         */
        public static string LoadApplication(String appId, CloudBlobClient blobClient,
                                           LocalResource applicationStorageLocation)
        {
            var applicationContainer =
                blobClient.GetContainerReference(RoleEnvironment.GetConfigurationSettingValue("ApplicationStorage"));
            foreach (
                var appBlockBlob in
                    applicationContainer.ListBlobs().OfType<CloudBlockBlob>().Where(
                        appBlockBlob => appBlockBlob.Name.StartsWith(String.Format("app_{0}_", appId))))
            {
                // This should return only one item or no items. AppID should be unique to each app.
                appBlockBlob.FetchAttributes();
                var appArchiveSize = appBlockBlob.Properties.Length;

                var blobName = appBlockBlob.Name;
                var appArchiveName = blobName.Substring(5 + appId.Length);

                
                // Create a new local file to write into
                var fileStream =
                    new FileStream(String.Format(@"{0}\apptemp\{1}", applicationStorageLocation.RootPath, appArchiveName),
                                   FileMode.Create, FileAccess.Write);
                fileStream.SetLength(appArchiveSize);

                // A buffer to fill per read request.
                var buffer = new byte[4*1024*1024];

                // Write application archive to temp location in local storage.
                using (var blobStream = appBlockBlob.OpenRead())
                {
                    while (appArchiveSize > 0)
                    {
                        var chunkLength = blobStream.Read(buffer, 0, buffer.Length);
                        fileStream.Write(buffer, 0, chunkLength);
                        appArchiveSize -= chunkLength;
                    }
                }

                fileStream.Close();

                //Unpack application archive to local storage.
                var appUnpackDirectory = String.Format(@"{0}\apps\{1}", applicationStorageLocation.RootPath, appId);
                using (var appArchive = ZipFile.Read(String.Format(@"{0}\apptemp\{1}", applicationStorageLocation.RootPath, appArchiveName)))
                {
                    foreach (var zipEntry in appArchive)
                    {
                        zipEntry.Extract(appUnpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                    }       
                }

                Trace.TraceInformation("Application found with App ID:" + appId);
                return String.Format(@"{0}\apps\{1}", applicationStorageLocation.RootPath, appId);
            }

            Trace.TraceInformation("No application found with App ID: " + appId);

            return null;
        }
    }
}