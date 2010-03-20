using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace basic_light_board
{
    public partial class SubmasterSliderGroup : UserControl
    {
        const int numSlider = 24;
        const int itemsPerRow = 12;
        const int itemsPerHGrp = 12;
        /// <summary>
        /// prevents the control from updating values when lots of stuff is happening
        /// </summary>
        private int suspendValuesChanged = 0;
        /// <summary>
        /// the actual GUI elements
        /// </summary>
        private SingleSlider[] mSliders=new SingleSlider[LightingConstants.numSubSliders];
        /// <summary>
        /// constains a list of (submasters (eg List(channel@level) ) )
        /// </summary>
        private Submaster[] mSubmasters = new Submaster[LightingConstants.numSubSliders];
        /// <summary>
        /// contains the list of all submaster labels
        /// </summary>
        private String[] mLabels = new string[LightingConstants.numSubSliders];
        /// <summary>
        /// maintains a list of the state of every channels level, due to submasters.
        /// </summary>
        private byte[] mChannelLevels = new byte[LightingConstants.maxChannels];

        #region events
        [Description("cueChanged fires when any Value property changes")]
        [Category("Action")]
        public event EventHandler ValueChanged;

        [Description("cueChanged fires when any Label property changes")]
        [Category("Action")]
        public static event EventHandler<LabelChangedArgs> LabelChanged;
        #endregion

        #region properties
        public byte[] ChannelLevels
        {
            get 
            {
                /* Console.WriteLine("SubmasterSliderGroup: {0} getChannelLevels", this.Name); */
                return mChannelLevels; 
            }
        }
        #endregion

        #region control setup and maintanence
        public SubmasterSliderGroup()
        {
            InitializeComponent();
            for (int i = 0; i < mSubmasters.Length; i++) mSubmasters[i] = new Submaster(i + 1);
            mSubmasters[0].SubValues[0] = 100;
           
        }
        private void setupSliders()
        {
            SingleSlider temp;

            int col, row;
            this.SuspendLayout();
            for (int i = 0; i < numSlider; i++)
            {
                temp = new SingleSlider(i + 1);
                //temp.Channel = i + 1;
                temp.ValueChanged += singleSlider1_ValueChanged;
                temp.Scroll += singleSlider1_Scroll;
                temp.LabelChanged += singleSlider1_LabelChanged;
                col = i % itemsPerRow + (i % itemsPerRow) / itemsPerHGrp;
                row = (i / itemsPerRow);
                temp.Left = (temp.Width) * col + ((col + 1) * temp.Margin.Left);
                temp.Top = (temp.Height) * row + ((row + 1) * 10);
                mSliders[i] = temp;
                this.Controls.Add(temp);
                //this.components.Add(temp);

                if (mLabels == null) continue;
                if (mLabels.Length >= temp.Channel)
                    temp.textBox1.Text = mLabels[temp.Channel - 1];
            }
            this.ResumeLayout(false);
        }
        private void SliderGroup_Resize(object sender, EventArgs e)
        {
            int rows = numSlider / itemsPerRow;
            int cols = itemsPerRow + itemsPerRow / itemsPerHGrp - 1;
            this.SuspendLayout();
            foreach (SingleSlider s in mSliders)
            {
                if (s == null) continue;
                s.Width = this.Width / cols - s.Margin.Left - s.Margin.Right;
                s.Left = ((s.Channel - 1) % itemsPerRow + ((s.Channel - 1) % itemsPerRow / itemsPerHGrp)) * (s.Width + s.Margin.Left + s.Margin.Right) + s.Margin.Left;
            }
            this.ResumeLayout(false);
        }
        private void SubmasterSliderGroup_Load(object sender, EventArgs e)
        {
            setupSliders();
            
        }
        #endregion

        //helper functions
        public byte getChannelLevel(int channelNumber)
        {
            /* Console.WriteLine("SubmasterSliderGroup: {0} getChannelLevel:{1}", this.Name, channelNumber); */
            return mChannelLevels[channelNumber-1];
        }
        public byte getSubLevel(int subNumber)
        {
            /* Console.WriteLine("SubmasterSliderGroup: {0} getSubLevel:{1}", this.Name, subNumber); */
            if (subNumber<0 || subNumber>=LightingConstants.numSubSliders) 
                throw new ArgumentOutOfRangeException("subNumber");
            return mSliders[subNumber-1].Value;
        }
        public void setSubLevel(int subNumber, byte value)
        {
            /* Console.WriteLine("SubmasterSliderGroup: {0} setSubLevel:{1}-{2}", this.Name, subNumber,value); */
            if (subNumber < 0 || subNumber >= LightingConstants.numSubSliders)
                throw new ArgumentOutOfRangeException("subNumber");
            mSliders[subNumber - 1].Value = value;
        }
        public Submaster getSubmaster(int subNum)
        {
            /* Console.WriteLine("SubmasterSliderGroup: {0} getSubMaster:{1}", this.Name, subNum); */
            return mSubmasters[subNum - 1];
        }
        private void mixSubLevels()
        {
            /* Console.WriteLine("SubmasterSliderGroup: {0} mixSubLevels", this.Name); */
            int i, j;
            byte adjustedVal;

            for (j = 1; j <= LightingConstants.maxChannels; j++)
            {
                mChannelLevels[j-1] = 0;
                for (i = 0; i < mSliders.Length; i++)
                {
                    if (mSliders[i] == null) return;
                    adjustedVal = (byte)((int)mSubmasters[i][j] * mSliders[i].Value / (byte)255);
                    if (mChannelLevels[j-1] < adjustedVal)
                        mChannelLevels[j-1] = adjustedVal;
                }
            }
        }


        public void suspendValueChangedUpdates()
        {
            /* Console.WriteLine("SubmastrerSliderGroup: {0} suspendValueChangedUpdates()", this.Name); */
            suspendValuesChanged++;
        }
        public void resumeValueChangedUpdates()
        {
            /* Console.WriteLine("SubmastrerSliderGroup: {0} suspendValueChangedUpdates()", this.Name); */
            suspendValuesChanged--;
            onValuesChanged();
        }

        private void onValuesChanged()
        {
            /* Console.WriteLine("SubmastrerSliderGroup: {0} suspendValueChangedUpdates()", this.Name); */
            if (suspendValuesChanged==0 && ValueChanged != null)
            {
                mixSubLevels();
                ValueChanged(this, new EventArgs());
            }
        }
        

        
        #region handle all of the sliders events
        
        private void singleSlider1_LabelChanged(object sender, EventArgs e)
        {
            SingleSlider s = (SingleSlider)sender;
            //mLabels[s.Channel - 1] = s.textBox1.Text;
            mSubmasters[s.Channel-1].SubName = s.textBox1.Text;
            if (LabelChanged != null)
            {
                LabelChanged(this, new LabelChangedArgs(sender as SingleSlider));
                return;
            }            
        }
        private void singleSlider1_ValueChanged(object sender, EventArgs e)
        {
            //int channel = (sender as SingleSlider).Channel;
            //byte value = (sender as SingleSlider).Value;
            /* Console.WriteLine("SubmastrerSliderGroup: {0} sliderValueChanged", this.Name); */
            onValuesChanged();
        }
        private void singleSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            //int channel = (sender as SingleSlider).Channel;
            //byte value = (sender as SingleSlider).Value;
            onValuesChanged();
        }

        #endregion

        

        public void saveToFile(string fileName)
        {
            System.IO.StreamWriter f = new System.IO.StreamWriter(fileName, false, Encoding.UTF8);
            
            f.WriteLine("{0}", this.mSubmasters.Length);
            foreach (Submaster s in this.mSubmasters)
            {
                f.WriteLine(s.serialize());
            }
            f.Close();
        }

        public void LoadSubsFromFile(string fileName)
        {
            StreamReader f = null;
            string s;
            try
            {
                f = new System.IO.StreamReader(fileName, Encoding.UTF8);
                
                //if (int.Parse(f.ReadLine()) != LightCue.version) throw new InvalidDataException("version of cue file not compatable");
                s = f.ReadLine();
                int num = int.Parse(s);
                for (int i = 0; i < num; i++)
                {
                    s = f.ReadLine();
                    this.mSubmasters[i] = new Submaster(s);
                    this.mSliders[i].textBox1.Text = mSubmasters[i].SubName;
                }
            }
            catch
            {
                
            }
            finally
            {
                if (f != null) f.Close();
            }
        }
        


        /*
        private void onValuesChanged()
        {
            if (ValueChanged != null && !suspendValuesChanged) ValueChanged(this, new EventArgs());
        }

        public static string[] Labels;
        
        private List<SingleSlider> m_sliders;
        
        [Description("cueChanged fires when any Value property changes")]
        [Category("Action")]
        public event EventHandler ValueChanged;

        [Description("cueChanged fires when any Label property changes")]
        [Category("Action")]
        public static event EventHandler<LabelChangedArgs> LabelChanged;

        public SubmasterSliderGroup()
        {
            InitializeComponent();

            if (SubPatchlist.Length==0) setupPatch();
            
            // construct the list of sliders based  what is already in the control
            m_sliders=new List<SingleSlider>();
            SingleSlider temp;

            int col,row;
            for (int i = 0; i < numSlider; i++)
            {
                temp = new SingleSlider(i + 1);
                temp.ValueChanged += singleSlider1_ValueChanged;
                temp.Scroll += singleSlider1_Scroll;
                temp.LabelChanged += singleSlider1_LabelChanged;
                m_sliders.Add(temp);
                col = i % itemsPerRow + (i % itemsPerRow) / itemsPerHGrp;
                row = (i / itemsPerRow);
                temp.Left = (temp.Width) * col + ((col + 1) * temp.Margin.Left);
                temp.Top = (temp.Height) * row + ((row + 1) * 10);
                this.Controls.Add(temp);

                if (Labels == null) continue;
                if (Labels.Length >=temp.Channel)
                    temp.textBox1.Text = Labels[temp.Channel - 1];
            }

            m_sliders.Sort(delegate(SingleSlider a, SingleSlider b) { return a.Channel.CompareTo(b.Channel); });
        }
        private void SliderGroup_Resize(object sender, EventArgs e)
        {
            int rows = numSlider / itemsPerRow;
            int cols = itemsPerRow + itemsPerRow / itemsPerHGrp - 1;

            foreach (SingleSlider s in m_sliders)
            {
                s.Width = this.Width / cols - s.Margin.Left - s.Margin.Right;
                s.Left = ((s.Channel - 1) % itemsPerRow + ((s.Channel - 1) % itemsPerRow / itemsPerHGrp)) * (s.Width + s.Margin.Left + s.Margin.Right) + s.Margin.Left;
            }
        }

        public bool SelectSlider(int Number)
        {
            if (Number >= m_sliders.Count || Number <= 0) return false;
            m_sliders[Number - 1].Select();
            return true;
        }
        public void setLevel(int sub, byte value)
        {
            if (sub < 1) throw new ArgumentOutOfRangeException("channel"); 
            if (value<0 || value >255 ) throw new ArgumentOutOfRangeException("value");
            
            if (sub < m_sliders.Count)
            {
                if (m_sliders[sub - 1].Value != value)
                {
                    m_sliders[sub - 1].Value = value;
                }
            }
            mSubLevels[sub - 1] = value;
            updateValuesWithPatchList(sub, value);
        }
        public static void addChannel(int sub, int channel, byte maxLevel)
        {
            if (sub < 1 || sub > numSlider) throw new ArgumentOutOfRangeException("Sub");
            if (channel < 1) throw new ArgumentOutOfRangeException("channel");
            if (maxLevel < 0 || maxLevel > 255) throw new ArgumentOutOfRangeException("maxLevel");
            SubPatchlist[sub, channel]. = maxLevel;
        }
        public static void patch(int dimmer, int channel, byte maxLevel)
        {
            if (dimmer < 1 || dimmer > 512) throw new ArgumentOutOfRangeException("Dimmer)");
            if (channel < 1) throw new ArgumentOutOfRangeException("dimmer");
            if (maxLevel < 0 || maxLevel > 255) throw new ArgumentOutOfRangeException("maxLevel");
            SubPatchlist[dimmer-1]=channel;
            Level[dimmer-1]=maxLevel;
        }

        #region handle all of the sliders events
        private void singleSlider1_LabelChanged(object sender, EventArgs e)
        {
            if (LabelChanged != null)
            {
                LabelChanged(this, new LabelChangedArgs(sender as SingleSlider));
                return;
            }
            SingleSlider s = (SingleSlider)sender;
            if (Labels.Length < (s.Channel))
            {
                string[] temp = new string[s.Channel];
                Array.Copy(Labels, temp, Labels.Length);
                Labels = temp;
            }
            Labels[s.Channel - 1] = s.textBox1.Text;
        }
        private void singleSlider1_ValueChanged(object sender, EventArgs e)
        {
            int channel = (sender as SingleSlider).Channel;
            byte value = (sender as SingleSlider).Value;
            setLevel(channel, value);
        }
        private void singleSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            int channel = (sender as SingleSlider).Channel;
            byte value = (sender as SingleSlider).Value;
            setLevel(channel, value);
        }
        #endregion

        private void updateValuesWithPatchList(int chan,byte value)
        {
            
            int i=0;
             //update dimmerList based on the patch list
            i=Patchlist.IndexOf(chan, i);
            while (i != -1)
            {
                mValues[i] = (byte)(value*Level[i]/255);
                i = Patchlist.IndexOf(chan, i+1);
            }
            onValuesChanged();
        }
    }
    public class ChannelPatch
    {
        public byte MinLevel;
        public byte MaxLevel;
        public ChannelPatch() : this(0, 0)
        { }
        public ChannelPatch(byte min,byte max)
        {
            MinLevel = min;
            MaxLevel = max;
        }
         */
    }
         
}
