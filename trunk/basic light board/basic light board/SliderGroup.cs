using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace basic_light_board
{
    public partial class SliderGroup : UserControl
    {
        
        /// <summary>
        /// Values contains one entry for ever dimmer
        /// Values[0] is the value of dimmer 1 (0-255)
        /// </summary>
        public byte[] Values = new byte[512];

        
        /// <summary>
        ///Patchlist contains one entry for every dimmer
        ///Patchlist[0] contains the channel that dimmer 1 is patched into. (1-> anything)
        /// </summary>
        public static List<int> Patchlist=new List<int>();

        
        /// <summary>
        ///Level contains one entry for every dimmer
        ///Level[0] contains the maxLevel that dimmer 1 can achieve (0->255)
        /// </summary>
        public static List<byte> Level = new List<byte>();

        //public List<int> m_PatchList { get { return Patchlist; } set { Patchlist = value; } }
        private List<SingleSlider> m_sliders;
        
        [Description("Event fires when the Value property changes")]
        [Category("Action")]
        public event EventHandler ValueChanged;

        public SliderGroup()
        {
            InitializeComponent();

            if (Patchlist.Count==0) setupPatch();
            
            m_sliders=new List<SingleSlider>();
            foreach (SingleSlider slide in this.tableLayoutPanel1.Controls)
            {
                slide.Value = 0;
                m_sliders.Add(slide );
            }
        }

        public void setLevel(int channel, byte value)
        {
            if (channel < 1) throw new ArgumentOutOfRangeException("channel"); 
            if (value<0 || value >255 ) throw new ArgumentOutOfRangeException("value");
            
            if (channel < m_sliders.Count)
            {
                m_sliders[channel-1].Value = value;
                return;
            }
            updateValuesWithPatchList(channel, value);
        }

        public static void patch(int dimmer, int channel, byte maxLevel)
        {
            if (dimmer < 1 || dimmer > 512) throw new ArgumentOutOfRangeException("Dimmer)");
            if (channel < 1) throw new ArgumentOutOfRangeException("Channel");
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

        private void singleSlider1_ValueChanged(object sender, EventArgs e)
        {
            int channel = (sender as SingleSlider).Channel;
            byte value = (sender as SingleSlider).Value;

            updateValuesWithPatchList(channel, value);
        }

        private void singleSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            int channel = (sender as SingleSlider).Channel;
            byte value = (sender as SingleSlider).Value;

            updateValuesWithPatchList(channel, value);
        }

        private void updateValuesWithPatchList(int chan,byte value)
        {
            int i=0;
             //update dimmerList based on the patch list
            i=Patchlist.IndexOf(chan, i);
            while (i != -1)
            {
                Values[i] = (byte)(value*Level[i]/255);
                i = Patchlist.IndexOf(chan, i+1);
            }
            if (ValueChanged != null) ValueChanged(this, new EventArgs());
        }
    }
}
