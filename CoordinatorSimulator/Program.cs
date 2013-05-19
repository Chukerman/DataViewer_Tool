using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Xml;
using System.Threading;

namespace CoordinatorSimulator
{
	enum State
	{
		Networking, Start
	}

	class Program
	{
		static State mode;
		static SerialPort port;

		static void Main(string[] args)
		{
			XmlDocument config = new XmlDocument();
			config.Load("Configuration.xml");
			string comName = config.GetElementsByTagName("COMName")[0].InnerText;
			int baudRate = Int32.Parse(config.GetElementsByTagName("BaudRate")[0].InnerText);
			port = new SerialPort(comName, baudRate);
			port.DataReceived += port_DataReceived;
			mode = State.Networking;
			port.Open();
			Console.WriteLine("Port is open");
			while (true)
			{
				Thread.Sleep(60000);
				if (mode == State.Start)
				{
					byte[] message = new byte[1];
					message[0] = 0x01;
					port.Write(message, 0, message.Length);
					Console.WriteLine("Send Request...");
				}
			}
		}

		private static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			byte[] message;
			switch (mode)
			{
				case State.Networking:
					port.DiscardInBuffer();
					message = new byte[1];
					message[0] = 0x00;
					port.Write(message, 0, message.Length);
					Console.WriteLine("Get ID, Change to Start mode");
					mode = State.Start;
					break;
				case State.Start:
					if (port.BytesToRead > 6)
					{
						Console.WriteLine("Error: ");
						byte[] recv = new byte[port.BytesToRead];
						port.Read(recv, 0, recv.Length);
						string data = "";
						foreach (byte item in recv)
						{
							data += item.ToString("X2");
						}
						Console.WriteLine(data);
					}
					else
					{
						byte[] receive = new byte[6];
						port.Read(receive, 0, 6);
						string id = receive[2].ToString("G");
						ushort amount = BitConverter.ToUInt16(receive.ToList<byte>().GetRange(4, 2).Reverse<byte>().ToArray(), 0);
						Console.WriteLine("{0} : {1}", id, amount);
					}
					break;
				default:
					break;
			}
		}

	}
}
