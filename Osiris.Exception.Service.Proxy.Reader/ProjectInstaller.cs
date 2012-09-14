using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Inmeta.Exception.Service.Common;
using Inmeta.Exception.Service.Common.Sec;


namespace Inmeta.Exception.Service.Proxy.Reader
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            var serviceUrl = Path.Combine(Context.Parameters["serviceurl"], "ExceptionService", "Exceptions.svc");
            var domain = Context.Parameters["domain"];
            var targetDirectory = Context.Parameters["targetdir"];
            var usernamee = Context.Parameters["username"];
            var password = Context.Parameters["password"];
            var mailserverto = Context.Parameters["mailserverto"];
            var mailserverhost = Context.Parameters["mailserverhost"];
            var mailserverlogin = Context.Parameters["mailserverlogin"];
            var mailserverpassword = Context.Parameters["mailserverpassword"];
            
            
            var exePath = Path.Combine(targetDirectory.Trim(), "Inmeta.Exception.Service.Proxy.Reader.exe");
      //      MessageBox.Show("service URL " + serviceUrl + ", Poll  " + pollIntervall + ", target = " +
        //                    targetDirectory + " exepath " + exePath + "Username  = "+ usernamee + ", password = " + password);

            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(exePath);

                config.AppSettings.Settings["serviceURL"].Value = serviceUrl;
                config.AppSettings.Settings["domain"].Value = new Encryption().Encrypt(domain);
                config.AppSettings.Settings["username"].Value = new Encryption().Encrypt(usernamee);
                config.AppSettings.Settings["password"].Value = new Encryption().Encrypt(password);
                config.AppSettings.Settings["mailServerTo"].Value = mailserverto;
                config.AppSettings.Settings["mailServerHost"].Value = mailserverhost;
                config.AppSettings.Settings["mailServerLogin"].Value = new Encryption().Encrypt(mailserverlogin);
                config.AppSettings.Settings["mailServerPassword"].Value = new Encryption().Encrypt(mailserverpassword);   
                config.Save();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }

            try 
            {
                // overwrite TFS Server settings
                var applicationFile = Path.Combine(targetDirectory.Trim(), "Applications.xml");
                File.Delete(applicationFile);
                
                var applicationFileKM = Path.Combine(targetDirectory.Trim(), "Applications_KM.xml");
                File.Move(applicationFileKM, applicationFile);
            } 
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }

        }
    }
}
