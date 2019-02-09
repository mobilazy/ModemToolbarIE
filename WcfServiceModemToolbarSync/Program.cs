using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceModemToolbarSync
{
    static class Program
    {
        static void Main()
        {

#if DEBUG
            ModemWindowsService service = new ModemWindowsService();
            service.OnDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

#else
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[]
            {
                new ModemWindowsService()
            };

            ServiceBase.Run(servicesToRun);

#endif
        }
    }
}
