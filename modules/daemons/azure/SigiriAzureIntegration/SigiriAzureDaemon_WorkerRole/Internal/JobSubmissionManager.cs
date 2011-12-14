﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

        public JobSubmissionManager(JobStore jobStore, ApplicationStore applicationStore)
        {
            _jobStore = jobStore;
            _applicationStore = applicationStore;
            _handlers = new Dictionary<string, Handler>();
            _handlerSequence = new LinkedList<string>();
        }

        /**
         * Loads the handler chain configuration for jub submission flow.
         * Initialize handler in job submission flow.
         */

        public void OnStart()
        {
            // TODO: Put handler objects in to _handlers dictionary and 
            // TODO: setup the handler sequence according to configuration.
            _handlerSequence.AddLast("ResourceIdentificationHandler");
            _handlerSequence.AddLast("CredentialManagementHandler");
            _handlerSequence.AddLast("InputDataMovementHandler");
            _handlerSequence.AddLast("WorkerRoleSetupHandler");
            _handlerSequence.AddLast("VMRoleSetupHandler");
            _handlerSequence.AddLast("ApplicationExecutionHandler");
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
                        Type = SigiriAzureDaemonContext.JobType.ApplicationExecution,
                        JobId = job.JobId,
                        ApplicationId =
                            _applicationStore.GetApplicationId(job.Executable)
                    };

                    jobSubmissionContext.AddParameter("Job", job);

                    foreach (var handler in _handlerSequence.Select(handlerId => _handlers[handlerId]))
                    {
                        handler.Invoke(jobSubmissionContext);
                    }
                }
            }
        }
    }
}