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
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

namespace Service_Gateway_SMS
{
	public class Service_Gateway_SMS : ServiceBase
	{
		public const string MyServiceName = "Service_Gateway_SMS";
		private TCPServer server=null;
		
		public Service_Gateway_SMS()
		{
			InitializeComponent();
		}
		
		private void InitializeComponent()
		{
			this.ServiceName = MyServiceName;
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			// TODO: Add cleanup code here (if required)
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// Start this service.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			// TODO: Add start code here (if required) to start your service.
			conectar();
		}
		
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			// TODO: Add tear-down code here (if required) to stop your service.
			/*Si le hago el stop y no esta creado. Se queda ahi*/

			TCPServer.logger.logData("Cerrando Servicio...");
			/*Cierro el serial*/
			try{
				if (server != null && server.isServerRunning()){
					server.StopServer();
				}
				
			}
			catch (Exception ex)
			{
				TCPServer.logger.logData("EXEPCION: "+ex);
			}
			finally
			{
				if(TCPServer.serialPort.IsOpen){
					TCPServer.serialPort.Close();
				}
			}
		}
		
		void conectar()
		{
			int i = 0;
			bool ok = false;
			
			try{
				TCPServer.serialPort.PortName=Settings1.Default.puertoCOM;
				while (i < 3 && !ok)
				{

					//restartUSBController();
					
					if(!TCPServer.serialPort.IsOpen){
						TCPServer.serialPort.Open();
					}
					
					if(TCPServer.serialPort.IsOpen){
						TCPServer.logger.logData("CONECTADO AL PUERTO "+TCPServer.serialPort.PortName);
						if (TCPServer.gsm_module.connectSIM900() && TCPServer.gsm_module.setSignal() && TCPServer.gsm_module.prepareSMS())
						{
							ok=true;
							TCPServer.logger.logData("CONECTADO AL MODULO GSM");
						}
						else
						{
							TCPServer.logger.logData("ERROR : No se pudo conectar al módulo SIM");
						}	
					}
					else
					{
						TCPServer.logger.logData("ERROR : No se pudo conectar al puerto COM");
					}
					
					
					i++;
					System.Threading.Thread.Sleep(1000);
				}
							
			}
			catch(Exception e)
			{
				TCPServer.logger.logData("EXEPCION: "+e);
			}
			
			
			if (ok){
				try{
					// Create the Server Object ans Start it.
					server = new TCPServer();
					server.StartServer();
					TCPServer.logger.logData("Servidor Iniciado");
				}
				catch(Exception e2){
					TCPServer.logger.logData("ERROR : Error de creacion del Server");
					TCPServer.logger.logData(e2.ToString());
					
				}

				TCPServer.sendErrorEmail(3);
				
			}
			else
			{

				TCPServer.sendErrorEmail(2);
				TCPServer.logger.logData("ERROR : No se pudo conectar con el modulo GSM");
			}
		}
		
		void restartUSBController()
		{
			//Create process
			System.Diagnostics.Process pProcess = new System.Diagnostics.Process();

			//strCommand is path and file name of command to run
			pProcess.StartInfo.FileName = @AppDomain.CurrentDomain.BaseDirectory.ToString()+"\\devcon.exe";

			//Execute comand "restart *deviceID*"
			pProcess.StartInfo.Arguments = " restart *"+Settings1.Default.deviceID+"*";

			pProcess.StartInfo.UseShellExecute = false;

			//Set output of program to be written to process output stream
			pProcess.StartInfo.RedirectStandardOutput = true;

			//Start the process
			pProcess.Start();

			//Get program output
			//string strOutput = pProcess.StandardOutput.ReadToEnd();

			//Wait for process to finish
			pProcess.WaitForExit();
			
			
			

		}
	}
}
