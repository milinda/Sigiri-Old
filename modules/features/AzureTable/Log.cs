using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;

namespace AzureTable
{
    //Log entry
    class Log: TableServiceEntity
    {
        public String logValue { get; set; }
        public DateTime TimeUtc { get; set; }
        public Guid Id { get; set; }

        public Log()
        {}
        //each table is partitioned on partition keys, and this entry(row) has a unique partition key and a unique row key within the partition
        public Log(Guid Id, DateTime timeUtc, string message)
            : base(Log.GetParitionKey(Id), Log.GetRowKey(timeUtc))
        {
            this.TimeUtc = timeUtc;
            this.Id = Id;
            this.logValue = message;
        }

        public static string GetParitionKey(Guid id)
        {
             return id.ToString().Replace("-", "").ToLower();
        }

        public static string GetRowKey(DateTime time)
        {
            string str = time.ToString("yyyyMMdd_HH_mm_ss");
            return str;
           
        }
       
        
       
       
    }
}
