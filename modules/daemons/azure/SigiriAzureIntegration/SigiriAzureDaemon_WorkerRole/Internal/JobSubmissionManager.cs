using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using SigiriAzureDaemon_WorkerRole.Internal.Handlers;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    /**
     * Retrieve jobs to submitted to computing resource from jobs store and
     * send them through job submission handler chain. Manage the job
     * submission handler chain.
     */

    internal class JobSubmissionManager : IWorker
    {
        private readonly JobStore _jobStore;

        private readonly ApplicationStore _applicationStore;

        private readonly Dictionary<string, Handler> _handlers;

        private readonly LinkedList<string> _handlerSequence;

        private readonly SigiriAzureDaemonConfiguration _daemonConfiguration;

        public JobSubmissionManager(SigiriAzureDaemonConfiguration daemonConfiguration, JobStore jobStore,
                                    ApplicationStore applicationStore)
        {
            _daemonConfiguration = daemonConfiguration;
            _jobStore = jobStore;
            _applicationStore = applicationStore;
            _handlers = new Dictionary<string, Handler>();
            _handlerSequence = new LinkedList<string>();
        }

        /// <summary>
        /// Loads the handler chain configuration for jub submission flow.
        /// Initialize handler in job submission flow.
        /// </summary>
        public void OnStart()
        {
            InitializeHandlerSequence();
        }

        public void OnStop()
        {
            Trace.TraceInformation("Job Submission Manager Shuting Down....");
        }

        public void Run()
        {
            while (true)
            {
                // TODO: Find out the possibility of submitting several jobs at once using multiple threads.
                // TODO: We may have to make handlers thread safe.
                var jobs = _jobStore.GetJobsToBeScheduledForExecution();
                foreach (var job in jobs)
                {
                    var jobSubmissionContext = new JobSubmissionContext
                                                   {
                                                       Type = JobSubmissionContext.JobType.ApplicationExecution,
                                                       JobId = job.JobId,
                                                       ApplicationId =
                                                           _applicationStore.GetApplicationId(job.Executable),
                                                       DaemonConfiguration = _daemonConfiguration
                                                   };

                    jobSubmissionContext.AddParameter("Job", job);

                    foreach (var handler in _handlerSequence.Select(handlerId => _handlers[handlerId]))
                    {
                        handler.Invoke(jobSubmissionContext);
                    }
                }

                Thread.Sleep(2000);
            }
        }

        private void InitializeHandlerSequence()
        {
            InitHandlers();
            _handlerSequence.AddLast("ResourceIdentificationHandler");
            _handlerSequence.AddLast("CredentialManagementHandler");
            _handlerSequence.AddLast("InputDataMovementHandler");
            _handlerSequence.AddLast("WorkerRoleSetupHandler");
            _handlerSequence.AddLast("ApplicationExecutionHandler");
        }

        private void InitHandlers()
        {
            // TODO: Add handler description based initialization.
            _handlers.Add("ResourceIdentificationHandler", new ResourceIdentificationHandler());
            _handlers.Add("CredentialManagementHandler", new CredentialManagementHandler());
            _handlers.Add("InputDataMovementHandler", new InputDataMovementHandler());
            _handlers.Add("WorkerRoleSetupHandler", new WorkerRoleSetupHandler());
            _handlers.Add("ApplicationExecutionHandler", new ApplicationExecutionHandler());
        }
    }
}