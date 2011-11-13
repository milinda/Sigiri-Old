package edu.indiana.d2i.sigiri;

public class Constants {

    public static final String CONF_PROP_DATA_SOURCE = "sigiri.conf.datasource";
    public static final String CONF_PROP_INITIAL_CONTEXT_FACTORY = "sigiri.conf.initial.context.factory";
    public static final String CONF_PROP_JNDI_SERVER_URL = "sigiri.conf.jndi.server.url";

    public static final String DESC_JOB_SUBMISSION_SUCCESSFUL = "Job Submission Successful.";

     public abstract class TableNames {
        public static final String JOBS = "Jobs";
        public static final String QOSParams = "QOSParams";
        public static final String DAEMONS = "Daemons";
        public static final String FILE_MOVEMENT_JOBS = "FileMovementJobs";
    }

    public abstract class ColumnNames {
        public static final String JOBS_CALLBACK_URL = "CallbackURL";
        public static final String INTERNAL_ID = "InternalId";
        public static final String JOBS_HPC_RESOURCE_NAME = "HPCResourceName";
        public static final String JOBS_JOB_MANAGER_NAME = "JobManagerName";
        public static final String ERROR_DESCRIPTION = "ErrorDescription";
        public static final String JOBS_NOTIFICATION_STATUS = "NotificationStatus";
        public static final String JOBS_Id = "Id";
        public static final String JOBS_JOB_DESCRIPTION_XML = "JobDescriptionXML";
        public static final String STATUS = "Status";
        public static final String JOBS_JOB_ID = "JobId";
        public static final String JOBS_WALL_CLOCK_LIMIT = "WallClockLimit";
        public static final String JOBS_JOB_SUBMITTED_TIME = "JobSubmittedTime";
        public static final String JOBS_JOB_SCHEDULED_TO_BE_KILLED = "JobToBeKilled";

        public static final String QOSPARAMS_ID = "Id";
        public static final String QOSPARAMS_JOB_INTERNAL_ID = "InternalJobId";
        public static final String QOSPARAMS_NAME = "ParamName";
        public static final String QOSPARAMS_VALUE = "ParamValue";

        public static final String DAEMONS_ID = "Id";
        public static final String DAEMONS_HPC_RESOURCE_NAME = "HPCResourceName";
        public static final String DAEMONS_JOB_MANAGER_NAME = "JobManagerName";
        public static final String DAEMONS_CONFIGURATION = "Configuration";
        public static final String DAEMONS_LAST_HEART_BEAT_TIME = "LastHeartBeatTime";
        public static final String DAEMONS_HEART_BEAT_INTERVAL = "HeartBeatInterval";

        public static final String FMJ_ID = "Id";
        public static final String FMJ_SOURCE_LOCATION = "SourceLocation";
        public static final String FMJ_SOURCE_LOCATION_TYPE = "SourceLocationType";
        public static final String FMJ_FILE_NAME_FILTER = "FileNameFilter";
        public static final String FMJ_TARGET_LOCATION = "TargetLocation";
        public static final String FMJ_TARGET_LOCATION_TYPE = "TargetLocationType";
        public static final String FMJ_HPC_RESOURCE_NAME = "HPCResourceName";
        public static final String FMJ_IS_COPY = "IsCopy";
        public static final String FMJ_ERROR_DESC = "ErrorDescription";

        public static final String FMJ_USERNAME = "Username";
    }

    public abstract class JobStatus {
        public static final String ACCEPTED = "Accepted";
        public static final String SUBMITTING_TO_JOB_MANAGER = "Submitting to Job Manager";
        public static final String SUBMITTED_TO_JOB_MANAGER = "Submitted To Job Manager";
        public static final String JOB_SUBMISSION_FAILED = "Job Submission Failed";
        public static final String JOB_COMPLETED = "JOB_COMPLETED";
        public static final String FAILED = "Failed";
        public static final String NOTIFICATION_SENT = "Notification Sent";
        public static final String NOT_AVAILABLE = "Job Not Available";
        public static final String STATUS_NOT_AVAILABLE = "Status Not Available";
        public static final String STATE_IDLE = "STATE_IDLE";
        public static final String STATE_JOB_STARTED = "JOB_STARTED";
        public static final String STATE_JOB_KILLED = "JOB_KILLED";
        public static final String STATE_JOB_HELD = "JOB_HELD";
        public static final String STATE_JOB_SCHEDULED_TO_BE_KILLED = "Job Scheduled to be killed";
    }

}
