using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace basic_light_board
{
    public partial class SingleSlider : UserControl, basic_light_board.ISingleSlider
    {
        [Description("cueChanged fires when the Label property changes")]
        [Category("Action")]
        public event EventHandler LabelChanged;


        [Description("cueChanged fires when the Value property changes")]
        [Category("Action")]
        public event EventHandler ValueChanged;


        [Description("cueChanged fires when the Slider position is changed")]
        [Category("Behavior")]
        public new event ScrollEventHandler Scroll;

        private byte preBumpVal;
        private int _channel;
        public byte Value { 
            get { return (byte)mainSlider.Value; } 
            set { 
                mainSlider.Value = (int)value; 
                if (ValueChanged!=null) 
                    ValueChanged(this, EventArgs.Empty); 
            } 
        }
        public int Channel {
            get {return _channel;}
            set {_channel=value;channelLabel.Text=_channel.ToString();}}

        public SingleSlider() :this(0)
        {}

        public SingleSlider(int ch)
        {
            InitializeComponent();
            Channel = ch;
            Value = 0;
        }

        private void updateLabel()
        {
            //percentLabel.Text = ((int)(Value / 255.0 * 100.0)).ToString();
            percentLabel.Text = Value.ToString();
        }

        private void mainSlider_Scroll(object sender, ScrollEventArgs e)
        {
            updateLabel(); ;
            if (Scroll!=null) Scroll(this, e);
        }

        private void mainSlider_ValueChanged(object sender, EventArgs e)
        {
            updateLabel();
            if (ValueChanged != null) ValueChanged(this, e);
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            preBumpVal = Value;
            mainSlider.Value = 255;
            if (ValueChanged != null) ValueChanged(this, new EventArgs());
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            mainSlider.Value=preBumpVal ;
            if (ValueChanged != null) ValueChanged(this, new EventArgs());
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (LabelChanged != null) LabelChanged(this, new EventArgs());
        }

        public override string ToString()
        {
            return string.Format("{0,4}-[{1}]: {2}", this.Channel, this.textBox1.Text, this.Value);
        }

        private void SingleSlider_Enter(object sender, EventArgs e)
        {
            this.mainSlider.Select();
        }

    }
}
