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
        public byte[] Values = new byte[513];
        public static List<int> Patchlist = new List<int>(512);
        public static List<byte> Level = new List<byte>(512);

        public List<int> m_PatchList { get { return Patchlist; } set { Patchlist = value; } }
        private List<SingleSlider> m_sliders;
        
        [Description("Event fires when the Value property changes")]
        [Category("Action")]
        public event EventHandler ValueChanged;

        public SliderGroup()
        {
            InitializeComponent();
            
            setupPatch();
            
            m_sliders=new List<SingleSlider>();
            foreach (SingleSlider slide in this.tableLayoutPanel1.Controls)
            {
                slide.Value = 0;
                m_sliders.Add(slide );
            }
        }

        public void setLevel(int channel, byte value)
        {
            if (channel < m_sliders.Count)
            {
                m_sliders[channel-1].Value = value;
                return;
            }
            updateValuesWithPatchList(channel, value);
        }

        public static void patch(int dimmer, int channel, byte maxLevel)
        {
            Patchlist[dimmer]=channel;
            Level[dimmer]=maxLevel;
        }

        private void setupPatch()
        {
            Patchlist.Clear();
            for (int i = 0; i < 512; i++)
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
