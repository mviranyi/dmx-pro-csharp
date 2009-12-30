using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
        byte[] LiveLevels = new byte[24];
        byte[] XLevels = new byte[24];
        byte[] YLevels = new byte[24];

        public Form1()
        {
            InitializeComponent();
        }

        private void updateTextBox()
        {
            StringBuilder str = new StringBuilder();
            int i = 1;
            foreach (byte b in LiveLevels)
            {
                str.AppendFormat("Ch{0,2}:{1,4} ", i++, b);
                if (i!=0 && (i-1) % 6 == 0) str.AppendLine();
            }
            textBox1.Text = str.ToString();
        }

        private void FullScale(byte[] Live, byte[] X, byte[] Y, byte xLev,byte yLev)
        {
            int max = X.Length;
            for (int i = 0; i < max; i++)
            {
                Live[i] = scale(X[i], Y[i], xLev,yLev);
            }
            updateTextBox();
        }
               

        private byte scale(byte Xval, byte Yval, byte XLevel,byte Ylevel)
        {
            byte temp =(byte)Math.Round(((Xval * (XLevel / 255.0) + Yval * (Ylevel / 255.0))));
            if (temp>255) return 255;
            return temp;
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
            goTime.Interval = (int)((((int)numericUpDown1.Value)) / 255.0);
            goTime.Tick += new EventHandler(goTime_Tick);
            if (crossFaders1.CrossFaderValue == 0) goTime.Tag = "up";
            else goTime.Tag = "down";
            button1.Enabled = false;
            goTime.Start();

        }

        void goTime_Tick(object sender, EventArgs e)
        {
            if (!(sender is Timer)) return;
            Timer t = (sender as Timer);

            if (((string)t.Tag) == "up") crossFaders1.CrossFaderValue++;
            else crossFaders1.CrossFaderValue--;
            if (crossFaders1.CrossFaderValue  == 0 || crossFaders1.CrossFaderValue == 255)
            {
                button1.Enabled = true;
                t.Stop();
            }
        }
    }
}
