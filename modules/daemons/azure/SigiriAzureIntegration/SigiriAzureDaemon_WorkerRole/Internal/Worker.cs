﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigiriAzureDaemon_WorkerRole.Internal
{
    interface IWorker
    {
        void OnStart();
        void OnStop();
        void Run();

    }
}
