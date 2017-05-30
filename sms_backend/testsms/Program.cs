﻿/*
 * Created by SharpDevelop.
 * User: User
 * Date: 11.05.2017
 * Time: 8:07
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
using System.Globalization;



namespace testsms
{
	class Program
	{
		static SerialPort port1,port2;
        
		struct Settings
		{
		public string ServiceComPort;
		public string AppComPort;
		}
					
        static void Main(string[] args)
        {       
        	Settings Settings;  
        
        	using (var managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                string[] portNames = SerialPort.GetPortNames();
                IEnumerable<string> ports = managementObjectSearcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());
                 
                Settings.ServiceComPort = portNames.FirstOrDefault(x => ports.Any(p => p.Contains(x) && p.Contains("3G PC UI Interface")));
                Settings.AppComPort = portNames.FirstOrDefault(x => ports.Any(p => p.Contains(x) && p.Contains("3G Application Interface")));
            }
       	
        	string comport1=Settings.ServiceComPort;
            string comport2=Settings.ServiceComPort;
            
        	string phone="+79538000300";
        	string smstext="text";        	     	

        	if(args.Length==2)
        	{
        		comport1=args[0];
        		comport2=args[1];
         	}
        	
        	if(args.Length==4)
        	{
        		comport1=args[0];
        		comport2=args[1];
        		phone=args[2];
        		smstext=args[3];
        	}
        	
        	Console.WriteLine("phone=" + phone);
        	Console.WriteLine("smstext=" + smstext);
            Console.WriteLine("comport1=" + comport1);
            Console.WriteLine("comport2=" + comport2);
            
            port1 = new SerialPort();
            OpenPort(port1,comport1);

            if(args.Length==4)
        	{
            	bool result=false;
	            Console.WriteLine("Sending sms");
    	        result = sendSMS(smstext, phone); 

    	        if (result == true)
    	        {
    	            Console.WriteLine("Successfully sended");
    	        }
    	        else
    	        {
                Console.WriteLine("Send error");
    	        }
    	        
    	        Console.ReadLine();
            }
            else
            {
            	port2 = new SerialPort();
            	OpenPort(port2, comport2);
            	
            	string recievedData;
            	System.Threading.Thread.Sleep(500);
                recievedData = port1.ReadExisting();
				//port.WriteLine("ATI\r\n");
				System.Threading.Thread.Sleep(500);
				
				//AA180C3602 = *100#
				
				port1.WriteLine("AT+CUSD=1,\"AA180C3602\",15\r\n");
				System.Threading.Thread.Sleep(500);
				recievedData = port1.ReadExisting();
				System.Threading.Thread.Sleep(500);
				string recievedData2;
				recievedData2 = port2.ReadExisting();
				string ussd="";
				
				try
				{
					int pos=recievedData2.IndexOf("+CUSD:")+10;
					ussd=recievedData2.Substring(pos,recievedData2.Length-pos);
					ussd=ussd.Substring(0,ussd.IndexOf("\""));
					ussd=UCS2ToString(ussd);
				}
				catch
				{
					Console.WriteLine("error decode ussd");
				}
            	
				
				Console.WriteLine("ussd=" + ussd);
					
				//UCS2ToString
								
				Console.WriteLine(recievedData2);
                Console.WriteLine(recievedData);
                port2.Close();
                Console.ReadLine();
            }
            
            port1.Close();
        }


        private static bool sendSMS(string textsms, string telnumber)
        {
            if (!port1.IsOpen) return false;

            try
            {
                System.Threading.Thread.Sleep(500);
                port1.WriteLine("AT\r\n"); // означает "Внимание!" для модема 
                System.Threading.Thread.Sleep(500);

                port1.Write("AT+CMGF=0\r\n"); // устанавливается цифровой режим PDU для отправки сообщений
                System.Threading.Thread.Sleep(500);
                
            }
            catch
            {
                return false;
            }

            try
            {
                telnumber = telnumber.Replace("-", "").Replace(" ", "").Replace("+", "");

                // 01 это PDU Type или иногда называется SMS-SUBMIT. 01 означает, что сообщение передаваемое, а не получаемое 
                // цифры 00 это TP-Message-Reference означают, что телефон/модем может установить количество успешных сообщений автоматически
                // telnumber.Length.ToString("X2") выдаст нам длинну номера в 16-ричном формате
                // 91 означает, что используется международный формат номера телефона
                telnumber = "01" + "00" + telnumber.Length.ToString("X2") + "91" + EncodePhoneNumber(telnumber);

                textsms = StringToUCS2(textsms);
                // 00 означает, что формат сообщения неявный. Это идентификатор протокола. Другие варианты телекс, телефакс, голосовое сообщение и т.п.
                // 08 означает формат UCS2 - 2 байта на символ. Он проще, так что рассмотрим его.
                // если вместо 08 указать 18, то сообщение не будет сохранено на телефоне. Получится flash сообщение
                string leninByte = (textsms.Length / 2).ToString("X2");
                textsms = telnumber + "00" + "08" + leninByte + textsms;

                // посылаем команду с длинной сообщения - количество октет в десятичной системе. то есть делим на два количество символов в сообщении
                // если октет неполный, то получится в результате дробное число. это дробное число округляем до большего
                double lenMes = textsms.Length / 2;
                port1.Write("AT+CMGS=" + (Math.Ceiling(lenMes)).ToString() + "\r\n");
                System.Threading.Thread.Sleep(500);

                // номер sms-центра мы не указываем, считая, что практически во всех SIM картах он уже прописан
                // для того, чтобы было понятно, что этот номер мы не указали добавляем к нашему сообщению в начало 2 нуля
                // добавляем именно ПОСЛЕ того, как подсчитали длинну сообщения
                textsms = "00" + textsms;

                port1.Write(textsms + char.ConvertFromUtf32(26) + "\r\n");
                System.Threading.Thread.Sleep(500);
            }
            catch
            {
                return false;
            }

            try
            {
                string recievedData;
                recievedData = port1.ReadExisting();

                if (recievedData.Contains("ERROR"))
                {
                    return false;
                }

            }
            catch { }

            return true;
        }




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

        // перекодирование номера телефона для формата PDU
        public static string EncodePhoneNumber(string PhoneNumber)
        {
            string result = "";
            if ((PhoneNumber.Length % 2) > 0) PhoneNumber += "F";

            int i = 0;
            while (i < PhoneNumber.Length)
            {
                result += PhoneNumber[i + 1].ToString() + PhoneNumber[i].ToString();
                i += 2;
            }
            return result.Trim();
        }


        // перекодирование текста смс в UCS2 
        public static string StringToUCS2(string str)
        {
            UnicodeEncoding ue = new UnicodeEncoding();
            byte[] ucs2 = ue.GetBytes(str);

            int i = 0;
            while (i < ucs2.Length)
            {
                byte b = ucs2[i + 1];
                ucs2[i + 1] = ucs2[i];
                ucs2[i] = b;
                i += 2;
            }
            return BitConverter.ToString(ucs2).Replace("-", "");
        }
        
        
        public static string UCS2ToString(string inText)
        {
            string res = "";
            if ((inText.Length == 0) || ((inText.Length % 2) != 0))
            {
                return null;
            }
            int num = inText.Length / 2;
            byte[] buffer = new byte[num];
            for (int i = 0; i < num; i++)
            {
                buffer[i] = byte.Parse(inText.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            res = Encoding.BigEndianUnicode.GetString(buffer);
            return res;
        }
		
		
		
	}
}