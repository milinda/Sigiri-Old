using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;

namespace Sigiri_WorkerRole
{
    /**
     * Execute Sigiri applications on a incoming message. Responsible for creating Process object
     * and execute the application with required paramters.
     */

    internal class AppRunner
    {
        private readonly SigiriApplicationConfiguration _sigiriApplicationConfiguration;

        private readonly CloudBlobClient _cloudBlobClient;

        private readonly CloudQueue _outMessageQueue;

        public AppRunner(SigiriApplicationConfiguration sigiriApplicationConfiguration, CloudBlobClient cloudBlobClient,
                         CloudQueue outMessageQueue)
        {
            _sigiriApplicationConfiguration = sigiriApplicationConfiguration;
            _cloudBlobClient = cloudBlobClient;
            _outMessageQueue = outMessageQueue;
        }

        public void RunJob(SigiriAzureInMessage inMessage)
        {
            var processStartInfo = new ProcessStartInfo
                                       {
                                           WorkingDirectory = _sigiriApplicationConfiguration.WorkingDirectory,
                                           UseShellExecute = false,
                                           RedirectStandardOutput = true,
                                           RedirectStandardError = true,
                                           FileName = "CMD.exe",
                                           Arguments =
                                               String.Format("/C {0}",
                                                             _sigiriApplicationConfiguration.ExecutableName)
                                       };
            var process = new Process {EnableRaisingEvents = false, StartInfo = processStartInfo};

            process.Start();

            var stdOutput = process.StandardOutput.ReadToEnd();
            var stdError = process.StandardError.ReadToEnd();

            process.WaitForExit();
            process.Close();

            WritingJobCompletionNotificationToQueue(inMessage);
        }

        private void WritingJobCompletionNotificationToQueue(SigiriAzureInMessage inMessage)
        {
            _outMessageQueue.AddMessage(
                new CloudQueueMessage(
                    String.Format(
                        "<SigiriAzureOutMessage><AppId>{0}</AppId><JobId>{1}</JobId><Status>{2}</Status></SigiriAzureOutMessage>",
                        _sigiriApplicationConfiguration.ApplicationId, inMessage.JobId, "SUCCESSFUL")));
        }
    }
}