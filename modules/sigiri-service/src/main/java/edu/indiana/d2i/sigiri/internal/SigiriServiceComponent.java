package edu.indiana.d2i.sigiri.internal;

import edu.indiana.d2i.sigiri.DBConnectionManager;
import edu.indiana.d2i.sigiri.JobManager;
import edu.indiana.d2i.sigiri.SigiriServiceConfiguration;
import org.apache.commons.logging.Log;
import org.apache.commons.logging.LogFactory;
import org.osgi.framework.BundleContext;
import org.osgi.service.component.ComponentContext;
import org.wso2.carbon.datasource.DataSourceInformationRepositoryService;
import org.wso2.carbon.registry.core.service.RegistryService;
import org.wso2.carbon.utils.CarbonUtils;

import java.io.File;

/**
 * @scr.component name="edu.indiana.d2i.sigiri.SigiriServiceComponent" immediate="true"
 * @scr.reference name="datasource.information.repository.service"
 * interface="org.wso2.carbon.datasource.DataSourceInformationRepositoryService"
 * cardinality="1..1" policy="dynamic"  bind="setDataSourceInformationRepositoryService"
 * unbind="unsetDataSourceInformationRepositoryService"
 * @scr.reference name="registry.service" interface="org.wso2.carbon.registry.core.service.RegistryService"
 * cardinality="1..1" policy="dynamic"  bind="setRegistryService" unbind="unsetRegistryService"
 */
public class SigiriServiceComponent {
    private static Log log = LogFactory.getLog(SigiriServiceComponent.class);
    private BundleContext bundleContext;
    private boolean dataSourceInfoRepoProvided = false;
    private static DBConnectionManager dbConnectionManager;
    private static JobManager jobManager;

    protected void activate(ComponentContext componentContext){
        try{
            this.bundleContext = componentContext.getBundleContext();
            if(dataSourceInfoRepoProvided){
                File sigiriConfigurationFile = new File(CarbonUtils.getCarbonConfigDirPath() + File.separator + "sigiri.conf" );
                dbConnectionManager = new DBConnectionManager(new SigiriServiceConfiguration(sigiriConfigurationFile));
                jobManager = new JobManager(dbConnectionManager);
                log.info("Sigiri Service Initialized.....");
            }

        } catch (Throwable t){
            log.error("Failed to activate Sigiri Core bundle", t);
        }
    }

    public static DBConnectionManager getDbConnectionManager(){
        return dbConnectionManager;
    }

    public static JobManager getJobManager(){
        return jobManager;
    }

    protected void setDataSourceInformationRepositoryService(
            DataSourceInformationRepositoryService repositoryService) {
        if (log.isDebugEnabled()) {
            log.debug("DataSourceInformationRepositoryService bound to the Sigiri component");
        }
        this.dataSourceInfoRepoProvided = true;
    }

    protected void unsetDataSourceInformationRepositoryService(
            DataSourceInformationRepositoryService repositoryService) {
        if (log.isDebugEnabled()) {
            log.debug("DataSourceInformationRepositoryService unbound from the Sigiri component");
        }
        this.dataSourceInfoRepoProvided = false;
    }

    protected void setRegistryService(RegistryService registrySvc) {
        if (log.isDebugEnabled()) {
            log.debug("RegistryService bound to the BPEL component");
        }
    }

    protected void unsetRegistryService(RegistryService registrySvc) {
        if (log.isDebugEnabled()) {
            log.debug("RegistryService unbound from the BPEL component");
        }
    }

}
