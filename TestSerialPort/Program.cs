using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Xml;

namespace TestSerialPort
{
	class Program
	{
		static SerialPort port = new SerialPort();

		static void Main(string[] args)
		{
			XmlDocument config = new XmlDocument();
			config.Load("Configuration.xml");
			port.PortName = config.GetElementsByTagName("PortName")[0].InnerText;
			port.BaudRate = 38400;
			port.DataReceived += port_DataReceived;
			port.NewLine = "\n";
			port.Open();
			port.WriteLine("Port Open...");
			while (true) ;
		}

		private static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			Dictionary<int, int> concentration = new Dictionary<int, int>();
			string data_all = port.ReadLine();
			string[] data_unit = data_all.Trim().Split(' ');
			foreach (string item in data_unit)
			{
				string[] keyValuePair = item.Split(':');
				concentration.Add(Int32.Parse(new string(keyValuePair[0].Reverse<char>().ToArray<char>())), Int32.Parse(new string(keyValuePair[1].Reverse<char>().ToArray<char>())));
			}
			foreach (var key in concentration.Keys)
			{
				Console.WriteLine("{0} : {1}", key, concentration[key]);
			}
		}
	}
}
