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
        public byte[] Values = new byte[24];
        
        [Description("Event fires when the Value property changes")]
        [Category("Action")]
        public event EventHandler ValueChanged;

        public SliderGroup()
        {
            InitializeComponent();
            foreach (SingleSlider slide in this.tableLayoutPanel1.Controls)
            {
                slide.Value = 0;
            }
        }

        private void singleSlider1_ValueChanged(object sender, EventArgs e)
        {
            Values[(sender as SingleSlider).Channel - 1] = (sender as SingleSlider).Value;
            if (ValueChanged != null) ValueChanged(this, new EventArgs());
        }

        private void singleSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            Values[(sender as SingleSlider).Channel - 1] = (sender as SingleSlider).Value;
            if (ValueChanged != null) ValueChanged(this, new EventArgs());
        }

    }
}
