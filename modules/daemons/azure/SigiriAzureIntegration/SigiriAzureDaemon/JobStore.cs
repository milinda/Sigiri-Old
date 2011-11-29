using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace SigiriAzureDaemon
{
    internal class JobStore
    {
        private string _computingResourceName;
        private string _jobManagerName;

        public JobStore(string computingReesourceName, string jobManagerName)
        {
            _computingResourceName = computingReesourceName;
            _jobManagerName = jobManagerName;
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
                           Constants.JobStatus.ACCEPTED + "' AND " +
                           Constants.ColumnNames.JOBS_HPC_RESOURCE_NAME + "= '" +
                           _computingResourceName + "' AND (" + Constants.ColumnNames.JOBS_JOB_MANAGER_NAME +
                           "= '" + _jobManagerName + "' OR " +
                           Constants.ColumnNames.JOBS_JOB_MANAGER_NAME + "= '') AND " + Constants.TableNames.JOBS + "." +
                           Constants.ColumnNames.INTERNAL_ID + "= QOSParams1." + Constants.ColumnNames.QOSPARAMS_JOB_INTERNAL_ID +
                           " AND " + Constants.TableNames.JOBS + "." +
                           Constants.ColumnNames.INTERNAL_ID + "= QOSParams2." + Constants.ColumnNames.QOSPARAMS_JOB_INTERNAL_ID +
                           " AND QOSParams1.ParamName='useSameResourceUsername' AND QOSParams2.ParamName='useSameResourceUserProvidedId'";


            command.CommandText = sql;


            connection.Open();


            MySqlDataReader reader = command.ExecuteReader();


            // Once we retrieve jobs to be executed from the database, we should mark that record as read in the database.
            // I was using batch update in the java code to do that, but I still couldn't find a way to do that in C#. 
            // Until I find that I will collect all the update statements and execute them sequentially. 
            var updateStatements = new List<string>();

            while (reader.Read())
            {
                var job = new Job();
                var internalJobId = reader.GetString(Constants.ColumnNames.INTERNAL_ID);

                updateStatements.Add("Update " + Constants.TableNames.JOBS +
                        " SET " + Constants.ColumnNames.STATUS + " =  '" + Constants.JobStatus.SUBMITTING_TO_JOB_MANAGER + "'" +
                        " where " + Constants.ColumnNames.INTERNAL_ID + " = '" + internalJobId + "'");

                job.useSameResourceUsername = reader.GetString("userName");
                job.useSameResourceUserProvidedId = reader.GetString("userId");
                job.internalId = internalJobId;
                job.jobDescription = reader.GetString(Constants.ColumnNames.JOBS_JOB_DESCRIPTION_XML);
                job.jobSubmittedTime = reader.GetInt64(Constants.ColumnNames.JOBS_JOB_SUBMITTED_TIME);

                // Fill the fields in job info bean with the information inside job description xml
                JobScriptConverter jobScriptConverter = new JobScriptConverter();
                jobScriptConverter.convert(job.jobDescription, job);

                jobsToBeExecuted.Add(job);
            }

            reader.Close();

            // Now update the database using the collected update statements.
            foreach (string updateStatement in updateStatements)
            {
                command.CommandText = updateStatement;
                command.ExecuteNonQuery();
            }

            connection.Close();


            return jobsToBeExecuted;
            return new List<Job>();
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