using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;

namespace AzureTable
{
    //Like a Log table-it will contain only Log entries
    class LogsContext : TableServiceContext 
    {
        private String tableName;

        public LogsContext(String baseAddress, StorageCredentials credentials,String logTableName)
            : base(baseAddress, credentials)
        {
            this.tableName = logTableName;
        }

        public IQueryable<Log> Logs
        {
            get
            {
                return this.CreateQuery<Log>(tableName);
            }
        }

        public void AddLog(Log log)
        {
            this.AddObject(tableName, log);
            this.SaveChanges();
        }
    }
}
