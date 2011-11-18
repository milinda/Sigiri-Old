using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using NLog;

namespace SigiriAzureDaemon
{
    internal class SigiriAzureDaemon
    {
        private readonly static Logger Log = LogManager.GetCurrentClassLogger();

        private  CloudBlobClient _blobStorageClient;
        private  CloudQueueClient _queueStorageClient;
        private ApplicationManager _applicationManager;

        static void Main(string[] args)
        {
            var sigiriAzureDaemon = new SigiriAzureDaemon();
            sigiriAzureDaemon.Initialize();
        }

        /**
         * Initialize the Azure Daemon for Sigiri.
         */
        public void Initialize()
        {
            InitializeStorageServices();
            InitializeApplicationManager();
        }

        /**
         * Initialize the application manager responsible for managing 
         * Sigiri application archives.
         */
        private void InitializeApplicationManager()
        {
            _applicationManager = new ApplicationManager(_blobStorageClient);
            _applicationManager.Initialize();
        }

        /**
         * Initialize the Azure Blob and Queue storage clients to use with
         * application management and job management.
         */
        private void InitializeStorageServices()
        {
            try
            {
                var azureStorageAccountConnectionString = ConfigurationManager.AppSettings["DataConnectionString"];
                var azureStorageAccount = CloudStorageAccount.Parse(azureStorageAccountConnectionString);

                _blobStorageClient = azureStorageAccount.CreateCloudBlobClient();
                _queueStorageClient = azureStorageAccount.CreateCloudQueueClient();
            }catch(WebException e)
            {
                Log.Error("Error while initializing Azure storage services.", e);
                throw new WebException("Storage services initialization failure. "
                        + "Check your storage account configuration settings. If running locally, "
                        + "ensure that the Development Storage service is running.", e);
            }
        }
    }
}
