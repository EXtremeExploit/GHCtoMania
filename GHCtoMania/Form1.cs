using SharpDX.DirectInput;
using System;
using System.IO;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using Z.Expressions;

namespace GHCtoMania
{
	public partial class Form1 : Form
	{
		DirectInput directInput = new DirectInput();
		Guid GuitarGuid = Guid.Empty;
		Joystick Guitar;
		InputSimulator InputSimulator = new InputSimulator();

		#region Guitar Status
		bool pressing_green = false;
		bool pressing_red = false;
		bool pressing_yellow = false;
		bool pressing_blue = false;
		bool pressing_orange = false;
		bool pressing_strum_green = false;
		bool pressing_strum_red = false;
		bool pressing_strum_yellow = false;
		bool pressing_strum_blue = false;
		bool pressing_strum_orange = false;
		#endregion

		#region GuitarCFG
		string HID;
		string GuitarName;
		string Green;
		string _GreenValue;
		string Red;
		string _RedValue;
		string Yellow;
		string _YellowValue;
		string Blue;
		string _BlueValue;
		string Orange;
		string _OrangeValue;
		string StrumDown;
		string _StrumDownValue;
		string StrumUp;
		string _StrumUpValue;
		#endregion

		#region Guitar Values
		object GreenValue;
		object RedValue;
		object YellowValue;
		object BlueValue;
		object OrangeValue;
		object StrumDownValue;
		object StrumUpValue;
		#endregion

		#region Guitar Keys
		VirtualKeyCode GreenKey;
		VirtualKeyCode RedKey;
		VirtualKeyCode YellowKey;
		VirtualKeyCode BlueKey;
		VirtualKeyCode OrangeKey;
		#endregion

		public Form1()
		{
			InitializeComponent();
		}


		public void FindGuitar()
		{
			string guitarsFile = "./Data/guitars.cfg";
			string[] guitarslines = File.ReadAllLines(guitarsFile);
			foreach (var deviceInstance in directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices))
			{
				GuitarGuid = deviceInstance.InstanceGuid;
			}
			if (GuitarGuid == Guid.Empty)
			{
				toolStripStatusLabel1.Text = "No Guitar Found.";
			}
			else
			{
				Guitar = new Joystick(directInput, GuitarGuid);
				GuitarGuid = Guid.Empty;
				Guitar.Acquire();
				for (var line = 0; line < guitarslines.Length; line++)
				{
					if (guitarslines[line].StartsWith("HID"))
					{
						string hid = guitarslines[line].Replace("HID ", "");
						string vid = hid.Split(' ')[0];
						string pid = hid.Split(' ')[1];

						string guitarIntP = Guitar.Properties.InterfacePath;
						string guitarHID = guitarIntP.Split('#')[1];
						string guitarVID = guitarHID.Split('&')[0].Replace("vid_", "");
						string guitarPID = guitarHID.Split('&')[1].Replace("pid_", "");

						if (guitarVID == vid && guitarPID == pid)
						{
							HID = guitarslines[line].Replace("HID ", "");
							GuitarName = guitarslines[line + 1];
							Green = guitarslines[line + 2];
							_GreenValue = guitarslines[line + 3];
							Red = guitarslines[line + 4];
							_RedValue = guitarslines[line + 5];
							Yellow = guitarslines[line + 6];
							_YellowValue = guitarslines[line + 7];
							Blue = guitarslines[line + 8];
							_BlueValue = guitarslines[line + 9];
							Orange = guitarslines[line + 10];
							_OrangeValue = guitarslines[line + 11];
							StrumDown = guitarslines[line + 12];
							_StrumDownValue = guitarslines[line + 13];
							StrumUp = guitarslines[line + 14];
							_StrumUpValue = guitarslines[line + 15];
							toolStripStatusLabel1.Text = Guitar.Information.ProductName;
						}
					}
				}
			}
		}

		public void ApplyValues()
		{
			GreenValue = _GreenValue.Compile<Func<object>>()();
			RedValue = _RedValue.Compile<Func<object>>()();
			YellowValue = _YellowValue.Compile<Func<object>>()();
			BlueValue = _BlueValue.Compile<Func<object>>()();
			OrangeValue = _OrangeValue.Compile<Func<object>>()();

			StrumDownValue = _StrumDownValue.Compile<Func<object>>()();
			StrumUpValue = _StrumUpValue.Compile<Func<object>>()();
		}

		public void ApplyConfig()
		{
			string ConfigPath = "./Data/config.cfg";
			string[] lines = File.ReadAllLines(ConfigPath);
			var done = false;
			for (var i = 0; i < lines.Length; i++)
			{
				if (lines[i].StartsWith("#") == false && done == false)
				{
					GreenKey = (VirtualKeyCode)Eval.Compile(lines[i])();
					RedKey = (VirtualKeyCode)Eval.Compile(lines[i + 1])();
					YellowKey = (VirtualKeyCode)Eval.Compile(lines[i + 2])();
					BlueKey = (VirtualKeyCode)Eval.Compile(lines[i + 3])();
					OrangeKey = (VirtualKeyCode)Eval.Compile(lines[i + 4])();

					GreenValue_l.Text = GreenKey.ToString();
					RedValue_l.Text = RedKey.ToString();
					YellowValue_l.Text = YellowKey.ToString();
					BlueValue_l.Text = BlueKey.ToString();
					OrangeValue_l.Text = OrangeKey.ToString();
					done = true;
				}
			}
		}

		private void Form1_Resize(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized)
			{
				Hide();
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			FindGuitar();
			ApplyValues();
			ApplyConfig();

			Timer timer = new Timer
			{
				Interval = 1
			};
			timer.Tick += Timer_Tick;
			timer.Start();

		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			JoystickState data;
			try
			{
				Guitar.Poll();
				data = Guitar.GetCurrentState();
			}
			catch
			{
				return;
			}

			var GreenState = Eval.Compile<Func<JoystickState, object>>(Green + ";", "data")(data);
			var RedState = Eval.Compile<Func<JoystickState, object>>(Red + ";", "data")(data);
			var YellowState = Eval.Compile<Func<JoystickState, object>>(Yellow + ";", "data")(data);
			var BlueState = Eval.Compile<Func<JoystickState, object>>(Blue + ";", "data")(data);
			var OrangeState = Eval.Compile<Func<JoystickState, object>>(Orange + ";", "data")(data);

			var StrumDownState = Eval.Compile<Func<JoystickState, object>>(StrumDown + ";", "data")(data);
			var StrumUpState = Eval.Compile<Func<JoystickState, object>>(StrumUp + ";", "data")(data);

			//Green
			if (GreenState.ToString() == GreenValue.ToString())
			{
				if (pressing_green == false)
				{
					InputSimulator.Keyboard.KeyDown(GreenKey);
					pressing_green = true;
				}
				else
				{
					if (StrumDownState.ToString() == StrumDownValue.ToString() || StrumUpState.ToString() == StrumUpValue.ToString())
					{
						if (pressing_strum_green == false)
						{
							InputSimulator.Keyboard.KeyUp(GreenKey);
							pressing_green = false;
							pressing_strum_green = true;
						}
					}
					else
					{
						pressing_strum_green = false;
					}
				}
			}
			else
			{
				if (pressing_green)
				{
					InputSimulator.Keyboard.KeyUp(GreenKey);
					pressing_green = false;
				}
			}

			//Red
			if (RedState.ToString() == RedValue.ToString())
			{
				if (pressing_red == false)
				{
					InputSimulator.Keyboard.KeyDown(RedKey);
					pressing_red = true;
				}
				else
				{
					if (StrumDownState.ToString() == StrumDownValue.ToString() || StrumUpState.ToString() == StrumUpValue.ToString())
					{
						if (pressing_strum_red == false)
						{
							InputSimulator.Keyboard.KeyUp(RedKey);
							pressing_red = false;
							pressing_strum_red = true;
						}
					}
					else
					{
						pressing_strum_red = false;
					}
				}
			}
			else
			{
				if (pressing_red)
				{
					InputSimulator.Keyboard.KeyUp(RedKey);
					pressing_red = false;
				}
			}

			//Yellow
			if (YellowState.ToString() == YellowValue.ToString())
			{
				if (pressing_yellow == false)
				{
					InputSimulator.Keyboard.KeyDown(YellowKey);
					pressing_yellow = true;
				}
				else
				{
					if (StrumDownState.ToString() == StrumDownValue.ToString() || StrumUpState.ToString() == StrumUpValue.ToString())
					{
						if (pressing_strum_yellow == false)
						{
							InputSimulator.Keyboard.KeyUp(YellowKey);
							pressing_yellow = false;
							pressing_strum_yellow = true;
						}
					}
					else
					{
						pressing_strum_yellow = false;
					}
				}
			}
			else
			{
				if (pressing_yellow)
				{
					InputSimulator.Keyboard.KeyUp(YellowKey);
					pressing_yellow = false;
				}
			}

			//Blue
			if (BlueState.ToString() == BlueValue.ToString())
			{
				if (pressing_blue == false)
				{
					InputSimulator.Keyboard.KeyDown(BlueKey);
					pressing_blue = true;
				}
				else
				{
					if (StrumDownState.ToString() == StrumDownValue.ToString() || StrumUpState.ToString() == StrumUpValue.ToString())
					{
						if (pressing_strum_blue == false)
						{
							InputSimulator.Keyboard.KeyUp(BlueKey);
							pressing_blue = false;
							pressing_strum_blue = true;
						}
					}
					else
					{
						pressing_strum_blue = false;
					}
				}
			}
			else
			{
				if (pressing_blue)
				{
					InputSimulator.Keyboard.KeyUp(BlueKey);
					pressing_blue = false;
				}
			}

			//Orange
			if (OrangeState.ToString() == OrangeValue.ToString())
			{
				if (pressing_orange == false)
				{
					InputSimulator.Keyboard.KeyDown(OrangeKey);
					pressing_orange = true;
				}
				else
				{
					if (StrumDownState.ToString() == StrumDownValue.ToString() || StrumUpState.ToString() == StrumUpValue.ToString())
					{
						if (pressing_strum_orange == false)
						{
							InputSimulator.Keyboard.KeyUp(OrangeKey);
							pressing_orange = false;
							pressing_strum_orange = true;
						}
					}
					else
					{
						pressing_strum_orange = false;
					}
				}
			}
			else
			{
				if (pressing_orange)
				{
					InputSimulator.Keyboard.KeyUp(OrangeKey);
					pressing_orange = false;
				}
			}

		}

		private void Find_Guitar_b_Click(object sender, EventArgs e)
		{
			FindGuitar();
			ApplyValues();
			ApplyConfig();
		}

		private void ReloadConfig_b_Click(object sender, EventArgs e)
		{
			ApplyConfig();
		}

		private void FindGuitarToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FindGuitar();
		}

		private void reloadConfigToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ApplyConfig();
		}

		private void showToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			Show();
			WindowState = FormWindowState.Normal;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void notifyIcon_Click(object sender, EventArgs e)
		{
			Show();
			WindowState = FormWindowState.Normal;
		}
	}
}
