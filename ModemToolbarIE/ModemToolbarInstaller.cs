using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ModemToolbarIE
{
    [RunInstaller(true)]
    public partial class ModemToolbarInstaller : Installer
    {
        public ModemToolbarInstaller()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Installation
        /// </summary>
        
        public override void Install(System.Collections.IDictionary stateSaver)
        {

            base.Install(stateSaver);
            //RegAsm("");
            Toolbar.InstallationDate = DateTime.Now;

            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.RegisterAssembly(GetType().Assembly,
            AssemblyRegistrationFlags.None))
            {
                throw new InstallException("Failed To Register for COM");
            }


        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);    
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);  
        }

        /// <summary>
        /// Deinstallation
        /// </summary>
        /// <param name="savedState"></param>

        public override void Uninstall(System.Collections.IDictionary savedState)
        {

            //RegAsm("/u");

            Assembly asm = Assembly.GetExecutingAssembly();
            string fullName = asm.GetModules()[0].FullyQualifiedName;
            string dataFolder = Toolbar.DataFolder;


            try
            {
                Directory.Delete(dataFolder, true);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show (ex.Message);
            }

            try
            {
                Registry.LocalMachine.DeleteSubKeyTree(Toolbar.AppKey);
                Registry.CurrentUser.DeleteSubKeyTree(Toolbar.AppKey);
            }
            catch (Exception)
            {
            }

            base.Uninstall(savedState);
            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.UnregisterAssembly(GetType().Assembly))
            {
                throw new InstallException("Failed To Unregister for COM");
            }
        }

        private static void RegAsm(string parameters)
        {
            // RuntimeEnvironment.GetRuntimeDirectory() returns something like
            //    C:\Windows\Microsoft.NET\Framework64\v2.0.50727\
            // We need to get to the
            //    C:\Windows\Microsoft.NET
            // part in order to create 32 and 64 bit paths
            var net_base = Path.GetFullPath(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), @"..\.."));

            // Create paths to 32bit and 64bit versions of regasm.exe
            var to_exec = new[]
            {
            string.Concat(net_base, "\\Framework\\", RuntimeEnvironment.GetSystemVersion(), "\\regasm.exe"),
            string.Concat(net_base, "\\Framework64\\", RuntimeEnvironment.GetSystemVersion(), "\\regasm.exe")
        };

            var dll_path = Assembly.GetExecutingAssembly().Location; //ModemToolbarIE.Toolbar.DataFolder + "\\ModemToolbarIE.dll"; //
            MessageBox.Show(dll_path);
            foreach (var path in to_exec)
            {
                // Skip the executables that do not exist
                // This most likely happens on a 32bit machine when processing the path to 64bit regasm
                if (!File.Exists(path))
                {
                    continue;
                }
                //string arg = string.Format("\"{0}\" {1} \"{2}\"", path, parameters, dll_path);
                //MessageBox.Show(arg);

                var process = new Process
                {
                    StartInfo =
                {
                    CreateNoWindow = false,
                    ErrorDialog = false,
                    UseShellExecute = false,
                    //WorkingDirectory = Path.GetDirectoryName(path),
                    FileName = path,
                    Verb = "runas",
                    Arguments = string.Format(" \"\"\"{0}\"\"\" \"\"\"{1}\"\"\" ", parameters , dll_path  )
                }
                };

                using (process)
                {

                    process.Start();
                    process.WaitForExit(2000);
                }
            }
        }
    }
}
