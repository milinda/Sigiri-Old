using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;

namespace AzureTable
{
    class Program
    {
        static void Main(string[] args)
        {
            string tableName = "logtable";
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = storageAccount.CreateCloudTableClient();

            tableClient.CreateTableIfNotExist(tableName);
            IEnumerable<string> listOfTables =  tableClient.ListTables();
            foreach (string table in listOfTables)
                Console.WriteLine(table);

             LogsContext logsContext = new LogsContext(storageAccount.TableEndpoint.ToString(),
                                                        storageAccount.Credentials,tableName);

            Guid jobId = Guid.NewGuid();
            logsContext.AddLog(new Log(jobId,DateTime.UtcNow, String.Format("[PERF][Type=@Worker-Role][Id={0}][Response={1}]",
                                           jobId, System.DateTime.UtcNow)));

            IQueryable<Log> logs = logsContext.Logs;
            foreach (Log log in logs)
            {
                String logValue = log.logValue;
                Console.WriteLine("Log:" + logValue);
            }
                
            //efficient querying based on jobId
            
        }
    }
}
