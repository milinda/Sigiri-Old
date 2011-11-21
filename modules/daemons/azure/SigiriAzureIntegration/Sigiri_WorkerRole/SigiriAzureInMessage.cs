using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sigiri_WorkerRole
{
    class SigiriAzureInMessage
    {
        public string ApplicationId { set; get; }
        public string JobId { set; get; }

        public static SigiriAzureInMessage CreateSigiriAzureInMessageFromXml(string inMessageXmlString)
        {
            var inMessageDoc = new XmlDocument();
            inMessageDoc.LoadXml(inMessageXmlString);

            var inMessage = new SigiriAzureInMessage
                                {
                                    ApplicationId = inMessageDoc.GetElementsByTagName("AppId")[0].InnerText,
                                    JobId = inMessageDoc.GetElementsByTagName("JobId")[0].InnerText
                                };

            return inMessage;
        }
    }
}
