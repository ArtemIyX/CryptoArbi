using ArbiLib.Services.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugApp.Workers
{
    public class LocalAsyncWorker(LocalArbiService InService) : AsyncWorker()
    {
        public LocalArbiService ArbiService { get; private set; } = InService;
    }
}
