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
        public List<int> Patchlist = new List<int>(512);
        public List<byte> Level = new List<byte>(512);
        
        [Description("Event fires when the Value property changes")]
        [Category("Action")]
        public event EventHandler ValueChanged;

        public SliderGroup()
        {
            InitializeComponent();
            
            setupPatch();
            
            foreach (SingleSlider slide in this.tableLayoutPanel1.Controls)
            {
                slide.Value = 0;
            }
        }

        public void patch(int dimmer,int channel,byte maxLevel)
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
            int i=0;
            int channel = (sender as SingleSlider).Channel;
            byte value = (sender as SingleSlider).Value;

            updateValuesWithPatchList(channel, value);
                    
            if (ValueChanged != null) ValueChanged(this, new EventArgs());
        }

        private void singleSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            int channel = (sender as SingleSlider).Channel;
            byte value = (sender as SingleSlider).Value;

            updateValuesWithPatchList(channel, value);

            if (ValueChanged != null) ValueChanged(this, new EventArgs());
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
        }
    }
}
