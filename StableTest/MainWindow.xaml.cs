using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace StableTest
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		SerialPort port = new SerialPort();

		public MainWindow()
		{
			InitializeComponent();
			PortName_ComboBox.ItemsSource = SerialPort.GetPortNames();
			PortName_ComboBox.SelectedIndex = 0;
			port.DataReceived += port_DataReceived;
			port.NewLine = "\n";
		}

		private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			string data_all = port.ReadLine();
			if (data_all != null && data_all != "")
			{
				DateTime loggingOn = DateTime.Now;
				string[] data_unit = data_all.Trim().Split(' ');
				foreach (string item in data_unit)
				{
					string[] keyValuePair = item.Split(':');
					Data data = new Data();
					data.NodeID = Int32.Parse(new string(keyValuePair[0].Reverse<char>().ToArray<char>()));
					data.Low = Int32.Parse(new string(keyValuePair[1].Reverse<char>().ToArray<char>()));
					data.LoggingOn = loggingOn;
					data.RawData = item;
					data.Save();
				}
			}
		}

		private void On_StartLoggingButton_Click(object sender, RoutedEventArgs e)
		{
			port.PortName = PortName_ComboBox.SelectedItem.ToString();
			port.BaudRate = 38400;
			try
			{
				port.Open();
				StartLogging_Button.Content = "Stop Logging";
				StartLogging_Button.Click -= On_StartLoggingButton_Click;
				StartLogging_Button.Click += On_StopLoggingButton_Click;
			}
			catch (Exception)
			{
				MessageBox.Show("Error!");
			}
		}

		private void On_StopLoggingButton_Click(object sender, RoutedEventArgs e)
		{
			port.Close();
			StartLogging_Button.Content = "Start Logging";
			StartLogging_Button.Click -= On_StopLoggingButton_Click;
			StartLogging_Button.Click += On_StartLoggingButton_Click;
		}

		private void On_TestButton_Click(object sender, RoutedEventArgs e)
		{
			if (port.IsOpen)
			{
				port.WriteLine("5:00003 ");
			}
			else
			{
				MessageBox.Show("Start Logging First!");
			}
		}

		private void On_GetLatestButton_Click(object sender, RoutedEventArgs e)
		{
			List<Data> data = Data.Get_All();
			if (data.Count != 0)
				MessageBox.Show(data[0].ToString());
			else
				MessageBox.Show("No Data!");
		}

		private void On_ClearDatabase_Click(object sender, RoutedEventArgs e)
		{
			DBHelper.UpdateDeleteCommand("Data_clear", CommandType.StoredProcedure);
			MessageBox.Show("Success!");
		}

		private void On_GetDelayedButton_Click(object sender, RoutedEventArgs e)
		{
			List<Data> data = Data.Get_All();
			if (data.Count == 0)
				MessageBox.Show("No Data!");
			else
			{
				List<TimeSpan> timeSpans = new List<TimeSpan>();
				for (int i = 0; i < data.Count - 1; i++)
					timeSpans.Add(data[i].LoggingOn - data[i + 1].LoggingOn);
				if (timeSpans.Count != 0)
				{
					TimeSpan acceptTimeSpan = timeSpans.Min<TimeSpan>().Add(new TimeSpan(0, 0, Int32.Parse(Mistake_TextBox.Text)));
					StringBuilder delayedStringBuilder = new StringBuilder();
					int delayCounter = 0;
					for (int i = 0; i < timeSpans.Count; i++)
					{
						if (timeSpans[i] > acceptTimeSpan)
						{
							delayedStringBuilder.AppendFormat("{0} == ({1})", data[i + 1].ToString(), timeSpans[i].ToString());
							delayedStringBuilder.AppendLine();
							delayCounter++;
						}
					}
					delayedStringBuilder.AppendLine((delayCounter * 1.0 / data.Count).ToString());
					Log_TextBox.Text = delayedStringBuilder.ToString();
				}
			}
		}

		private void On_TestWrongButton_Click(object sender, RoutedEventArgs e)
		{
			if (port.IsOpen)
			{
				port.WriteLine("1:0003");
			}
			else
			{
				MessageBox.Show("Start Logging First!");
			}
		}

		private void On_GetWrongButton_Click(object sender, RoutedEventArgs e)
		{
			List<Data> data = Data.Get_All();
			if (data.Count != 0)
			{
				List<Data> wrongData = new List<Data>();
				foreach (Data item in data)
				{
					if (item.Low != 30000)
						wrongData.Add(item);
				}
				StringBuilder logStringBuilder = new StringBuilder();
				foreach (Data item in wrongData)
				{
					logStringBuilder.AppendLine(item.ToString());
				}
				logStringBuilder.AppendLine((wrongData.Count * 1.0 / data.Count).ToString());
				Log_TextBox.Text = logStringBuilder.ToString();
			}
			else
			{
				MessageBox.Show("No Data");
			}
		}

		private void On_GetTimeSpanButton_Click(object sender, RoutedEventArgs e)
		{
			List<Data> data = Data.Get_All();
			if (data.Count != 0)
			{
				MessageBox.Show(string.Format("{0} : {1}", data.Count, data[0].LoggingOn - data[data.Count - 1].LoggingOn));
			}
			else
			{
				MessageBox.Show("No Data!");
			}
		}
	}
}
