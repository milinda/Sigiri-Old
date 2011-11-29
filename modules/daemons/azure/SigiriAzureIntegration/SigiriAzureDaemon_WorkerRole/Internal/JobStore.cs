using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    internal class JobStore
    {
        private readonly string _computingResourceName;
        private readonly string _jobManagerName;
        private readonly List<string> _jobStatusUpdateQueries;

        public JobStore(string computingReesourceName, string jobManagerName)
        {
            _computingResourceName = computingReesourceName;
            _jobManagerName = jobManagerName;
            _jobStatusUpdateQueries = new List<string>();
        }

        public List<Job> GetJobsToBeScheduledForExecution()
        {
            var jobsToBeExecuted = new List<Job>();
            var connection = GetDatabaseConnection();
            var command = connection.CreateCommand();


            string sql = "SELECT " + Constants.ColumnNames.INTERNAL_ID + ", " +
                         Constants.ColumnNames.JOBS_JOB_DESCRIPTION_XML + ", " +
                         Constants.ColumnNames.JOBS_JOB_SUBMITTED_TIME + ", " +
                         "QOSParams1." + Constants.ColumnNames.QOSPARAMS_VALUE + " as userName, " +
                         "QOSParams2." + Constants.ColumnNames.QOSPARAMS_VALUE + " as userId" +
                         " FROM " + Constants.TableNames.JOBS + "," +
                         Constants.TableNames.QOSParams + " AS QOSParams1 ," +
                         Constants.TableNames.QOSParams + " AS QOSParams2 " +
                         " WHERE " + Constants.ColumnNames.STATUS + " = '" +
                         Constants.JobStatus.JOB_SUBMISSION_ACCEPTED + "' AND " +
                         Constants.ColumnNames.JOBS_HPC_RESOURCE_NAME + "= '" +
                         _computingResourceName + "' AND (" + Constants.ColumnNames.JOBS_JOB_MANAGER_NAME +
                         "= '" + _jobManagerName + "' OR " +
                         Constants.ColumnNames.JOBS_JOB_MANAGER_NAME + "= '') AND " + Constants.TableNames.JOBS + "." +
                         Constants.ColumnNames.INTERNAL_ID + "= QOSParams1." +
                         Constants.ColumnNames.QOSPARAMS_JOB_INTERNAL_ID +
                         " AND " + Constants.TableNames.JOBS + "." +
                         Constants.ColumnNames.INTERNAL_ID + "= QOSParams2." +
                         Constants.ColumnNames.QOSPARAMS_JOB_INTERNAL_ID +
                         " AND QOSParams1.ParamName='useSameResourceUsername' AND QOSParams2.ParamName='useSameResourceUserProvidedId'";


            command.CommandText = sql;

            connection.Open();

            MySqlDataReader reader = command.ExecuteReader();

            // Once we retrieve jobs to be executed from the database, we should mark that record as read in the database.
            // I was using batch update in the java code to do that, but I still couldn't find a way to do that in C#. 
            // Until I find that I will collect all the update statements and execute them sequentially. 
            while (reader.Read())
            {
                var job = new Job();
                var internalJobId = reader.GetString(Constants.ColumnNames.INTERNAL_ID);

                AddJobStatusUpdateQuery(internalJobId, Constants.JobStatus.JOB_PICKED_BY_MANAGEMENT_DAEMON);

                job.UseSameResourceUserName = reader.GetString("userName");
                job.UseSameResourceUserProvidedId = reader.GetString("userId");
                job.JobId = internalJobId;
                job.JobDescription = reader.GetString(Constants.ColumnNames.JOBS_JOB_DESCRIPTION_XML);
                job.JobSubmittedTime = reader.GetInt64(Constants.ColumnNames.JOBS_JOB_SUBMITTED_TIME);

                // Fill the fields in job info bean with the information inside job description xml
                var jobScriptConverter = new JobScriptConverter();
                jobScriptConverter.Convert(job.JobDescription, job);

                jobsToBeExecuted.Add(job);
            }


            reader.Close();
            connection.Close();

            // Executing job status update queries as a batch.
            BatchUpdateJobStatus();

            return jobsToBeExecuted;
        }

        private void AddJobStatusUpdateQuery(string jobId, string newStatus)
        {
            _jobStatusUpdateQueries.Add("Update " + Constants.TableNames.JOBS +
                                        " SET " + Constants.ColumnNames.STATUS + " =  '" +
                                        newStatus + "'" +
                                        " where " + Constants.ColumnNames.INTERNAL_ID + " = '" + jobId + "'");
        }

        private void BatchUpdateJobStatus()
        {
            var connection = GetDatabaseConnection();
            var command = connection.CreateCommand();

            foreach (var updateStatement in _jobStatusUpdateQueries)
            {
                command.CommandText = updateStatement;
                command.ExecuteNonQuery();
            }

            connection.Close();
            _jobStatusUpdateQueries.Clear();
        }

        public List<Job> GetFileMovementJobs()
        {
            return new List<Job>();
        }

        public void UpdateExecutionJobStatus()
        {
        }

        public void UpdateFileMovementJobStatus()
        {
        }

        public MySqlConnection GetDatabaseConnection()
        {
            var dbUrl = ConfigurationManager.AppSettings["DB.Url"];
            var dbUser = ConfigurationManager.AppSettings["DB.User"];
            var dbPassword = ConfigurationManager.AppSettings["DB.Password"];
            var dbName = ConfigurationManager.AppSettings["DB.Name"];

            var connecitonString = String.Format("SERVER={0};" +
                                                 "DATABASE={1};" +
                                                 "UID={2};" +
                                                 "PASSWORD={3};",
                                                 dbUrl,
                                                 dbName,
                                                 dbUser,
                                                 dbPassword);
            return new MySqlConnection(connecitonString);
        }
    }
}