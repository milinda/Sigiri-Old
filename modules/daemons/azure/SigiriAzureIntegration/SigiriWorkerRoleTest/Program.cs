using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace SigiriWorkerRoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var appId = "MatrixMul";
            var inMessage =
                "<SigiriAzureInMessage><AppId>MatirxMul</AppId><JobId>urn:uuid:19939393949493</JobId></SigiriAzureInMessage>";
            var appArchivePath =
                @"D:\Milinda\Workspace\sigiri-new\modules\samples\sigiri-apps\MatrixMultiplier\MatrixMultiplier.zip";
            //CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
 //configSetter(ConfigurationManager.AppSettings[configName]));
           
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            var blobClient = storageAccount.CreateCloudBlobClient();
            var queueClient = storageAccount.CreateCloudQueueClient();

            Utils.SetupApplication(blobClient, appArchivePath);
            Utils.PutMessageToApplicationInQueue(queueClient, appId, inMessage);
            Utils.ListApplications(blobClient);
            while(true)
            {
                Utils.ReadJobCompletionNotificationsFromOutQueue(queueClient, appId);
                Thread.Sleep(5000);
            }


        }

        
    }
}
