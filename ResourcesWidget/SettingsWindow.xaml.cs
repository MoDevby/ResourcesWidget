using ResourcesWidget.Properties;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ResourcesWidget
{

	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
		}

		#region Window
		public bool Taskbar_Click
		{
			get { return Settings.Default.TaskbarIcon; }
			set
			{
				Settings.Default.TaskbarIcon = value;
				Settings.Default.Save();
			}
		}
		public List<string> TaskBarOptions
		{
			get
			{
				return new List<string> { "Downlad Speed", "Upload Speed", "CPU Usage", "Ram Usage" };
			}
		}
		public string TaskbarSelectedOption
		{
			get { return Settings.Default.TaskbarTitle; }
			set
			{
				Settings.Default.TaskbarTitle = value;
				Settings.Default.Save();
			}
		}
		public double WindowOpacity
		{
			get { return (int)(Settings.Default.WindowOpacity * 100); }
			set
			{
				if (value > 100 || value < 0)
					return;
				Settings.Default.WindowOpacity = value / 100;
				Settings.Default.Save();
			}
		}
		public Color Window_Background_Color
		{
			get { return Settings.Default.WindowColor; }
			set
			{
				Settings.Default.WindowColor = value;
				Settings.Default.Save();
			}
		}
		#endregion

		#region Color Picker
		public Color Lables_ColorChanged
		{
			get { return Settings.Default.LableColor; }
			set
			{
				Settings.Default.LableColor = value;
				Settings.Default.Save();
			}
		}
		public Color IP_ColorChanged
		{
			get { return Settings.Default.IpColor; }
			set
			{
				Settings.Default.IpColor = value;
				Settings.Default.Save();
			}
		}
		public Color Status_ColorChanged
		{
			get { return Settings.Default.StatusColor; }
			set
			{
				Settings.Default.StatusColor = value;
				Settings.Default.Save();
			}
		}
		public Color Received_ColorChanged
		{
			get { return Settings.Default.ReceivedColor; }
			set
			{
				Settings.Default.ReceivedColor = value;
				Settings.Default.Save();
			}
		}
		public Color DSpeed_ColorChanged
		{
			get { return Settings.Default.DColor; }
			set
			{
				Settings.Default.DColor = value;
				Settings.Default.Save();
			}
		}
		public Color Sent_ColorChanged
		{
			get { return Settings.Default.SentColor; }
			set
			{
				Settings.Default.SentColor = value;
				Settings.Default.Save();
			}
		}
		public Color UpSpeed_ColorChanged
		{
			get { return Settings.Default.UColor; }
			set
			{
				Settings.Default.UColor = value;
				Settings.Default.Save();
			}
		}
		public Color CPU_ColorChanged
		{
			get { return Settings.Default.CpuColor; }
			set
			{
				Settings.Default.CpuColor = value;
				Settings.Default.Save();
			}
		}
		public Color Ram_ColorChanged
		{
			get { return Settings.Default.RamColor; }
			set
			{
				Settings.Default.RamColor = value;
				Settings.Default.Save();
			}
		}
		#endregion

		#region Visibility
		public bool InterfaceSelection_Click
		{
			get { return Settings.Default.InterfaceVisibility == (byte)Visibility.Visible; }
			set
			{
				Settings.Default.InterfaceVisibility = (byte)(value ? 0 : 2);
				Settings.Default.Save();
			}
		}
		public bool Lables_Click
		{
			get { return Settings.Default.LableVisibility == (byte)Visibility.Visible; }
			set
			{
				Settings.Default.LableVisibility = (byte)(value ? 0 : 2);
				Settings.Default.Save();
			}
		}
		public bool IP_Click
		{
			get { return Settings.Default.IPVisibility == (byte)Visibility.Visible; }
			set
			{
				Settings.Default.IPVisibility = (byte)(value ? 0 : 2);
				Settings.Default.Save();
			}
		}
		public bool Status_Click
		{
			get { return Settings.Default.StatusVisibility == (byte)Visibility.Visible; }
			set
			{
				Settings.Default.StatusVisibility = (byte)(value ? 0 : 2);
				Settings.Default.Save();
			}
		}
		public bool Received_Click
		{
			get { return Settings.Default.ReceivedVisibility == (byte)Visibility.Visible; }
			set
			{
				Settings.Default.ReceivedVisibility = (byte)(value ? 0 : 2);
				Settings.Default.Save();
			}
		}
		public bool DSpeed_Click
		{
			get { return Settings.Default.DSpeedVisibility == (byte)Visibility.Visible; }
			set
			{
				Settings.Default.DSpeedVisibility = (byte)(value ? 0 : 2);
				Settings.Default.Save();
			}
		}
		public bool Sent_Click
		{
			get { return Settings.Default.SentVisibility == (byte)Visibility.Visible; }
			set
			{
				Settings.Default.SentVisibility = (byte)(value ? 0 : 2);
				Settings.Default.Save();
			}
		}
		public bool UpSpeed_Click
		{
			get { return Settings.Default.USpeedVisibility == (byte)Visibility.Visible; }
			set
			{
				Settings.Default.USpeedVisibility = (byte)(value ? 0 : 2);
				Settings.Default.Save();
			}
		}
		public bool CPU_Click
		{
			get { return Settings.Default.CpuVisibility == (byte)Visibility.Visible; }
			set
			{
				Settings.Default.CpuVisibility = (byte)(value ? 0 : 2);
				Settings.Default.Save();
			}
		}
		public bool Ram_Click
		{
			get { return Settings.Default.RamVisibility == (byte)Visibility.Visible; }
			set
			{
				Settings.Default.RamVisibility = (byte)(value ? 0 : 2);
				Settings.Default.Save();
			}
		}

		#endregion

		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			Taskbar_Click = true;
			TaskbarSelectedOption = "CPU Usage";
			WindowOpacity = 50;

			Window_Background_Color = Colors.White;
			Lables_ColorChanged = Colors.Black;
			IP_ColorChanged = Colors.Black;
			Status_ColorChanged = Colors.Black;
			Received_ColorChanged = Colors.ForestGreen;
			DSpeed_ColorChanged = Colors.Green;
			Sent_ColorChanged = Colors.IndianRed;
			UpSpeed_ColorChanged = Colors.Red;
			CPU_ColorChanged = Colors.Blue;
			Ram_ColorChanged = Colors.Blue;

			InterfaceSelection_Click = false;
			Lables_Click = true;
			IP_Click = false;
			Status_Click = false;
			Received_Click = true;
			DSpeed_Click = true;
			Sent_Click = true;
			UpSpeed_Click = true;
			CPU_Click = true;
			Ram_Click = true;
			DoneButton_Click(null, null);
		}
		private void DoneButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
