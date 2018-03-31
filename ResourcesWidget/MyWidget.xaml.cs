using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Net.NetworkInformation;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ResourcesWidget.Properties;
using System.Diagnostics;
using Microsoft.VisualBasic.Devices;

namespace ResourcesWidget
{
	public partial class MyWidget : Window, INotifyPropertyChanged
	{
		private NetworkInterface _selectedInterface;
		private List<NetworkInterface> _interfaces = new List<NetworkInterface>();
		private string _ip, _status, _type;
		private decimal _dSpeed, _uSpeed, _receivedBytes, _sentBytes, _ramUsage, _cpuUsage;
		System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

		#region Proprties
		private PerformanceCounter cpuCounter;
		private ComputerInfo computerInfo;
		private IPv4InterfaceStatistics interfaceData { get; set; }

		public List<NetworkInterface> Interfaces
		{
			get { return _interfaces; }
		}
		public NetworkInterface SelectedInterface
		{
			get { return _selectedInterface; }
			set
			{
				try
				{
					///Populating IPv4 address
					if (value != null && value.GetIPProperties().UnicastAddresses != null)
					{
						UnicastIPAddressInformationCollection ipInfo = value.GetIPProperties().UnicastAddresses;

						foreach (UnicastIPAddressInformation item in ipInfo)
						{
							if (item.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
							{
								Ip = item.Address.ToString();
								break;
							}
						}
					}

					NetworkBandwidthCalculator(value);
					dispatcherTimer.Start();
				}
				catch (Exception)
				{
					throw;
				}
				_selectedInterface = value;
				OnPropertyChanged();
			}
		}

		public string Ip
		{
			get { return _ip; }
			set { _ip = value; OnPropertyChanged(); }
		}
		public string Status
		{
			get { return _status; }
			set { _status = value; OnPropertyChanged(); }
		}

		public string Type
		{
			get { return _type; }
			set { _type = value; OnPropertyChanged(); }
		}
		public decimal ReceivedBytes
		{
			get { return _receivedBytes.GMKByte().Normalize(); }
			set { _receivedBytes = value; OnPropertyChanged(); OnPropertyChanged("ReceivedUnit"); }
		}
		public string ReceivedUnit { get { return _receivedBytes.GMKBUnit(); } }
		public decimal SentBytes
		{
			get { return _sentBytes.GMKByte().Normalize(); }
			set { _sentBytes = value; OnPropertyChanged(); OnPropertyChanged("SentUnit"); }
		}
		public string SentUnit { get { return _sentBytes.GMKBUnit(); } }
		public decimal DSpeed
		{
			get { return _dSpeed.GMKByte().Normalize(); }
			set { _dSpeed = value; OnPropertyChanged(); OnPropertyChanged("DSpeedUnit"); if (TaskbarIcon) OnPropertyChanged("TaskBarTitle"); }
		}
		public string DSpeedUnit { get { return _dSpeed.GMKBUnitPerSecond(); } }

		public decimal USpeed
		{
			get { return _uSpeed.GMKByte().Normalize(); }
			set { _uSpeed = value; OnPropertyChanged(); OnPropertyChanged("USpeedUnit"); if (TaskbarIcon) OnPropertyChanged("TaskBarTitle"); }
		}
		public string USpeedUnit { get { return _uSpeed.GMKBUnitPerSecond(); } }


		public decimal CpuUsage
		{
			get { return _cpuUsage.Normalize(); }
			set { _cpuUsage = value; OnPropertyChanged(); if (TaskbarIcon) OnPropertyChanged("TaskBarTitle"); }
		}
		public string CpuUnit { get { return "%"; } }

		public decimal RamUsage
		{
			get { return _ramUsage.GMKByte().Normalize(); }
			set { _ramUsage = value; OnPropertyChanged(); OnPropertyChanged("RamUnit"); if (TaskbarIcon) OnPropertyChanged("TaskBarTitle"); }
		}
		public string RamUnit { get { return _ramUsage.GMKBUnit(); } }

		#region Window Properties

		public double WindowOpacity
		{ get { return Settings.Default.WindowOpacity; } }
		public Color WindowColor
		{ get { return Settings.Default.WindowColor; } }

		public bool AlwaysOnTop
		{
			get { return Settings.Default.AlwaysOnTop; }
			set
			{
				Settings.Default.AlwaysOnTop = value;
				Settings.Default.Save();
				OnPropertyChanged();
			}
		}
		public bool TaskbarIcon { get { return Settings.Default.TaskbarIcon; } }
		public string TaskBarTitle
		{
			get
			{
				if (!TaskbarIcon) return "";
				switch (Settings.Default.TaskbarTitle)
				{
					case "Downlad Speed":
						return "DSpeed : " + DSpeed + " " + DSpeedUnit;
					case "Upload Speed":
						return "USpeed : " + USpeed + " " + USpeedUnit;
					case "CPU Usage":
						return "CPU : " + CpuUsage + " " + CpuUnit;
					case "Ram Usage":
						return "Ram : " + RamUsage + " " + RamUnit;
					default:
						return "";
				}
			}
		}

		#endregion

		#region Colors
		public Brush LableColor
		{
			get
			{
				return new SolidColorBrush(Settings.Default.LableColor);
			}
		}
		public Brush IpColor
		{
			get
			{
				return new SolidColorBrush(Settings.Default.IpColor);
			}
		}
		public Brush StatusColor
		{
			get
			{
				return new SolidColorBrush(Settings.Default.StatusColor);
			}
		}
		public Brush ReceivedColor
		{
			get
			{
				return new SolidColorBrush(Settings.Default.ReceivedColor);
			}
		}
		public Brush DColor
		{
			get
			{
				return new SolidColorBrush(Settings.Default.DColor);
			}
		}
		public Brush SentColor
		{
			get
			{
				return new SolidColorBrush(Settings.Default.SentColor);
			}
		}
		public Brush UColor
		{
			get { return new SolidColorBrush(Settings.Default.UColor); }
		}


		public Brush CpuColor
		{
			get
			{
				return new SolidColorBrush(Settings.Default.CpuColor);
			}
		}
		public Brush RamColor
		{
			get
			{
				return new SolidColorBrush(Settings.Default.RamColor);
			}
		}

		#endregion

		#region Visibility
		public Visibility InterfaceVisibility { get { return (Visibility)Settings.Default.InterfaceVisibility; } }
		public Visibility LableVisiblity { get { return (Visibility)Settings.Default.LableVisibility; } }
		public Visibility IpLableVisiblity { get { return LableVisiblity == Visibility.Visible && IpVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed; } }
		public Visibility StatusLableVisiblity { get { return LableVisiblity == Visibility.Visible && StatusVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed; } }
		public Visibility ReceivedLableVisiblity { get { return LableVisiblity == Visibility.Visible && ReceivedVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed; } }
		public Visibility DSpeedLableVisiblity { get { return LableVisiblity == Visibility.Visible && DSpeedVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed; } }
		public Visibility SentLableVisiblity { get { return LableVisiblity == Visibility.Visible && SentVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed; } }
		public Visibility USpeedLableVisiblity { get { return LableVisiblity == Visibility.Visible && USpeedVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed; } }
		public Visibility CPULabelVisibility { get { return LableVisiblity == Visibility.Visible && CpuVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed; } }
		public Visibility RamLableVisiblity { get { return LableVisiblity == Visibility.Visible && RamVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed; } }

		public Visibility IpVisibility { get { return (Visibility)Settings.Default.IPVisibility; } }
		public Visibility StatusVisibility { get { return (Visibility)Settings.Default.StatusVisibility; } }
		public Visibility ReceivedVisibility { get { return (Visibility)Settings.Default.ReceivedVisibility; } }
		public Visibility DSpeedVisibility { get { return (Visibility)Settings.Default.DSpeedVisibility; } }
		public Visibility SentVisibility { get { return (Visibility)Settings.Default.SentVisibility; } }
		public Visibility USpeedVisibility { get { return (Visibility)Settings.Default.USpeedVisibility; } }
		public Visibility CpuVisibility { get { return (Visibility)Settings.Default.CpuVisibility; } }
		public Visibility RamVisibility { get { return (Visibility)Settings.Default.RamVisibility; } }

		#endregion

		#endregion

		#region Methods
		public MyWidget()
		{
			InitializeNetworkMeter();

			/// Set the timer
			dispatcherTimer.Tick += DispatcherTimer_Tick;
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

			InitializeComponent();
			InitializeCpuRam();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
			this.Top = 0;
		}

		private void InitializeNetworkMeter()
		{
			/// Detecting Network Adaptors Using 
			List<NetworkInterface> nics = NetworkInterface.GetAllNetworkInterfaces().Where(network =>
			  network.OperationalStatus == OperationalStatus.Up &&
			  (network.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
			   network.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)).ToList();
			/// If none is active get all of them :)
			if (nics.Count == 0)
				nics = NetworkInterface.GetAllNetworkInterfaces().Where(network =>
			  (network.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
			   network.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)).ToList();

			/// Add Items To the combo item source
			nics.ForEach(i => Interfaces.Add(i));
			//Set the selected interface to the first one
			if (Interfaces.Count > 0)
				SelectedInterface = Interfaces.First();
		}

		private void InitializeCpuRam()
		{
			//PrintPerformanceCounterParameters();
			cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
			computerInfo = new ComputerInfo();
		}

		private void NetworkBandwidthCalculator(NetworkInterface selecNic)
		{
			try
			{
				if (selecNic == null)
					return;
				//Setting General Information 
				Type = selecNic.NetworkInterfaceType.ToString();
				Status = selecNic.OperationalStatus.ToString();


				interfaceData = selecNic.GetIPv4Statistics();

				//todo: this may fail if the BytesReceived is more than 8 GB
				long bytesRecivedSpeedValue = (long)(interfaceData.BytesReceived - _receivedBytes);
				DSpeed = bytesRecivedSpeedValue;//Download speed
				ReceivedBytes = interfaceData.BytesReceived;

				long bytesSentSpeedValue = (long)(interfaceData.BytesSent - _sentBytes);
				USpeed = bytesSentSpeedValue;//Upload Speed
				SentBytes = interfaceData.BytesSent;


				//if (ipInfo != null)
				//{
				//	TimeSpan addressLifeTime = TimeSpan.FromSeconds(ipInfo.AddressValidLifetime);
				//	Lifetime = string.Format("{0:D2}h:{1:D2}m",
				//						   addressLifeTime.Hours,
				//						   addressLifeTime.Minutes);
				//}
			}
			catch (Exception)
			{
				throw;
			}

		}
		private void CpuRamCalc()
		{
			CpuUsage = (decimal)cpuCounter.NextValue();
			RamUsage = computerInfo.TotalPhysicalMemory - computerInfo.AvailablePhysicalMemory;
		}

		private void UpdateColorsAndVis()
		{
			OnPropertyChanged("WindowOpacity");
			OnPropertyChanged("WindowColor");
			OnPropertyChanged("TaskbarIcon");


			OnPropertyChanged("LableColor");
			OnPropertyChanged("IpColor");
			OnPropertyChanged("StatusColor");
			OnPropertyChanged("ReceivedColor");
			OnPropertyChanged("DColor");
			OnPropertyChanged("SentColor");
			OnPropertyChanged("UColor");
			OnPropertyChanged("CpuColor");
			OnPropertyChanged("RamColor");

			OnPropertyChanged("IpVisibility");
			OnPropertyChanged("StatusVisibility");
			OnPropertyChanged("ReceivedVisibility");
			OnPropertyChanged("DSpeedVisibility");
			OnPropertyChanged("SentVisibility");
			OnPropertyChanged("USpeedVisibility");
			OnPropertyChanged("CpuVisibility");
			OnPropertyChanged("RamVisibility");

			OnPropertyChanged("InterfaceVisibility");
			OnPropertyChanged("LableVisiblity");
			OnPropertyChanged("IpLableVisiblity");
			OnPropertyChanged("StatusLableVisiblity");
			OnPropertyChanged("ReceivedLableVisiblity");
			OnPropertyChanged("DSpeedLableVisiblity");
			OnPropertyChanged("SentLableVisiblity");
			OnPropertyChanged("USpeedLableVisiblity");
			OnPropertyChanged("CPULabelVisibility");
			OnPropertyChanged("RamLableVisiblity");
		}
		#endregion

		#region Events
		private void DispatcherTimer_Tick(object sender, EventArgs e)
		{
			NetworkBandwidthCalculator(SelectedInterface);
			CpuRamCalc();
		}
		private void Close_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			var settings = new SettingsWindow();
			settings.ShowDialog();
			UpdateColorsAndVis();
		}
		private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
				this.DragMove();
		}
		#endregion

		#region NotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

	}

	public static class Helper
	{
		public static decimal Normalize(this decimal value)
		{
			value = Math.Round(value, Settings.Default.Decimals);
			return value / 1.000000000000000000000000000000000m;
		}
		public static decimal GMKByte(this decimal value)
		{
			return value > 1073741824 ? value / 1073741824 : (value > 1048576 ? value / 1048576 : (value > 1024 ? value / 1024 : value));
		}
		public static string GMKBUnit(this decimal value)
		{
			return value > 1073741824 ? "GB" : (value > 1048576 ? "MB" : (value > 1024 ? "KB" : "Byte"));
		}
		public static string GMKBUnitPerSecond(this decimal value)
		{
			return value > 1073741824 ? "GB/S" : (value > 1048576 ? "MB/S" : (value > 1024 ? "KB/S" : "B/S"));
		}
	}
}
