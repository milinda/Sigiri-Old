namespace SigiriAzureDaemon_WorkerRole.Internal
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
        public string WallClockLimit { set; get; }
        public string InitialDir { set; get; }
        public string MaxTime { set; get; }
        public string NodeCount { set; get; }
        public string Executable { set; get; }
        public string ExecutionType { set; get; }
        public string UseSameResourceUserName { set; get; }
        public string UseSameResourceUserProvidedId { set; get; }
        public long JobSubmittedTime { set; get; }
    }
}
