package edu.indiana.d2i.sigiri;

import org.apache.commons.logging.Log;
import org.apache.commons.logging.LogFactory;

import javax.naming.Context;
import javax.naming.InitialContext;
import javax.naming.NamingException;
import javax.sql.DataSource;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.Properties;

public class DBConnectionManager {
    private static Log log = LogFactory.getLog(DBConnectionManager.class);

    private SigiriServiceConfiguration sigiriServiceConfiguration;
    private DataSource sigiriDataSource;

    public DBConnectionManager(SigiriServiceConfiguration sigiriServiceConfiguration) throws NamingException {
        this.sigiriServiceConfiguration = sigiriServiceConfiguration;
        initDataSource();
    }

    public Connection getConnection() throws SQLException {
        return sigiriDataSource.getConnection();
    }

    private void initDataSource() throws NamingException {
        this.sigiriDataSource = lookupJNDI(sigiriServiceConfiguration.getDataSourceName());
    }

    private <T> T lookupJNDI(String dataSourceName) throws NamingException {
        ClassLoader old = Thread.currentThread().getContextClassLoader();
        Thread.currentThread().setContextClassLoader(getClass().getClassLoader());

        InitialContext initialContext = null;

        try {

            if (sigiriServiceConfiguration.getJndiInitialContextFactory() != null &&
                    sigiriServiceConfiguration.getJndiProviderUrl() != null) {
                Properties jndiProps = new Properties();

                jndiProps.setProperty(Context.INITIAL_CONTEXT_FACTORY,
                        sigiriServiceConfiguration.getJndiInitialContextFactory());
                jndiProps.setProperty(Context.PROVIDER_URL,
                        sigiriServiceConfiguration.getJndiProviderUrl());

                initialContext = new InitialContext(jndiProps);
            } else {
                initialContext = new InitialContext();
            }

            return (T) initialContext.lookup(dataSourceName);
        } finally {
            if (initialContext != null) {
                try {
                    initialContext.close();
                } catch (Exception e) {
                    log.error("Error closing JNDI connection.", e);
                }
            }
            Thread.currentThread().setContextClassLoader(old);
        }
    }
}
