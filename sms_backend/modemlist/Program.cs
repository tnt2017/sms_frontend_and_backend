/*
 * Created by SharpDevelop.
 * User: User
 * Date: 24.05.2017
 * Time: 8:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Management;
using Microsoft.Win32;
using System.Windows.Forms;

namespace modemlist
{
	class Program
	{
		
		private static void OpenPort(SerialPort port, string sport)
        {
            port.BaudRate = 2400; // еще варианты 4800, 9600, 28800 или 56000
            port.DataBits = 7; // еще варианты 8, 9

            port.StopBits = StopBits.One; // еще варианты StopBits.Two StopBits.None или StopBits.OnePointFive         
            port.Parity = Parity.Odd; // еще варианты Parity.Even Parity.Mark Parity.None или Parity.Space

            port.ReadTimeout = 500; // еще варианты 1000, 2500 или 5000 (больше уже не стоит)
            port.WriteTimeout = 500; // еще варианты 1000, 2500 или 5000 (больше уже не стоит)

            //port.Handshake = Handshake.RequestToSend;
            //port.DtrEnable = true;
            //port.RtsEnable = true;
            //port.NewLine = Environment.NewLine;

            port.Encoding = Encoding.GetEncoding("windows-1251");

            port.PortName = sport;

            // незамысловатая конструкция для открытия порта
            if (port.IsOpen)
                port.Close();
            try
            {
                port.Open();
            }
            catch { }

        }
		
		public static void Main(string[] args)
		{
			string param1="";
			int id=0;
			if(args.Length==1)
        	{
				param1=args[0];
         	}
			
			
			if(param1=="modems" || param1=="")
			{
				string query="Select * From Win32_POTSModem";
				using (var searcher = new ManagementObjectSearcher(query)) 
            	{
                //IEnumerable<string> ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());
 		        foreach (ManagementObject service in searcher.Get())
        	    {
             		string comport=service["AttachedTo"].ToString();
             		
             		SerialPort port = new SerialPort();
            		OpenPort(port, comport);
            		
            		port.WriteLine("AT+CIMI\r\n");
            		System.Threading.Thread.Sleep(50);
            		string imsi = port.ReadExisting();
            		//MessageBox.Show(imsi);
            		String[] lines = imsi.Split('\n');
            		//Console.WriteLine(lines[1]);
            		System.Threading.Thread.Sleep(50);
            		
            		port.WriteLine("ATI\r\n");
					System.Threading.Thread.Sleep(50);
							
					string recievedData = port.ReadExisting();
					String[] substrings = recievedData.Split('\n');

					System.Threading.Thread.Sleep(50);
					
					string model=substrings[2];
					model=model.Substring(7,model.Length-8);
					//MessageBox.Show(model);
										
					string revision=substrings[3];
					revision=revision.Substring(10,revision.Length-11);
										
					string imei=substrings[4];
					imei=imei.Substring(6,imei.Length-7);
					
					imsi=lines[1];
					imsi=imsi.Substring(0,imsi.Length-1);
					id++;
					//MessageBox.Show(imsi);					
					
					Console.Write("{\"id\":\"" + id + "\", \"comport\":\"" + service["AttachedTo"] + "\", ");
					Console.Write("\"modem\": \"" + service["Name"] + " (" + service["AttachedTo"] + ")\", ");
					Console.Write("\"imei\":\"" + imei + "\", \"imsi\":\"" + imsi + "\", ");
					Console.WriteLine("\"model\":\"" + model + "\", \"revision\":\"" + revision + "\" }");// +
					port.Close();
 		        }
				}
			}
			
			id=0;
			
			if(param1=="ui" || param1=="")
			{		
	       	using (var managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%'")) //
            {
                string[] portNames = SerialPort.GetPortNames();
                IEnumerable<string> ports = managementObjectSearcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());
                //Settings.ServiceComPort = portNames.FirstOrDefault(x => ports.Any(p => p.Contains(x) && p.Contains("3G PC UI Interface")));
                //Settings.AppComPort = portNames.FirstOrDefault(x => ports.Any(p => p.Contains(x) && p.Contains("3G Application Interface")));
                foreach (var element in portNames)
                {
  					string ServiceComPort = ports.FirstOrDefault(x => ports.Any(p => p.Contains(x) && p.Contains(element)));
  					//Console.WriteLine(ServiceComPort);
  					if(ServiceComPort!=null)
  					{
  						if(ServiceComPort.Contains("3G PC UI")) //
  						{
  						 //Console.WriteLine(element + ":" + ServiceComPort);
  						 id++;
  						 Console.WriteLine("{\"id\":\"" + id + "\", \"comport\":\"" + element + "\", \"modem\":\"" + ServiceComPort + "\"}");
  						}
  					}
                }
            }
			}
			
	       	Console.ReadLine();
		}
	}
}