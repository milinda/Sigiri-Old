using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    class SigiriAzureOutMessage
    {
        public string ApplicationId { set; get; }
        public string JobId { set; get; }
        public string Status { set; get; }

        public static SigiriAzureOutMessage CreateSigiriAzureOutMessageFromXML(string outMessage)
        {
            var outMessageDoc = new XmlDocument();
            outMessageDoc.LoadXml(outMessage);

            return new SigiriAzureOutMessage()
                       {
                           ApplicationId = outMessageDoc.GetElementsByTagName("AppId")[0].InnerText,
                           JobId = outMessageDoc.GetElementsByTagName("JobId")[0].InnerText,
                           Status = outMessageDoc.GetElementsByTagName("Status")[0].InnerText
                       };
        }
    }
}
