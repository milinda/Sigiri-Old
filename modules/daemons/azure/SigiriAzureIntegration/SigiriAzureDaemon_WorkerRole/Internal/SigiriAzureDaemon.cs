using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    class SigiriAzureDaemon
    {
        private const string ComputingResourceName = "WindowsAzure";
        private const string JobManagerName = "AzureDaemon";

        private CloudBlobClient _blobStorageClient;
        private CloudQueueClient _queueStorageClient;
        private CloudTableClient _tableStorageClient;

        private ApplicationStore _applicationStore;
        private JobStore _jobStore;

        private IWorker _jobSubmissionManager;
        private IWorker _jobNotificationManager;


        private static void Main(string[] args)
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
            InitializeWorkers();
        }

        public void ShutDown()
        {
            if(_jobNotificationManager != null)
            {
                _jobNotificationManager.OnStop();
            }

            if(_jobSubmissionManager != null)
            {
                _jobSubmissionManager.OnStop();
            }
        }

        private void InitializeJobStore()
        {
            _jobStore = new JobStore(ComputingResourceName, JobManagerName);
        }

        private void InitializeWorkers()
        {
            _jobNotificationManager = new JobNotificationManager();
            _jobSubmissionManager = new JobSubmissionManager(_jobStore);

            _jobNotificationManager.OnStart();
            _jobSubmissionManager.OnStart();

            new Thread(_jobSubmissionManager.Run).Start();
            new Thread(_jobNotificationManager.Run).Start();
        }

        /**
         * Initialize the application manager responsible for managing 
         * Sigiri application archives.
         */

        private void InitializeApplicationManager()
        {
            _applicationStore = new ApplicationStore(_blobStorageClient);
            _applicationStore.Initialize();
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
            }
            catch (WebException e)
            {
                Trace.TraceError("Error while initializing Azure storage services.", e);
                throw new WebException("Storage services initialization failure. "
                                       + "Check your storage account configuration settings. If running locally, "
                                       + "ensure that the Development Storage service is running.", e);
            }
        }
    }
}
