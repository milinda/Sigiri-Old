package edu.indiana.d2i.sigiri;

import org.apache.commons.logging.Log;
import org.apache.commons.logging.LogFactory;
import org.quartz.Job;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Date;
import java.util.List;
import java.util.Map;
import java.util.UUID;

public class JobManager {
    private static final Log log = LogFactory.getLog(JobManager.class);

    private DBConnectionManager dbConnectionManager;

    public JobManager(DBConnectionManager dbConnectionManager) {
        this.dbConnectionManager = dbConnectionManager;
    }

    public String getJobStatus(String jobID) throws SQLException {
        String status = "";
        Connection connection = null;

        if (isJobExist(jobID)) {
            return Constants.JobStatus.NOT_AVAILABLE;
        }

        try {
            connection = dbConnectionManager.getConnection();

            String statusQuery = "SELECT " + Constants.ColumnNames.STATUS + ", " + Constants.ColumnNames.ERROR_DESCRIPTION +
                    " from " + Constants.TableNames.JOBS + " WHERE " + Constants.ColumnNames.INTERNAL_ID + "=?";

            PreparedStatement preparedStatement = connection.prepareStatement(statusQuery);

            preparedStatement.setString(1, jobID);

            ResultSet rs = preparedStatement.executeQuery();

            if (rs.next()) {
                status = rs.getString(1);

                if (status.equals("")) {
                    return Constants.JobStatus.NOT_AVAILABLE;
                }

            } else {
                status = Constants.JobStatus.NOT_AVAILABLE;
            }

            return status;
        } catch (SQLException e) {
            log.error("Error while getting job status from the database.", e);
            throw e;
        } finally {
            try {
                if (connection != null) {
                    connection.close();
                }
            } catch (SQLException e) {
                log.error("Error closing database connection.", e);
            }
        }
    }

    private boolean isJobExist(String jobId) throws SQLException {
        Connection connection = null;
        try {
            connection = dbConnectionManager.getConnection();
            String checkJobExistQuery = "SELECT COUNT(`Id`) FROM `Jobs` WHERE `InternalId`=?";

            PreparedStatement preparedStatement = connection.prepareStatement(checkJobExistQuery);

            preparedStatement.setString(1, jobId);

            ResultSet rs = preparedStatement.executeQuery();

            if (rs.next()) {
                int count = rs.getInt(1);
                if (count > 0) {
                    return true;
                }
            }
        } catch (SQLException e) {
            log.error("Error while checking existence of the job in database.", e);
            throw e;
        } finally {
            try {
                if (connection != null) {
                    connection.close();
                }
            } catch (SQLException e) {
                log.error("Error closing database connection.", e);
            }
        }

        return false;
    }

    public String addJobToQueue(String jobDescription, String hpcResoureName, String callbackURL, Map<String, String> qosParams) throws SQLException {
        String jobKey = "";
        Connection connection = null;
        try {
            connection = dbConnectionManager.getConnection();

            String sql = "INSERT INTO " + Constants.TableNames.JOBS + " (" +
                    Constants.ColumnNames.JOBS_JOB_DESCRIPTION_XML + ", " +
                    Constants.ColumnNames.JOBS_HPC_RESOURCE_NAME + ", " +
                    Constants.ColumnNames.JOBS_CALLBACK_URL + ", " +
                    Constants.ColumnNames.STATUS + ", " +
                    Constants.ColumnNames.INTERNAL_ID + ", " +
                    Constants.ColumnNames.JOBS_JOB_SUBMITTED_TIME +
                    ") VALUES(?,?,?,?,?,?)";

            PreparedStatement preparedStatement = connection.prepareStatement(sql);

            preparedStatement.setString(1, jobDescription);
            preparedStatement.setString(2, hpcResoureName);
            preparedStatement.setString(3, callbackURL == null || "".equals(callbackURL) ? "NULL" : callbackURL);
            preparedStatement.setString(4, Constants.JobStatus.ACCEPTED);

            jobKey = UUID.randomUUID().toString();
            preparedStatement.setString(5, jobKey);
            preparedStatement.setLong(6, new Date().getTime());

            preparedStatement.executeUpdate();
            preparedStatement.close();

            if (log.isDebugEnabled()) {
                log.debug("New job added to the database. Internal job id = " + jobKey);
            }

            // Adding QOS Parameters
            if (qosParams != null && qosParams.size() > 0) {
                sql = "INSERT INTO " + Constants.TableNames.QOSParams + " (" +
                        Constants.ColumnNames.QOSPARAMS_JOB_INTERNAL_ID + ", " +
                        Constants.ColumnNames.QOSPARAMS_NAME + ", " +
                        Constants.ColumnNames.QOSPARAMS_VALUE +
                        ") VALUES(?,?,?)";
                preparedStatement = connection.prepareStatement(sql);

                for (Map.Entry<String, String> parameter : qosParams.entrySet()) {
                    preparedStatement.setString(1, jobKey);
                    preparedStatement.setString(2, parameter.getKey());
                    preparedStatement.setString(3, parameter.getValue());

                    preparedStatement.executeUpdate();
                    preparedStatement.close();
                }

                if (log.isDebugEnabled()) {
                    log.debug(qosParams.size() + " QOS parameters were added to job " + jobKey);
                }
            }

        } catch (SQLException e) {
            log.error("Job Submission Failed - Error while accessing database.", e);
            throw e;
        } finally {
            try {
                if (connection != null) {
                    connection.close();
                }
            } catch (SQLException e) {
                log.error("Error closing database connection.", e);
            }
        }
        return jobKey;
    }


}
