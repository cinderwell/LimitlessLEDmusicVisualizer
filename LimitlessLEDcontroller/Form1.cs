using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using NAudio;
using NAudio.CoreAudioApi;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //public bool lightsOn = true;
        public bool listening = false;
        public bool shifting = false;
        public bool invPulse = false;
        public bool invShifting = false;
        public bool autoThresh = false;

        public int currLevel = 0;
        public int currThresh = 0;
        public System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient("192.168.11.43", 50000);
        public int threshold = 10;

        public Form1()
        {
            InitializeComponent();

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            comboBox1.Items.AddRange(devices.ToArray());
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            udpClient.Close();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            turnOn();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            turnOff();
        }

        private void turnOff()
        {
            //Connect to LimitlessLED Wifi Bridge Receiver
            
            //lightsOn = false;
            //Send hex command 38 which is "Turn Group1 LED lights ON" yes it remembers the last brightness and color, each LED contains a memory chip.
            udpClient.Send(new byte[] {
		        0x22,
		        0x0,
		        0x55
	        }, 3);
            //ToDo: send as many different commands here as you like, just change the number above where you see &H38

            //Close Connection
            //udpClient.Close();
        }

        private void shiftUp()
        {
            //Connect to LimitlessLED Wifi Bridge Receiver

            //lightsOn = false;
            //Send hex command 38 which is "Turn Group1 LED lights ON" yes it remembers the last brightness and color, each LED contains a memory chip.
            udpClient.Send(new byte[] {
		        0x23,
		        0x0,
		        0x55
	        }, 3);
            //ToDo: send as many different commands here as you like, just change the number above where you see &H38

            //Close Connection
            //udpClient.Close();
        }

        private void shiftDown()
        {
            //Connect to LimitlessLED Wifi Bridge Receiver

            //lightsOn = false;
            //Send hex command 38 which is "Turn Group1 LED lights ON" yes it remembers the last brightness and color, each LED contains a memory chip.
            udpClient.Send(new byte[] {
		        0x24,
		        0x0,
		        0x55
	        }, 3);
            //ToDo: send as many different commands here as you like, just change the number above where you see &H38

            //Close Connection
            //udpClient.Close();
        }

        private void turnOn()
        {
            //Connect to LimitlessLED Wifi Bridge Receiver
            //System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient("192.168.11.43", 50000);
            //lightsOn = true;
            //Send hex command 38 which is "Turn Group1 LED lights ON" yes it remembers the last brightness and color, each LED contains a memory chip.
            udpClient.Send(new byte[] {
		        0x21,
		        0x0,
		        0x55
	        }, 3);
            //ToDo: send as many different commands here as you like, just change the number above where you see &H38

            //Close Connection
            //udpClient.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            /*
            MMDeviceEnumerator de = new MMDeviceEnumerator();
            MMDevice device = de.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            float volume = (float)device.AudioMeterInformation.MasterPeakValue * 100;
            progressBar1.Value = (int)volume;
            */
            if (comboBox1.SelectedItem != null)
            {

                var device = (MMDevice)comboBox1.SelectedItem;
                //device = device.AudioEndpointVolume.Channels;
                progressBar1.Value = (int)(Math.Round(device.AudioMeterInformation.MasterPeakValue * 100));
                label1.Text = (Math.Round(device.AudioMeterInformation.MasterPeakValue * 100)).ToString();
                currLevel = (int)(Math.Round(device.AudioMeterInformation.MasterPeakValue * 100));

                

                if(listening)
                    pulse((int)(Math.Round(device.AudioMeterInformation.MasterPeakValue * 100)));
                else if(shifting)
                    shift((int)(Math.Round(device.AudioMeterInformation.MasterPeakValue * 100)));
                else if (invPulse)
                    repulse((int)(Math.Round(device.AudioMeterInformation.MasterPeakValue * 100)));
                else if (invShifting)
                    reshift((int)(Math.Round(device.AudioMeterInformation.MasterPeakValue * 100)));

                if (autoThresh)
                {
                    currThresh = (int)(((float)currThresh + (float)currLevel) / 2);
                    threshold = currThresh;
                    label2.Text = threshold.ToString();
                }

            }


        }

        private void shift(int level)
        {
            if (level > threshold)
                shiftUp();
            else if (level < threshold)
                shiftDown();
        }

        private void reshift(int level)
        {
            if (level < threshold)
                shiftUp();
            else if (level > threshold)
                shiftDown();
        }

        private void pulse(int level)
        {
           
            if (level > threshold)
                turnOff();
            else
                turnOn();

        }

        private void repulse(int level)
        {

            if (level < threshold)
                turnOff();
            else
                turnOn();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = threshold.ToString();
        }



        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            threshold = (int)trackBar2.Value;
            label2.Text = threshold.ToString();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                listening = false;
                shifting = false;
                invShifting = false;
                invPulse = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                listening = true;
                shifting = false;
                invPulse = false;
                invShifting = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            shiftUp();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            shiftDown();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                turnOn();
                listening = false;
                shifting = true;
                invPulse = false;
                invShifting = false;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            changeColor((int)trackBar1.Value);
        }

        private void changeColor(int color)
        {
            Byte colorByte = System.Convert.ToByte(color);
            udpClient.Send(new byte[] {
		        0x20,
		        colorByte,
		        0x55
	        }, 3);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                listening = false;
                shifting = false;
                invPulse = true;
                invShifting = false;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                turnOn();
                listening = false;
                shifting = false;
                invPulse = false;
                invShifting = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                autoThresh = true;
                currThresh = currLevel;
            }
            else
            {
                autoThresh = false;
                threshold = (int)trackBar2.Value;
                label2.Text = threshold.ToString();
            }
        }

    }
}
