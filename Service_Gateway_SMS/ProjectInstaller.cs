/*
 * Created by SharpDevelop.
 * User: SoporteSEM
 * Date: 17/08/2017
 * Time: 14:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Service_Gateway_SMS
{
	[RunInstaller(true)]
	public class ProjectInstaller : Installer
	{

		public ProjectInstaller()
		{
ServiceProcessInstaller serviceProcessInstaller = 
                               new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            //# Service Account Information
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            //# Service Information
            serviceInstaller.DisplayName = "My New C# Windows Service";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //# This must be identical to the WindowsService.ServiceBase name
            //# set in the constructor of WindowsService.cs
            serviceInstaller.ServiceName = "My Windows Service";

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller); 	 	
		}
	}
}
