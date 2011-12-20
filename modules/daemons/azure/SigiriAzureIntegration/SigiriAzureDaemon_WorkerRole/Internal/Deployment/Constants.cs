using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal.Deployment
{
    class Constants
    {
        public const string ServiceManagementNS = "http://schemas.microsoft.com/windowsazure";
        public const string OperationTrackingIdHeader = "x-ms-request-id";
        public const string VersionHeaderName = "x-ms-version";
        public const string VersionHeaderContent = "2009-10-01";
        public const string VersionHeaderContent20100401 = "2010-04-01";
        public const string VersionHeaderContent20101028 = "2010-10-28";
        public const string PrincipalHeader = "x-ms-principal-id";

        public const string WebMethodPost = "POST";
        public const string ContentTypeApplicationXML = "application/xml";
    }
}
