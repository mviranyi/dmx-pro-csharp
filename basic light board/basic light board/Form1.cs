using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using basic_light_board.Properties;
using System.IO;

namespace basic_light_board
{
    public delegate void Action<T1, T2>(T1 arg1, T2 arg2);
    /// <summary>
    /// this form provides a basic 24 channel 1 to 1 patched XY cross fader.
    /// it allows for timed fades using a go button or manual time control using a crossfader.
    /// this currently does not interface with the Enttec USB pro because i havent gotten the thing yet
    /// if all goes well it should work just fine.
    /// </summary>
    public partial class Form1 : Form
    {
        public const int universeSize=512;
        public const int ShowSaveVersion = 200;

        /// <summary>
        /// LiveLevels contains one entry for every dimmer
        /// LiveLevels[0] contains the value for dimmer 1
        /// </summary>
        byte[] LiveLevels = new byte[universeSize];

        public CueList CList
        {
            get;
            set;
        }


        CueList mCList;
        LightCue blindCue;
        output m_outForm;
        Stopwatch m_timer;

        VComWrapper com;

        int iterations;
        int change;

        #region Form Events & constructor
        public Form1()
        {
            SliderGroup.Labels = Settings.Default.Labels.Split(',');
            SliderGroup.LabelChanged += new EventHandler<LabelChangedArgs>(SliderGroup_LabelChanged);
            InitializeComponent();

            mCList = new CueList();
            mCList.nextCueChanged += new EventHandler(mCList_nextCueChanged);
            //mCList.cueChanged += new EventHandler(mCList_cueChanged);
            

            

            com = new VComWrapper();
            com.SerialNumberReceived += new EventHandler<SerialNumberArgs>(com_SerialNumberReceived);
            com.WidgetParametersReceived += new EventHandler<WidgetParameterArgs>(com_WidgetParametersReceived);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            com.detatchPro();
            // save the labels
            //String.Join(",", intArray.Select(i => i.ToString()).ToArray());
            mCList.saveToFile(Settings.Default.cueFile);
            Settings.Default.Save();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            m_outForm = new output(48);
            m_outForm.Show();

            comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            comboBox1.SelectedIndex = 0;
            this.Text = "Not Connected";

            if (File.Exists(Settings.Default.cueFile))
            {
                mCList.loadFromFile(Settings.Default.cueFile);
            }
        }
        #endregion

        #region Form update Methods
        private void updateTextBox()
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < universeSize; i++)
            {
                str.AppendFormat("Ch{0,2}:{1,4} ", i + 1, LiveLevels[i]);
                if (i != 0 && ((i + 1) % 6) == 0) str.AppendLine();


            }
            textBox1.Text = str.ToString();
        }
        private void updateWidget()
        {
            if (com == null) return;
            if (com.IsOpen == false) return;
            if (com.m_port.BytesToWrite > 0) return;

            com.sendDMXPacketRequest(LiveLevels);
        }
        private void updateOutForm()
        {
            if (m_outForm == null) return;
            for (int i = 0; i < m_outForm.m_Bars.Count; i++)
            {
                m_outForm.m_Bars[i].Value = LiveLevels[i];
            }
        }
        void updateFader(byte c,byte n)
        {
            crossfaders1.CurrentSceneValue = c;
            crossfaders1.NextSceneValue = n;
        }

        #endregion

        #region handle CueList Events
        void mCList_cueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        void mCList_nextCueChanged(object sender, EventArgs e)
        {
            sliderGroupNext.ChannelValues = mCList.NextCue.channelLevels;
        }
        #endregion

        #region handle Go button
        private void GoButton_Click(object sender, EventArgs e)
        {
            Timer goTime = new Timer();
            goTime.Interval = 25;// 1/40 Sec  (max output rate of DMX)
            goTime.Tick += new EventHandler(goTime_Tick);
            button1.Enabled = false;
            m_timer = new Stopwatch();
            m_timer.Start();
            goTime.Start();
        }
        void goTime_Tick(object sender, EventArgs e)
        {
            Timer t = (sender as Timer);
            if (t.Enabled == false) { Console.WriteLine("**tick when Disabled**"); return; }
            long elapsed = m_timer.ElapsedMilliseconds;
            byte currentSceneVal = (byte)(255-Math.Min(255 * ((double)elapsed / mCList.NextCue.downFadeTime), 255));
            byte nextSceneVal = (byte)Math.Min(255 * ((double)elapsed / mCList.NextCue.upFadeTime), 255);

            Console.WriteLine(string.Format("Time_Tick:{0},{1}", currentSceneVal, nextSceneVal));
            
            
            if (crossfaders1.InvokeRequired)
                crossfaders1.Invoke(new Action<byte,byte>(updateFader),currentSceneVal, nextSceneVal);
            else
            {
                if (crossfaders1.NextSceneValue!=255 && crossfaders1.CurrentSceneValue ==0)
                    crossfaders1.NextSceneValue = nextSceneVal;
                else if (crossfaders1.CurrentSceneValue != 0 && crossfaders1.NextSceneValue == 255)
                    crossfaders1.CurrentSceneValue = currentSceneVal;
                else
                {
                    crossfaders1.CurrentSceneValue = currentSceneVal;
                    crossfaders1.NextSceneValue = nextSceneVal; 
                }

                
            }
            if (currentSceneVal == 0 && nextSceneVal == 255)
            {
                t.Stop();
                m_timer.Stop();
                m_timer = null;//release timer
                t.Enabled = false;
                Console.WriteLine("timer stopped");
                if (mCList.NextCue.isFollowCue)
                {
                    Timer follow = new Timer();
                    follow.Interval = mCList.NextCue.followTime;
                    follow.Tick += new EventHandler(follow_Tick);
                    follow.Start();
                }
                else
                {
                    button1.Enabled = true;
                }
            }

            
        }

        void follow_Tick(object sender, EventArgs e)
        {
            if (!(sender is Timer)) return;
            (sender as Timer).Stop();
            GoButton_Click(sender, e);
        }

        #endregion

        #region handle Widget Events
        private void button3_Click(object sender, EventArgs e)
        {
            if (com.initPro((string)comboBox1.SelectedItem))
                com.sendGetWidgetParametersRequest((ushort)0);
        }

        void com_WidgetParametersReceived(object sender, WidgetParameterArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new System.Action<string>(delegate(string s) { this.Text =s; }),"Connected");
            else
                this.Text = "Connected";
        }
        void com_SerialNumberReceived(object sender, SerialNumberArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        void SliderGroup_LabelChanged(object sender, LabelChangedArgs e)
        {
            string[] temp = Settings.Default.Labels.Split(',');
            if (temp.Length < (e.slider.Channel))
            {
                string[] temp1 = new string[e.slider.Channel];
                Array.Copy(temp, temp1, temp.Length);
                temp = temp1;
            }
            temp[e.slider.Channel - 1] = e.slider.textBox1.Text;
            Settings.Default.Labels = String.Join(",", temp);
            SliderGroup.Labels = temp;
            Settings.Default.Save();
        }
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            //FullScale(LiveLevels, sliderGroupLive.Values, sliderGroupNext.Values, crossFaders1.Scene1Value, crossFaders1.Scene2Value);
            outputLightMix(LiveLevels,SliderGroup.Patchlist,SliderGroup.Level,
                sliderGroupLive.ChannelValues, 
                mCList.CurrentCue.channelLevels, mCList.NextCue.channelLevels,
                crossfaders1.CurrentSceneValue, crossfaders1.NextSceneValue);
        }
        
        
        /// <summary>
        /// this will fill the dimmer list (Output) based on the value of the crossfaders/(next& currents Scenes)
        /// as wel as the Live console.
        /// </summary>
        /// <param name="Output">a list of dimmers Output[0] should be dimmer#1's value</param>
        /// <param name="patchList">this is how dimmers are patched into channels patchlist[0] contains the channel that dimmer#1 corrisponds to</param>
        /// <param name="Live">the CHANNEL list of the live values</param>
        /// <param name="currentScene">the CHANNEL list of the current scene</param>
        /// <param name="nextScene">the CHANNEL list of the next scene</param>
        /// <param name="currentSceneVal">the scale factor(0-255) of the current scene</param>
        /// <param name="NextSceneVal">the scale factor(0-255) of the next scene</param>
        private void outputLightMix(byte[] Output, List<int> patchList, List<byte> maxLevel, byte[] Live, byte[] currentScene, byte[] nextScene, byte currentSceneVal, byte NextSceneVal)
        {
            int i;
            int max = universeSize;
            int tChannel;
            byte tLevel;
            for (i = 0; i < max; i++)
            {
                tChannel = patchList[i]-1;
                tLevel = maxLevel[i];
                Output[i] = scale(Live[tChannel], currentScene[tChannel],
                          nextScene[tChannel],
                          currentSceneVal, NextSceneVal, tLevel);
            }
            updateTextBox();
            updateOutForm();
            updateWidget();
        }
        private byte[] mixChannelVals()
        {
            // for each channel mix the Live Sliders, the current Scene and Next Scene
            byte[] Output = new byte[512]; // this is a channel List fyi (not a dimmer List)
            int currentTemp,nextTemp,live;
            for (int i = 1; i <= 512; i++)
            {
                currentTemp = mCList.CurrentCue[i] * crossfaders1.CurrentSceneValue / 255;
                nextTemp = mCList.NextCue[i] * crossfaders1.NextSceneValue / 255;
                live = sliderGroupLive.ChannelValues[i-1];
                Output[i-1] = (byte)Math.Min(255, Math.Max(live, currentTemp + nextTemp));
            }
            return Output;
        }
        private byte scale(byte live, byte Xval, byte Yval, byte XLevel, byte Ylevel, byte maxDimmerVal)
        {
            int xTemp = (int)(Xval * (XLevel / 255.0) * (maxDimmerVal / 255.0));
            int yTemp = (int)(Yval * (Ylevel / 255.0) * (maxDimmerVal / 255.0));
            int liveTemp = (int)(live * (maxDimmerVal / 255));
            int temp =xTemp + yTemp;

            return (byte)Math.Min(255,Math.Max(liveTemp, temp));
            //return (byte)(xTemp < yTemp ? yTemp : xTemp);
            //return (byte)(temp > 255 ? 255 : temp);
        }


        private void button2_Click(object sender, EventArgs e)//record Cue
        {
            CueNumberForm c = new CueNumberForm();
            c.ShowDialog();
            if (c.DialogResult==DialogResult.Cancel)return;
            mCList.AddCue(new LightCue(c.CueNum, c.CueName, mixChannelVals() ));
        }

        #region Patch Cmd
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
        #endregion

        #region Channel Cmd
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                Regex rx = new Regex(@"(?<channel>\d+)@(?<level>\d+\%?)", RegexOptions.Compiled);
                Match m = rx.Match(txtLiveCmd.Text);
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

                    sliderGroupLive.setLevel(c, (byte) l);
                    MessageBox.Show(string.Format("channel {0} @ {1}", c, l));
                }
                catch (Exception ex)
                {
                        MessageBox.Show(ex.ToString());
                }
            }
        }
        #endregion
        
        private void sliderGroupLive_ValueChanged(object sender, EventArgs e)
        {
            outputLightMix(LiveLevels,SliderGroup.Patchlist,SliderGroup.Level, sliderGroupLive.dimmerValues, 
                mCList.CurrentCue.channelLevels, mCList.NextCue.channelLevels, 
                crossfaders1.CurrentSceneValue, crossfaders1.NextSceneValue);
        }

        

        private void crossFaders1_SceneChanged(object sender, EventArgs e)
        {
            mCList.gotoNextCue();
            updateCueLabels();
            sliderGroupNext.ChannelValues  = mCList.NextCue.channelLevels;

        }
        private void updateCueLabels()
        {
            lblCueCurrent.Text = string.Format("Current Cue:{0}", mCList.CurrentCueNumber);
            lblCueNext.Text = string.Format("Next Cue:{0}", mCList.NextCueNumber);
            lblCuePrev.Text = string.Format("Previous Cue:{0}", mCList.PrevCueNumber);
        }

        private void txtBlindCmd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                int cue;
                int channel;
                if (txtBlindCmd.Text[0] == 'C' || txtBlindCmd.Text[0] == 'c')
                {
                    if (int.TryParse(txtBlindCmd.Text.Substring(1), out channel))
                        sliderGroupBlind.SelectSlider(channel);
                    else
                        MessageBox.Show("not a valid channel number");
                }
                else if (int.TryParse(txtBlindCmd.Text, out cue))
                {
                    if (mCList[cue] != null)
                        loadCueIntoBlind(cue);
                    else
                    {
                        LightCue newBlankCue = LightCue.BlankCue;
                        newBlankCue.cueNumber=cue;
                        mCList.AddCue(newBlankCue);
                        loadCueIntoBlind(cue);
                    }
                }
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                txtBlindCmd.Text = "";
            }
        }

        public void loadCueIntoBlind(int num)
        {
            blindCue = mCList[num];
            
            if (blindCue==null) return;
            Console.WriteLine("before blindslider.channels=blindcue.channel:");
            Console.WriteLine(blindCue.serialize());
            sliderGroupBlind.ChannelValues = blindCue.channelLevels; 
            Console.WriteLine("after blindslider.channels=blindcue.channel:");
            Console.WriteLine(blindCue.serialize());
            
            txtCueName.Text = blindCue.cueName;
            nudDownFade.Value = blindCue.downFadeTime;
            nudUpFade.Value = blindCue.upFadeTime;
            nudFollowTime.Value = blindCue.followTime;
            chkFollow.Checked = blindCue.isFollowCue;
        }

        private void sliderGroupBlind_ValueChanged(object sender, EventArgs e)
        {
            blindCue.channelLevels = sliderGroupBlind.ChannelValues;
        }
        private void txtCueName_TextChanged(object sender, EventArgs e)
        {
            blindCue.cueName = txtCueName.Text;
        }

        private void nudUpFade_ValueChanged(object sender, EventArgs e)
        {
            blindCue.upFadeTime = (int)nudUpFade.Value;
        }

        private void nudDownFade_ValueChanged(object sender, EventArgs e)
        {
            blindCue.downFadeTime = (int)nudDownFade.Value ;
        }

        private void nudFollowTime_ValueChanged(object sender, EventArgs e)
        {
            blindCue.followTime = (int)nudFollowTime.Value ;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar==(char)Keys.F1)
            {
                tabControl1.SelectedTab = tabPageLive;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.KeyCode == Keys.F1)
                tabControl1.SelectedTab = tabPageLive;
            else if (e.KeyCode == Keys.F2)
                tabControl1.SelectedTab = tabPageNext;
            else if (e.KeyCode == Keys.F3)
                tabControl1.SelectedTab = tabPageBlind;
            else if (e.KeyCode == Keys.F4)
                tabControl1.SelectedTab = tabPagePatch;
            else if (e.KeyCode == Keys.F5)
                tabControl1.SelectedTab = tabPageConnection;
            else
            {
                if (e.KeyCode == Keys.Up && tabControl1.SelectedTab == tabPageBlind)
                { }
            }
            e.Handled = false;
             */
        }

        private void txtBlindCmd_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmdDeleteCue_Click(object sender, EventArgs e)
        {
            if (blindCue == null) return;
            MessageBox.Show("are you sure?", "Confirm?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1 );
            
            mCList.RemoveCue(blindCue.cueNumber);
            blindCue = null;
        }

        private void chkFollow_CheckedChanged(object sender, EventArgs e)
        {
            blindCue.isFollowCue = chkFollow.Checked;
        }

        private void toolStripButtonSaveShow_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.CreatePrompt = true;
            dlg.DefaultExt = "txt";
            dlg.OverwritePrompt = true;
            dlg.Title = "Where to save the show?";
            dlg.FileOk += new CancelEventHandler(dlg_FileOk);
            dlg.ShowDialog();
            


        }

        void dlg_FileOk(object sender, CancelEventArgs e)
        {
            if (!(sender is SaveFileDialog)) return; // sanity check
            //save the show (patchList and cue List(all cues,current,prev,next) and current Live Levels and crossfader levels)
            try
            {
                SaveFileDialog dlg = (sender as SaveFileDialog);
                System.IO.StreamWriter s=new StreamWriter(dlg.FileName);
                //write version of save file
                s.WriteLine(Form1.ShowSaveVersion);
                StringBuilder Plist = new StringBuilder();
                #region construct the patchlist
                foreach (int dimmer in SliderGroup.Patchlist)
                {
                    Plist.AppendFormat("{0},", dimmer);
                }
                Plist.Remove(Plist.Length - 1, 1); // remove the extra ","
                #endregion
                s.WriteLine(Plist.ToString());
            }
            catch
            {
            }


        }

        private void cmdSetNextCue_Click(object sender, EventArgs e)
        {
            CueNumberForm cnf = new CueNumberForm();
            cnf.ShowDialog();
            if (cnf.DialogResult == DialogResult.OK)
            {
                if (cnf.CueName!="") 
                {
                    if (mCList.setNextCue(cnf.CueName))
                        MessageBox.Show("cue set");
                    else
                        MessageBox.Show("no such Cue Name");
                }
                else 
                {
                    if (mCList.setNextCue(cnf.CueNum))
                        MessageBox.Show("cue set");
                    else
                        MessageBox.Show("no such Cue Number");
                }
            }
        }
    }
}
