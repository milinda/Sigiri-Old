using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SigiriAzureDaemon
{
    class SigiriApplicationConfiguration
    {
        public string ApplicationId { set; get; }

        public string ApplicationLocation { set; get; }

        // Executable can be in inner directory of the application root directory.
        // User should specify the correct working directory.
        public string WorkingDirectory { set; get; }

        // Name of the executable of application. Use to create the process.
        public string ExecutableName { set; get; }

        /**
         * Create SigiriApplicationConfiguration from executable-config.xml.
         */
        public static SigiriApplicationConfiguration CreateSigiriApplicationConfiguration(string applicationId, string appLocation, string appConfigFilePath)
        {
            var appConfig = new SigiriApplicationConfiguration { ApplicationId = applicationId, ApplicationLocation = appLocation };


            var appConfigDoc = new XmlDocument();
            appConfigDoc.Load(appConfigFilePath);

            // There should be only one WorkingDirectory element
            var workingDirectoryElement = appConfigDoc.GetElementsByTagName("WorkingDirectory").Item(0);
            if (workingDirectoryElement != null)
            {
                appConfig.WorkingDirectory = String.Format(@"{0}\{1}", appLocation, workingDirectoryElement.InnerText);
            }

            // There should be only one ExecutableName element
            var executableNameElement = appConfigDoc.GetElementsByTagName("ExecutableName").Item(0);
            if (executableNameElement != null)
            {
                appConfig.ExecutableName = executableNameElement.InnerText;
            }

            return appConfig;
        }
    }
}
