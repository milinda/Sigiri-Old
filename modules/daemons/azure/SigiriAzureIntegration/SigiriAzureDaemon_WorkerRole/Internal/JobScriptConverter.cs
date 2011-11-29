using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    class JobScriptConverter
    {
        public static string RslNamespace = "http://www.globus.org/namespaces/2004/10/gram/job/description";
        public static string JsdlNamesapce = "http://www.ggf.org/namespaces/2004/11/jsdl-1.0.xsd";

        public void Convert(String jobDescription, Job jobInfo)
        {
            if (jobInfo == null)
            {
                jobInfo = new Job();
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(jobDescription);

            if (xmlDocument.DocumentElement == null) return;

            var namespaceUri = xmlDocument.DocumentElement.NamespaceURI;
            var paramsMap = RslNamespace.Equals(namespaceUri)
                                ? loadRSLToParamMap()
                                : loadJSDLToParamMap();


            var parameterValues = LoadParameterValuesFromJobDescription(paramsMap,
                                                                        xmlDocument);
            FillJobInfoBean(jobInfo, parameterValues);
        }

        private static void FillJobInfoBean(Job jobInfo, Dictionary<string, string> parameterValues)
        {
            jobInfo.WallClockLimit = GetParameter("wall_clock_limit", parameterValues);
            jobInfo.InitialDir = GetParameter("initialdir", parameterValues);
            jobInfo.MaxTime = GetParameter("maxTime", parameterValues);
            jobInfo.NodeCount = GetParameter("node", parameterValues);
            jobInfo.Executable = GetParameter("executable", parameterValues);
            jobInfo.ExecutionType = "mpi".Equals(GetParameter("jobType", parameterValues)) ? "Parallel" : "Sequential";
        }

        private static string GetParameter(string parameterName, Dictionary<string, string> parameters)
        {
            return parameters.ContainsKey(parameterName) ? parameters[parameterName] : "";
        }

        private static Dictionary<string, string> LoadParameterValuesFromJobDescription(Dictionary<string, string> paramsMap, XmlDocument xmlDocument)
        {
            // Add the namespace.
            var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            var paramValues = new Dictionary<string, string>();

            if (xmlDocument.DocumentElement != null)
            {
                nsmgr.AddNamespace("rsl", xmlDocument.DocumentElement.NamespaceURI);

                foreach (var xpath in paramsMap.Keys)
                {
                    var node = (XmlElement)xmlDocument.SelectSingleNode(xpath, nsmgr);
                    if (node != null)
                    {
                        paramValues.Add(paramsMap[xpath], node.InnerText.Trim());
                    }

                }
            }

            return paramValues;
        }

        private Dictionary<string, string> loadRSLToParamMap()
        {
            //TODO: For the time being I'm filling this map manually. But change this to load from a file
            // as we did in the java version.
            Dictionary<string, string> paramsMap = new Dictionary<string, string>();
            paramsMap.Add("/rsl:job/rsl:directory", "initialdir");
            paramsMap.Add("/rsl:job/rsl:stdout", "output");
            paramsMap.Add("/rsl:job/rsl:stdin", "input");
            paramsMap.Add("/rsl:job/rsl:stderr", "error");
            paramsMap.Add("/rsl:job/rsl:maxTime", "maxTime");
            paramsMap.Add("/rsl:job/rsl:maxWallTime", "wall_clock_limit");
            paramsMap.Add("/rsl:job/rsl:maxCpuTime", "maxCpuTime");
            paramsMap.Add("/rsl:job/rsl:project", "account_no");
            paramsMap.Add("/rsl:job/rsl:queue", "class");
            paramsMap.Add("/rsl:job/rsl:count", "count");
            paramsMap.Add("/rsl:job/rsl:hostCount", "node");
            //paramsMap.Add("/rsl:job/rsl:environment[name/text()='inputData']/value" , "inputData");
            //paramsMap.Add("/rsl:job/rsl:environment[name/text()='outputData']/value" , "outputData");
            paramsMap.Add("/rsl:job/rsl:executable", "executable");
            paramsMap.Add("/rsl:job/rsl:jobType", "jobType");

            return paramsMap;
        }

        private Dictionary<string, string> loadJSDLToParamMap()
        {
            throw new NotImplementedException();
        }
    }
}
