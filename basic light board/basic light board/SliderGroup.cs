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
        private bool suspendValuesChanged = false;
        /// <summary>
        /// Values contains one entry for ever channel
        /// Values[0] is the value of channel 1 (0-255)
        /// </summary>
        private byte[] mValues = new byte[512];

        /// <summary>
        /// mChannelLevel contains one entry for every channel
        /// mChannelLevel[0] contains the value for channel 1 (0-255)
        /// </summary>
        private byte[] mChannelLevels = new byte[512]; //512 is just a safty maximum....this is the number of channels not dimmers

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
                suspendValuesChanged = true;
                for (int i = 0; i < Math.Min(m_sliders.Count,value.Length); i++)
                {
                    m_sliders[i].Value = value[i];
                }
                suspendValuesChanged = false;
                onValuesChanged();
            }
        }


        private void onValuesChanged()
        {
            if (ValueChanged != null && !suspendValuesChanged) ValueChanged(this, new EventArgs());
        }

        /// <summary>
        ///Patchlist contains one entry for every channel
        ///Patchlist[0] contains the channel that channel 1 is patched into. (1-> anything)
        /// </summary>
        public static List<int> Patchlist=new List<int>();

        
        /// <summary>
        ///Level contains one entry for every channel
        ///Level[0] contains the maxLevel that channel 1 can achieve (0->255)
        /// </summary>
        public static List<byte> Level = new List<byte>();

        public static string[] Labels;
        
        private List<SingleSlider> m_sliders;
        
        [Description("cueChanged fires when any Value property changes")]
        [Category("Action")]
        public event EventHandler ValueChanged;

        [Description("cueChanged fires when any Label property changes")]
        [Category("Action")]
        public static event EventHandler<LabelChangedArgs> LabelChanged;

         int numChannels = 96;
         int itemsPerRow = 24;
         int itemsPerHGrp = 12;

        public bool SelectSlider(int channelNumber)
        {
            if (channelNumber >= m_sliders.Count) return false;
            m_sliders[channelNumber - 1].Select();
            return true;
        }

        public SliderGroup()
        {
            InitializeComponent();

            if (Patchlist.Count==0) setupPatch();


           
            
            
            // construct the list of sliders based  what is already in the control
            m_sliders=new List<SingleSlider>();
            SingleSlider temp;
            //tableLayoutPanel1.RowCount=(numChannels/itemsPerRow)*2-1 ; // 1 row is a spacer. 5 rows = 4 spaces = 9 Total;
            //tableLayoutPanel1.ColumnCount=itemsPerRow + itemsPerRow/itemsPerHGrp-1; // items/(itemspergrp)=groups. 3 groups=2 spaces


            int col,row;
            for (int i = 0; i < numChannels; i++)
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



            /*foreach (SingleSlider slide in this.tableLayoutPanel1.Controls)
            {
                slide.Value = 0;
                m_sliders.Add(slide);
                if (Labels == null) continue;
                if (Labels.Length >= slide.Channel)
                    slide.textBox1.Text = Labels[slide.Channel-1];
            }*/

            m_sliders.Sort(delegate(SingleSlider a, SingleSlider b) { return a.Channel.CompareTo(b.Channel); });
        }

        public void setLevel(int channel, byte value)
        {
            if (channel < 1) throw new ArgumentOutOfRangeException("channel"); 
            if (value<0 || value >255 ) throw new ArgumentOutOfRangeException("value");
            
            if (channel < m_sliders.Count)
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

        private void setupPatch()
        {
            Patchlist.Clear();
            for (int i = 1; i <= 512; i++)
            {
                Patchlist.Add(i);
                Level.Add(255);
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
            setLevel(channel, value);
        }
        private void singleSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            int channel = (sender as SingleSlider).Channel;
            byte value = (sender as SingleSlider).Value;
            setLevel(channel, value);
        }

        private void updateValuesWithPatchList(int chan,byte value)
        {
            
            int i=0;
             //update dimmerList based onIsFollowTimeChanged the patch list
            i=Patchlist.IndexOf(chan, i);
            while (i != -1)
            {
                mValues[i] = (byte)(value*Level[i]/255);
                i = Patchlist.IndexOf(chan, i+1);
            }
            onValuesChanged();
        }

        private void SliderGroup_Resize(object sender, EventArgs e)
        {
            int rows = numChannels/itemsPerRow;
            int cols = itemsPerRow+ itemsPerRow/itemsPerHGrp-1;
            
            foreach(SingleSlider s in m_sliders)
            {
                s.Width = this.Width / cols - s.Margin.Left - s.Margin.Right;
                s.Left = ((s.Channel - 1)%itemsPerRow+ ((s.Channel-1)%itemsPerRow/itemsPerHGrp)) * (s.Width + s.Margin.Left+s.Margin.Right) + s.Margin.Left;
            }
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
