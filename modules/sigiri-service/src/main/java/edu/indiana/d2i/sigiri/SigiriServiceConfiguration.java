package edu.indiana.d2i.sigiri;

import com.sun.xml.internal.xsom.impl.Const;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Properties;

public class SigiriServiceConfiguration {
    private String dataSourceName;

    private String jndiProviderUrl;

    private String jndiInitialContextFactory;

    public SigiriServiceConfiguration(File configFile) throws IOException {
        Properties sigiriConfigurations = new Properties();
        sigiriConfigurations.load(new FileInputStream(configFile));

        this.dataSourceName = ((String)sigiriConfigurations.get(Constants.CONF_PROP_DATA_SOURCE)).trim();
        this.jndiProviderUrl = ((String)sigiriConfigurations.get(Constants.CONF_PROP_JNDI_SERVER_URL)).trim();
        this.jndiInitialContextFactory = ((String)sigiriConfigurations.get(Constants.CONF_PROP_INITIAL_CONTEXT_FACTORY)).trim();
    }

    public String getDataSourceName() {
        return dataSourceName;
    }

    public String getJndiProviderUrl() {
        return jndiProviderUrl;
    }

    public String getJndiInitialContextFactory() {
        return jndiInitialContextFactory;
    }
}
