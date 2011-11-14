/**
 * Copyright (c) 2011, Data to Insight Center. (http://pti.iu.edu/d2i) All Rights Reserved.
 * <p/>
 * WSO2 Inc. licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file except
 * in compliance with the License.
 * You may obtain a copy of the License at
 * <p/>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p/>
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
package edu.indiana.d2i.sigiri.service;

import edu.indiana.d2i.sigiri.Constants;
import edu.indiana.d2i.sigiri.JobManager;
import edu.indiana.d2i.sigiri.internal.SigiriServiceComponent;
import edu.indiana.d2i.sigiri.service.types.*;
import org.apache.bcel.classfile.Constant;
import org.apache.commons.logging.Log;
import org.apache.commons.logging.LogFactory;

import javax.transaction.Status;
import javax.xml.stream.XMLStreamException;
import java.sql.SQLException;
import java.util.Collections;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;

public class SigiriServiceSkeleton implements SigiriServiceSkeletonInterface {
    private static final Log log = LogFactory.getLog(SigiriServiceSkeleton.class);


    public JobStatusType checkStatus(String jobId0) {
        if (log.isDebugEnabled()) {
            log.debug("Checking status of job " + jobId0);
        }

        JobManager jobManager = SigiriServiceComponent.getJobManager();

        JobStatusType jobStatus = new JobStatusType();
        jobStatus.setStatus(Status_type1.JOB_NOT_AVAILABLE);
        jobStatus.setDescription(Constants.JobDescriptions.JOB_DESC_EMPTY);

        try {
            String status = jobManager.getJobStatus(jobId0);
            String description = jobManager.getJobDescription(jobId0);
            jobStatus.setStatus(Status_type1.Factory.fromValue(status));

            if(description != null){
                jobStatus.setDescription(description);
            }

        } catch (SQLException e) {
            log.error("Job status check failed for job " + jobId0 + ".", e);
            setErrorStatus(jobStatus, "Job status check failed for job " + jobId0 + ".", Status_type1.JOB_STATUS_CHECK_FAILED);
        }

        return jobStatus;
    }

    public JobStatusType submitJob(XMLContent jobDescriptionXML,
                                   HpcResourceName hpcResource13,
                                   String callbackURL,
                                   QOSParameter[] qOSParameters14) {
        if (log.isDebugEnabled()) {
            log.debug("Submitting new job to queue..");
        }

        Date jobSubmisstionTime = new Date();
        JobManager jobManager = SigiriServiceComponent.getJobManager();

        JobStatusType jobStatus = new JobStatusType();
        jobStatus.setStatus(Status_type1.JOB_SUBMISSION_FAILED);

        try {
            String jobId = jobManager.addJobToQueue(jobDescriptionXML.getExtraElement().toStringWithConsume(),
                    hpcResource13.getValue(), callbackURL, getQOSParametersAsNameValuePairs(qOSParameters14));

            jobStatus.setJobId(jobId);
            jobStatus.setStatus(Status_type1.JOB_SUBMISSION_ACCEPTED);
            jobStatus.setDescription(Constants.DESC_JOB_SUBMISSION_SUCCESSFUL);
        } catch (XMLStreamException e) {
            log.error("Job submission failed.", e);
            setErrorStatus(jobStatus, "Job submission failed due XML parsing error." + e.getMessage(), Status_type1.JOB_SUBMISSION_FAILED);
        } catch (SQLException e) {
            log.error("Job submission failed.", e);
            setErrorStatus(jobStatus, "Job submission failed due to dataase error." + e.getMessage(), Status_type1.JOB_SUBMISSION_FAILED);
        }

        return jobStatus;
    }

    public JobStatusType[] checkMultipleJobStatus(String[] id) {
        return new JobStatusType[0];
    }

    public JobStatusType moveFiles(String username, HpcResourceName hpcResource, T_Movement[] movement, QOSParameter[] qOSParameters) {
        return null;
    }

    public JobStatusType killJob(String jobId8) {
        return null;
    }

    private void setErrorStatus(JobStatusType jobStatus, String description, Status_type1 status) {
        jobStatus.setDescription(description);
        jobStatus.setJobId("NO_ID");
        jobStatus.setStatus(status);
    }

    private Map<String, String> getQOSParametersAsNameValuePairs(QOSParameter[] qosParameters) {
        Map<String, String> qosParameterMap = new HashMap<String, String>();
        if (qosParameters != null) {
            for (QOSParameter parameter : qosParameters) {
                qosParameterMap.put(parameter.getName(), parameter.getValue());
            }
            return qosParameterMap;
        }
        return Collections.emptyMap();
    }
}
