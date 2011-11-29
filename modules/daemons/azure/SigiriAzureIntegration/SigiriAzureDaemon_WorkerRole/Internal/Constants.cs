namespace SigiriAzureDaemon_WorkerRole.Internal
{
    internal class Constants
    {
        // TODO: move to common lib
        public abstract class ColumnNames
        {
            public const string JOBS_CALLBACK_URL = "CallbackURL";
            public const string INTERNAL_ID = "InternalId";
            public const string JOBS_HPC_RESOURCE_NAME = "HPCResourceName";
            public const string JOBS_JOB_MANAGER_NAME = "JobManagerName";
            public const string ERROR_DESCRIPTION = "ErrorDescription";
            public const string JOBS_NOTIFICATION_STATUS = "NotificationStatus";
            public const string JOBS_Id = "Id";
            public const string JOBS_JOB_DESCRIPTION_XML = "JobDescriptionXML";
            public const string STATUS = "Status";
            public const string JOBS_JOB_ID = "JobId";
            public const string JOBS_WALL_CLOCK_LIMIT = "WallClockLimit";
            public const string JOBS_JOB_SUBMITTED_TIME = "JobSubmittedTime";
            public const string JOBS_JOB_SCHEDULED_TO_BE_KILLED = "JobToBeKilled";

            public const string QOSPARAMS_ID = "Id";
            public const string QOSPARAMS_JOB_INTERNAL_ID = "InternalJobId";
            public const string QOSPARAMS_NAME = "ParamName";
            public const string QOSPARAMS_VALUE = "ParamValue";

            public const string DAEMONS_ID = "Id";
            public const string DAEMONS_HPC_RESOURCE_NAME = "HPCResourceName";
            public const string DAEMONS_JOB_MANAGER_NAME = "JobManagerName";
            public const string DAEMONS_CONFIGURATION = "Configuration";
            public const string DAEMONS_LAST_HEART_BEAT_TIME = "LastHeartBeatTime";
            public const string DAEMONS_HEART_BEAT_INTERVAL = "HeartBeatInterval";

            public const string FMJ_ID = "Id";
            public const string FMJ_INTERNAL_ID = "InternalId";
            public const string FMJ_SOURCE_LOCATION = "SourceLocation";
            public const string FMJ_SOURCE_LOCATION_TYPE = "SourceLocationType";
            public const string FMJ_FILE_NAME_FILTER = "FileNameFilter";
            public const string FMJ_TARGET_LOCATION = "TargetLocation";
            public const string FMJ_TARGET_LOCATION_TYPE = "TargetLocationType";
            public const string FMJ_HPC_RESOURCE_NAME = "HPCResourceName";
            public const string FMJ_USERNAME = "Username";

            public const string FMJ_IS_COPY = "IsCopy";
            public const string FMJ_ERROR_DESC = "ErrorDescription";

            public const string USER_CRED_ID = "Id";
            public const string USER_CRED_USERNAME = "Username";
            public const string USER_CRED_PRIVATE_KEY = "PrivateKey";
            public const string USER_CRED_SECRET_KEY = "SecretKey";
            public const string USER_CRED_RESOURCE_NAME = "ResourceName";
            public const string USER_CRED_ACCOUNT = "Account";
        }

        public abstract class TableNames
        {
            public const string JOBS = "Jobs";
            public const string QOSParams = "QOSParams";
            public const string DAEMONS = "Daemons";
            public const string FILE_MOVEMENT_JOBS = "FileMovementJobs";
            public const string USER_CREDENTIALS = "UserCredentials";
        }

        public abstract class JobStatus
        {
            public const string JOB_SUBMISSION_ACCEPTED = "JOB_SUBMISSION_ACCEPTED";
            public const string JOB_SUBMISSION_FAILED = "JOB_SUBMISSION_FAILED";
            public const string JOB_STATUS_CHECK_FAILED = "JOB_STATUS_CHECK_FAILED";
            public const string JOB_NOT_AVAILABLE = "JOB_NOT_AVAILABLE";
            public const string JOB_PICKED_BY_MANAGEMENT_DAEMON = "JOB_PICKED_BY_MANAGEMENT_DAEMON";
            public const string JOB_SUBMITTED_TO_COMPUTING_RESOURCE = "JOB_SUBMITTED_TO_COMPUTING_RESOURCE";

            public const string JOB_SUBMISSION_TO_COMPUTING_RESOURCE_FAILED =
                "JOB_SUBMISSION_TO_COMPUTING_RESOURCE_FAILED";

            public const string JOB_COMPLETED_SUCCESSFULLY = "JOB_COMPLETED_SUCCESSFULLY";
            public const string JOB_FAILED = "JOB_FAILED";
        }
    }
}