using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using basic_light_board.Properties;

namespace basic_light_board
{
    public partial class SliderGroup : UserControl
    {
        int numChannels = LightingConstants.numChannelSliders;
        int itemsPerRow = 24;
        int itemsPerHGrp = 12;
        private int suspendValuesChanged = 0;
        /// <summary>
        /// Values contains one entry for ever dimmer
        /// Values[0] is the value of dimmer 1 (0-255)
        /// </summary>
        private byte[] mValues = new byte[LightingConstants.maxDimmers];

        /// <summary>
        /// mChannelLevel contains one entry for every channel
        /// mChannelLevel[0] contains the value for channel 1 (0-255)
        /// </summary>
        private byte[] mChannelLevels = new byte[LightingConstants.maxChannels]; //512 is just a safty maximum....this is the number of channels not dimmers

        public byte[] dimmerValues
        {
            get { return mValues; }
            set { mValues = value; onValuesChanged(); }
        } 

        public byte[] ChannelValues
        {
            get { return mChannelLevels; }
            set
            {
                /* Console.WriteLine("SliderGroup: {0} setting channelValues", this.Name); */
                suspendValuesChanged++;
                for (int i = 0; i < Math.Min(m_sliders.Length,value.Length); i++)
                {
                    m_sliders[i].Value = value[i];
                }
                suspendValuesChanged--;
                onValuesChanged();
            }
        }


        public byte getChannelValue(int channelNum)
        {
            return mChannelLevels[channelNum-1];
        }
        public void setChannelValue(int channelNum,byte value)
        {
            /* Console.WriteLine("SliderGroup: {0} setting channelValue:{1} to {2}", this.Name, channelNum, value); */
            m_sliders[channelNum - 1].Value = value;
        }

        public void suspendValueChangedUpdates()
        {
            suspendValuesChanged++;
            /* Console.WriteLine("SliderGroup: {0} suspendValueChangedUpdates:", this.Name,suspendValuesChanged); */
        }
        public void resumeValueChangedUpdates()
        {
            
            suspendValuesChanged--;
            /* Console.WriteLine("SliderGroup: {0} resumeValueChangedUpdates:", this.Name, suspendValuesChanged); */
            onValuesChanged();
        }

        private void onValuesChanged()
        {
            /* Console.WriteLine("SliderGroup: {0} onValuesChanged():{1}", this.Name,suspendValuesChanged); */
            if (ValueChanged != null && suspendValuesChanged==0) ValueChanged(this, new EventArgs());
        }

        /// <summary>
        ///Patchlist contains one entry for every dimmer
        ///Patchlist[0] contains the channel that dimmer 1 is patched into. (1-> anything)
        /// </summary>
        public static int[] Patchlist=new int[LightingConstants.maxDimmers];

        
        /// <summary>
        ///Level contains one entry for every channel
        ///Level[0] contains the maxLevel that channel 1 can achieve (0->255)
        /// </summary>
        public static byte[] Level = new byte[LightingConstants.maxChannels];

        public static string[] Labels = new string[LightingConstants.numChannelSliders];
        
        private SingleSlider[] m_sliders= new SingleSlider[LightingConstants.numChannelSliders];
        
        [Description("cueChanged fires when any Value property changes")]
        [Category("Action")]
        public event EventHandler ValueChanged;

        [Description("cueChanged fires when any Label property changes")]
        [Category("Action")]
        public static event EventHandler<LabelChangedArgs> LabelChanged;



        public bool SelectSlider(int channelNumber)
        {
            if (channelNumber >= m_sliders.Length || channelNumber<=0) return false;
            m_sliders[channelNumber - 1].Select();
            return true;
        }

        public SliderGroup()
        {
            InitializeComponent();

            setupPatch();
            
            // construct the list of sliders based  what is already in the control
            
            SingleSlider temp;
            //tableLayoutPanel1.RowCount=(numChannels/itemsPerRow)*2-1 ; // 1 row is a spacer. 5 rows = 4 spaces = 9 Total;
            //tableLayoutPanel1.ColumnCount=itemsPerRow + itemsPerRow/itemsPerHGrp-1; // items/(itemspergrp)=groups. 3 groups=2 spaces

            this.SuspendLayout();
            int col,row;
            for (int i = 0; i < numChannels; i++)
            {
                temp = new SingleSlider(i + 1);
                temp.ValueChanged += singleSlider1_ValueChanged;
                temp.Scroll += singleSlider1_Scroll;
                temp.LabelChanged += singleSlider1_LabelChanged;
                m_sliders[i] = temp;
                col = i % itemsPerRow + (i % itemsPerRow) / itemsPerHGrp;
                row = (i / itemsPerRow);
                temp.Left = (temp.Width) * col + ((col + 1) * temp.Margin.Left);
                temp.Top = (temp.Height) * row + ((row + 1) * 10);
                this.Controls.Add(temp);


                if (Labels == null) continue;
                if (Labels.Length >=temp.Channel)
                    temp.textBox1.Text = Labels[temp.Channel - 1];
                
            }
            this.ResumeLayout();
        }

        public void setLevel(int channel, byte value)
        {
            if (channel < 1) throw new ArgumentOutOfRangeException("channel"); 
            if (value<0 || value >255 ) throw new ArgumentOutOfRangeException("value");
            /* Console.WriteLine("SliderGroup: {0} setLevel({1},{2}):", this.Name, channel,value); */
            if (channel < m_sliders.Length)
            {
                if (m_sliders[channel - 1].Value != value)
                {
                    m_sliders[channel - 1].Value = value;
                }
            }
            ChannelValues[channel - 1] = value;
            updateValuesWithPatchList(channel, value);
        }

        public static void patch(int dimmer, int channel, byte maxLevel)
        {
            if (dimmer < 1 || dimmer > 512) throw new ArgumentOutOfRangeException("Dimmer)");
            if (channel < 1) throw new ArgumentOutOfRangeException("dimmer");
            if (maxLevel < 0 || maxLevel > 255) throw new ArgumentOutOfRangeException("maxLevel");
            Patchlist[dimmer-1]=channel;
            Level[dimmer-1]=maxLevel;
        }
        //apply a 1 to 1 patch
        private void setupPatch()
        {
            for (int i = 1; i <= LightingConstants.maxDimmers; i++)
            {
                patch(i, i, 255);
            }
        }

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
            /* Console.WriteLine("SliderGroup: {0} singleSliderValueChanged-ch:{1} val:{2}", this.Name, channel, value); */
            setLevel(channel, value);
        }
        private void singleSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            int channel = (sender as SingleSlider).Channel;
            byte value = (sender as SingleSlider).Value;
            /* Console.WriteLine("SliderGroup: {0} singleSliderScroll-ch:{1} val:{2}", this.Name, channel, value); */
            setLevel(channel, value);
        }

        private void updateValuesWithPatchList(int chan,byte value)
        {
            /* Console.WriteLine("SliderGroup: {0} updateValuesWithPatchlist-ch:{1} val:{2}", this.Name, chan, value); */
            int i=0;
             //update dimmerList based on the patch list
            i=Array.IndexOf<int>(Patchlist, chan, i);
            while (i != -1)
            {
                mValues[i] = (byte)(value*Level[i]/255);
                i = Array.IndexOf<int>(Patchlist, chan, i+1);
            }
            onValuesChanged();
        }

        private void SliderGroup_Resize(object sender, EventArgs e)
        {
            int rows = numChannels/itemsPerRow;
            int cols = itemsPerRow+ itemsPerRow/itemsPerHGrp-1;
            this.SuspendLayout();
            foreach(SingleSlider s in m_sliders)
            {
                s.Width = this.Width / cols - s.Margin.Left - s.Margin.Right;
                s.Left = ((s.Channel - 1)%itemsPerRow+ ((s.Channel-1)%itemsPerRow/itemsPerHGrp)) * (s.Width + s.Margin.Left+s.Margin.Right) + s.Margin.Left;
            }
            this.ResumeLayout();
        }

        private void SliderGroup_Load(object sender, EventArgs e)
        {

        }
    }
    public class LabelChangedArgs : EventArgs
    {
        public SingleSlider slider;
        public LabelChangedArgs(SingleSlider s)
        {
            slider = s;
        }

    }
    
}
