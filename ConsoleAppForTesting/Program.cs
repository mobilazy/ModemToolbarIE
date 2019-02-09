using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WcfServiceModemToolbarSync;

namespace ConsoleAppForTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            Console.WriteLine("Hello World!");

            //CreateToolbarItems();



            Console.ReadKey();

            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 


        }

        static void CreateToolbarItems()
        {
            
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                Console.WriteLine(stopwatch.ElapsedMilliseconds.ToString());
                
                //create channerl to ModemSyncWindowsService and retrieve database data
                NetNamedPipeBinding myBinding = new NetNamedPipeBinding();
                
                EndpointAddress endpoint = new EndpointAddress("net.pipe://localhost/ModemToolbarSyncService");
                
                using (ChannelFactory<IWCFModemService> channelFactory = new ChannelFactory<IWCFModemService>(myBinding, endpoint))
                {
                    
                    IWCFModemService proxy = null;
                    try
                    {
                        proxy = channelFactory.CreateChannel();

                        List<MenuListItemClass> mlic = new List<MenuListItemClass>();
                        List<SearchBoxItemClass> sbic = new List<SearchBoxItemClass>();
                        List<LinkListItemClass> llic = new List<LinkListItemClass>();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds.ToString());
                        //Console.WriteLine("Sync Table Count: " + proxy.GetSyncStatus().ToString());
                        Console.WriteLine(stopwatch.ElapsedMilliseconds.ToString());
                        //mlic = proxy.GetDataMenuListItemClass();
                        //sbic = proxy.GetDataSearchBoxItemClass();
                        //llic = proxy.GetDataLinkListItemClass();

                        Console.WriteLine(stopwatch.ElapsedMilliseconds.ToString());

                        ((ICommunicationObject)proxy).Close();
                        channelFactory.Close();

                    }
                    catch (Exception)
                    {
                        (proxy as ICommunicationObject)?.Abort();
                    }

                }




            }
            finally
            {

               

            }
        }
    }
}
