using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WcfServiceModemToolbarSync
{
   

    class ModemWindowsService:ServiceBase   
    {
        public ServiceHost serviceHost = null;
        //WCFModemService wcfModemService;
        

        public ModemWindowsService()
        {
            ServiceName = "ModemWindowsService";
            
        }

        

        //for debugging purposes
        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            //base.OnStart(args);

            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            //create service host for modem wcf service
            serviceHost = new ServiceHost(typeof(WCFModemService));
            serviceHost.Open();
            

           

        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }

        private void InitializeComponent()
        {
            // 
            // ModemWindowsService
            // 
            this.ServiceName = "ModemWindowsService";

        }
    }
}
