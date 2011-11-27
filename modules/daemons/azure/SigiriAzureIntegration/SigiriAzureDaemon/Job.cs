using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon
{
    class Job
    {
        public string JobDescription { set; get; }
        public string JobId { set; get; }
        public string Status { set; get; }

        public enum JobType
        {
            FileMovement,
            ApplicationExecution
        }

        public JobType Type { set; get; }
    }
}
