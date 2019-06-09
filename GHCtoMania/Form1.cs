using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace GHCtoMania
{
	public partial class Form1 : Form
	{
		readonly DirectInput directInput = new DirectInput();
		Guid GuitarGuid = Guid.Empty;
		Joystick Guitar;
		readonly InputSimulator InputSimulator = new InputSimulator();

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
		object GreenState = null;
		object RedState = null;
		object YellowState = null;
		object BlueState = null;
		object OrangeState = null;
		object StrumDownState = null;
		object StrumUpState = null;
		#endregion

		#region GuitarCFG
		string HID;
		string GuitarName;
		string Green;
		string Red;
		string Yellow;
		string Blue;
		string Orange;
		string StrumDown;
		string StrumUp;
		#endregion

		#region Guitar Values
		object GreenValue;
		object GreenDefault;
		object RedValue;
		object RedDefault;
		object YellowValue;
		object YellowDefault;
		object BlueValue;
		object BlueDefault;
		object OrangeValue;
		object OrangeDefault;
		object StrumDownValue;
		object StrumDownDefault;
		object StrumUpValue;
		object StrumUpDefault;
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
			CheckForIllegalCrossThreadCalls = false;
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
				Guitar.Properties.BufferSize = 128;
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
							GuitarName = guitarslines[line++];
							Green = guitarslines[line++];
							GreenValue = guitarslines[line++];
							GreenDefault = guitarslines[line++];
							Red = guitarslines[line++];
							RedValue = guitarslines[line++];
							RedDefault = guitarslines[line++];
							Yellow = guitarslines[line++];
							YellowValue = guitarslines[line++];
							YellowDefault = guitarslines[line++];
							Blue = guitarslines[line++];
							BlueValue = guitarslines[line++];
							BlueDefault = guitarslines[line++];
							Orange = guitarslines[line++];
							OrangeValue = guitarslines[line++];
							OrangeDefault = guitarslines[line++];
							StrumDown = guitarslines[line++];
							StrumDownValue = guitarslines[line++];
							StrumDownDefault = guitarslines[line++];
							StrumUp = guitarslines[line++];
							StrumUpValue = guitarslines[line++];
							StrumUpDefault = guitarslines[line++];
							toolStripStatusLabel1.Text = GuitarName;
							GreenState = GreenDefault;
							RedState = RedDefault;
							YellowState = YellowDefault;
							BlueState = BlueDefault;
							OrangeState = OrangeDefault;
							StrumDownState = StrumDownDefault;
							StrumUpState = StrumUpDefault;
						}
					}
				}
			}
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
					GreenKey = (VirtualKeyCode)Convert.ToInt32(lines[i]);
					RedKey = (VirtualKeyCode)Convert.ToInt32(lines[i++]);
					YellowKey = (VirtualKeyCode)Convert.ToInt32(lines[i++]);
					BlueKey = (VirtualKeyCode)Convert.ToInt32(lines[i++]);
					OrangeKey = (VirtualKeyCode)Convert.ToInt32(lines[i++]);

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
				ShowInTaskbar = false;
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			List<string> envvars = new List<string>(Environment.GetCommandLineArgs());
			if (envvars.Contains("-autohide"))
			{
				WindowState = FormWindowState.Minimized;
			}
			if (envvars.Contains("-debug"))
			{
				Size = new System.Drawing.Size(480, 480);

				ApplyConfig();
				foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
					GuitarGuid = deviceInstance.InstanceGuid;

				if (GuitarGuid == Guid.Empty)
					foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
						GuitarGuid = deviceInstance.InstanceGuid;

				// If Joystick not found
				if (GuitarGuid == Guid.Empty)
				{
					Log.Text += "No joystick/Gamepad found.\n";
					toolStripStatusLabel1.Text = "(DEBUG MODE) No joystick/Gamepad found.";
				}
				else
				{
					var Guitar = new Joystick(directInput, GuitarGuid);
					Log.ScrollBars = ScrollBars.Vertical;
					Log.Text += $"Found Joystick/Gamepad with GUID: {GuitarGuid}{Environment.NewLine}";
					Log.Text += $"Name: {Guitar.Properties.ProductName}{Environment.NewLine}";
					string guitarIntP = Guitar.Properties.InterfacePath;
					string guitarHID = guitarIntP.Split('#')[1];
					string guitarVID = guitarHID.Split('&')[0].Replace("vid_", "");
					string guitarPID = guitarHID.Split('&')[1].Replace("pid_", "");
					Log.Text += $"VID / PID: {guitarVID} / {guitarPID}{Environment.NewLine}";

					// Query all suported ForceFeedback effects
					var allEffects = Guitar.GetEffects();
					foreach (var effectInfo in allEffects)
						Log.Text += $"Effect available {effectInfo.Name}{Environment.NewLine}";

					// Set BufferSize in order to use buffered data.
					Guitar.Properties.BufferSize = 128;

					// Acquire the joystick
					Guitar.Acquire();
					toolStripStatusLabel1.Text = "(DEBUG MODE) " + Guitar.Properties.ProductName;
					// Poll events from joystick
					Thread t = new Thread((x) =>
					{
						while (true)
						{
							Guitar.Poll();
							var datas = Guitar.GetBufferedData();
							foreach (var state in datas)
								Log.Text += state + Environment.NewLine;
						}
					});
					t.SetApartmentState(ApartmentState.MTA);
					t.Start();
				}
			}
			else
			{
				FindGuitar();
				ApplyConfig();
				if (HID != null)
				{
					Thread t = new Thread((x) =>
					{
						while (true)
						{
							Guitar.Poll();
							var datas = Guitar.GetBufferedData();
							foreach (var state in datas)
							{
								Console.WriteLine(state);
								if (state.Offset.ToString() == Green)
								{
									GreenState = state.Value;
								}
								if (state.Offset.ToString() == Red)
								{
									RedState = state.Value;
								}
								if (state.Offset.ToString() == Yellow)
								{
									YellowState = state.Value;
								}
								if (state.Offset.ToString() == Blue)
								{
									BlueState = state.Value;
								}
								if (state.Offset.ToString() == Orange)
								{
									OrangeState = state.Value;
								}
								if (state.Offset.ToString() == StrumDown)
								{
									StrumDownState = state.Value;
								}
								if (state.Offset.ToString() == StrumUp)
								{
									StrumUpState = state.Value;
								}

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
												Thread.Sleep(1);
												InputSimulator.Keyboard.KeyDown(GreenKey);
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
										GreenState = GreenDefault;
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
												Thread.Sleep(1);
												InputSimulator.Keyboard.KeyDown(RedKey);
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
										RedState = RedDefault;
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
												Thread.Sleep(1);
												InputSimulator.Keyboard.KeyDown(YellowKey);
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
										YellowState = YellowDefault;
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
												Thread.Sleep(1);
												InputSimulator.Keyboard.KeyDown(BlueKey);
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
										BlueState = BlueDefault;
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
												Thread.Sleep(1);
												InputSimulator.Keyboard.KeyDown(OrangeKey);
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
										OrangeState = OrangeDefault;
									}
								}
							}
						}
					});
					t.SetApartmentState(ApartmentState.MTA);
					t.Start();
				}
			}
		}
		private void Restart_b_Click(object sender, EventArgs e)
		{
			Log.Clear();
			Form1_Load(sender, e);
		}

		private void ShowToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			ShowInTaskbar = true;
			Show();
			WindowState = FormWindowState.Normal;
		}

		private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Show();
				WindowState = FormWindowState.Normal;
			}
			else if (e.Button == MouseButtons.Right)
			{
				contextMenuStrip1.Show();
			}
		}

		private void Log_TextChanged(object sender, EventArgs e)
		{
			Log.SelectionStart = Log.Text.Length;
			Log.ScrollToCaret();
		}
	}
}
