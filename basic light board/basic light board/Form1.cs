using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace basic_light_board
{
    /// <summary>
    /// this form provides a basic 24 channel 1 to 1 patched XY cross fader.
    /// it allows for timed fades using a go button or manual time control using a crossfader.
    /// this currently does not interface with the Enttec USB pro because i havent gotten the thing yet
    /// if all goes well it should work just fine.
    /// </summary>
    public partial class Form1 : Form
    {
        public const int universeSize=512;

        /// <summary>
        /// LiveLevels contains one entry for every dimmer
        /// LiveLevels[0] contains the value for dimmer 1
        /// </summary>
        byte[] LiveLevels = new byte[universeSize];
        byte[] XLevels = new byte[universeSize];
        byte[] YLevels = new byte[universeSize];
        output m_outForm;
        Stopwatch m_timer;
        VComWrapper com;

        int iterations;
        int change;

        public Form1()
        {
            InitializeComponent();
            m_outForm = new output(48);
            m_outForm.Show();

            com = new VComWrapper();
            //com.dataReceived +=new EventHandler<DMXMessage>(com_dataReceived);
            com.SerialNumberReceived += new EventHandler<SerialNumberArgs>(com_SerialNumberReceived);
            com.WidgetParametersReceived += new EventHandler<WidgetParameterArgs>(com_WidgetParametersReceived);
            com.initPro("COM4");
            com.sendGetWidgetParametersRequest(508);           
        }

        void com_WidgetParametersReceived(object sender, WidgetParameterArgs e)
        {
            MessageBox.Show(string.Format("Firmware: {0}\nBreakTime: {1}\nMarkTime:{2}\nOutRate: {3}\nUserConfig:{4}",
                e.Firmware, e.DMXOutBreakTime, e.DMXOutMarkTime, e.DMXOutRate, e.UserConfigData));
        }

        void com_SerialNumberReceived(object sender, SerialNumberArgs e)
        {
            throw new NotImplementedException();
        }

        /*
        void com_dataReceived(object sender, DMXMessage e)
        {
            int len;
            switch (e.type)
            {
                case DMXProMsgLabel.GET_WIDGET_PARAMETERS_REPLY: //3
                    UInt16 Firmware = (UInt16)(e.message[0] | (e.message[1] << 8));
                    double DMXOutBreakTime = 10.67 * e.message[2];
                    double DMXOutMarkTime = 10.67 * e.message[3];
                    int DMXOutRate = e.message[4];

                    len = e.message.Length - 5;
                    byte[] UserConfigData = new byte[len];
                    Array.Copy(e.message,5,UserConfigData,0,len);
                    break;
                case DMXProMsgLabel.GET_WIDGET_SERIAL_NUMBER_REPLY: //10
                    UInt32 SerialNumber;
                    SerialNumber = (UInt32)(e.message[0] | (e.message[1] << 8) | (e.message[2] << 16) | (e.message[3] << 24));
                    break;
                case DMXProMsgLabel.PROGRAM_FLASH_PAGE_REPLY:
                    bool success;
                    string label=Encoding.UTF8.GetString(e.message, 0, 4);
                    if (label == "TRUE") success = true;
                    else if (label == "FALSE") success = false;
                    else throw new Exception("Program Flash Page Responded with neither TRUE nor FALSE");
                    break;
                case DMXProMsgLabel.RECEIVED_DMX_CHANGE_OF_STATE_PACKET:
                    throw new NotImplementedException("Received DMX Change of State Packet is more effort than i want to put in at 12:26");
                    break;
                case DMXProMsgLabel.RECEIVED_DMX_PACKET:
                    //*The Widget sends this message to the PC unsolicited, 
                    // * whenever the Widget receives a DMX or
                    // * RDM packet from the DMX port, 
                    // * and the Receive DMX on Change mode is 'Send always'./
                    bool valid = (bool)((e.message[0] & 0x01)==1);
                    len = e.message.Length - 1;
                    byte[] levels = new byte[len];
                    Array.Copy(e.message, 1, levels, 0, len);
                    break;


            }                
        }
        */

        private void updateTextBox()
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < universeSize; i++)
            {
                str.AppendFormat("Ch{0,2}:{1,4} ", i + 1, LiveLevels[i]);
                if (i != 0 && ((i+1) % 6) == 0) str.AppendLine();


            }
            textBox1.Text = str.ToString();
        }

        private void updateWidget()
        {
            if (com.m_port.BytesToWrite == 0)
            {
                iterations = 0;
                com.sendDMXPacketRequest(LiveLevels);
            }
            else
            {
                iterations++;
            }
        }

        private void updateOutForm()
        {
            for (int i = 0; i < m_outForm.m_Bars.Count; i++)
            {
                m_outForm.m_Bars[i].Value = LiveLevels[i];
            }
        }


        
        private void FullScale(byte[] Live, byte[] X, byte[] Y, byte xLev, byte yLev)
        {
            int max = universeSize;
            for (int i = 0; i < max; i++)
            {
                Live[i] = scale(X[i], Y[i], xLev,yLev);
            }
            updateTextBox();
            updateOutForm();
            updateWidget();
        }
               

        private byte scale(byte Xval, byte Yval, byte XLevel,byte Ylevel)
        {
            int xTemp = (int)(Xval * (XLevel / 255.0));
            int yTemp = (int)(Yval * (Ylevel / 255.0));
            int temp =(int)Math.Round(((Xval * (XLevel / 255.0) + Yval * (Ylevel / 255.0))));

            return (byte)(xTemp < yTemp ? yTemp : xTemp);
            //return (byte)(temp > 255 ? 255 : temp);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            FullScale(LiveLevels, sliderGroup1.Values, sliderGroup2.Values, crossFaders1.Scene1Value, crossFaders1.Scene2Value);
            if (sender is CrossFaders )
            {
                if ((sender as CrossFaders).CrossFaderValue == 0) tabControl1.SelectTab(1);
                if ((sender as CrossFaders).CrossFaderValue == 255) tabControl1.SelectTab(0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Timer goTime = new Timer();
            if ((int)(((int)numericUpDown1.Value)/255.0) == 0)
            {
                crossFaders1.CrossFaderValue = (byte)(crossFaders1.CrossFaderValue == 0 ? 255 : 0);
                return;
            }
            goTime.Interval = (int)((double)numericUpDown1.Value / 255.0);
            goTime.Tick += new EventHandler(goTime_Tick);
            if (crossFaders1.CrossFaderValue == 0) change=  1;//up
            else change = -1;//down
            button1.Enabled = false;
            m_timer = new Stopwatch();
            m_timer.Start();
            
            goTime.Start();
            

        }

        void goTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Timer t = (sender as Timer);
            crossFaders1.CrossFaderValue = (byte)(crossFaders1.CrossFaderValue + change );

            if (crossFaders1.CrossFaderValue == 0 || crossFaders1.CrossFaderValue == 255)
            {
                button1.Enabled = true;
                
                t.Stop();
                m_timer.Stop();
                MessageBox.Show(string.Format("time={0}ms", m_timer.Elapsed));
            }
        }

        void updateFader(byte val)
        {
            crossFaders1.CrossFaderValue = val;
        }

        void goTime_Tick(object sender, EventArgs e)
        {

            Timer t = (sender as Timer);
            if (crossFaders1.InvokeRequired)
                crossFaders1.Invoke( new Action<byte>(updateFader),(byte)(crossFaders1.CrossFaderValue + change));
            else
                updateFader((byte)(crossFaders1.CrossFaderValue + change));

            if (crossFaders1.CrossFaderValue == 0 || crossFaders1.CrossFaderValue == 255)
            {
                button1.Enabled = true;
                t.Stop();
            }
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            com.GetWidgetSerialNumber();
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Regex rx = new Regex(@"(?<dimmer>\d+)(@(?<channel>\d+)(@(?<level>\d+))?)?", RegexOptions.Compiled);
            Regex rx2 = new Regex(@"\d+@\d+@\d+", RegexOptions.Compiled);


            Match m = rx.Match(textBox2.Text);
            //m = rx2.Match(textBox2.Text);
            label1.Text = "dimmer: " + m.Groups["dimmer"].Value;
            label2.Text = "channel: " + m.Groups["channel"].Value;
            label3.Text = "Value: " + m.Groups["level"].Value;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                Regex rx = new Regex(@"(?<dimmer>\d+)(@(?<channel>\d+)(@(?<level>\d+\%?))?)?", RegexOptions.Compiled);
                Match m = rx.Match(textBox2.Text);
                if (!m.Success) { MessageBox.Show("bad string"); return; }

                try
                {
                    int d = int.Parse(m.Groups["dimmer"].Value);
                    int c = int.Parse(m.Groups["channel"].Value);
                    byte l;
                    if (m.Groups["level"].Value == "")
                    {
                        l = 255;
                    }
                    else if (m.Groups["level"].Value.EndsWith("%"))
                    {
                        l = (byte)(int.Parse(m.Groups["level"].Value.TrimEnd('%')) * 255/100);
                    }
                    else
                    {
                        l = byte.Parse(m.Groups["level"].Value);
                    }

                    SliderGroup.patch(d, c, l);
                    MessageBox.Show(string.Format("Patched dimmer {0} to channel {1} @ {2}", d, c, l));
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentException) MessageBox.Show("there was an argument Exception");
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                Regex rx = new Regex(@"(?<channel>\d+)@(?<level>\d+\%?)", RegexOptions.Compiled);
                Match m = rx.Match(textBox3.Text);
                if (!m.Success) { MessageBox.Show("bad string"); return; }

                try
                {
                    int c = int.Parse(m.Groups["channel"].Value);
                    //byte l = byte.Parse(m.Groups["level"].Value);

                    int l;
                    if (m.Groups["level"].Value.EndsWith("%"))
                        l = (int.Parse(m.Groups["level"].Value.TrimEnd('%')) * 255 / 100);
                    else
                        l = int.Parse(m.Groups["level"].Value);

                    if (c < 1) throw new ArgumentOutOfRangeException("channel");
                    if (l < 0 || l > 255) throw new ArgumentOutOfRangeException("level");

                    sliderGroup1.setLevel(c, (byte) l);
                    MessageBox.Show(string.Format("channel {0} @ {1}", c, l));
                }
                catch (Exception ex)
                {
                        MessageBox.Show(ex.ToString());
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            com.detatchPro();
        }

    }
}
