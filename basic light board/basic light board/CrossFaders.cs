using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace basic_light_board
{
    public partial class CrossFaders : UserControl
    {
        public event EventHandler ValueChanged;

        public byte Scene1Value {
            set
            {
                if (value != Scene1Slider.Value)
                    Scene1Slider.Value = value;
            }
            get
            {
                return (byte)Scene1Slider.Value;
            }
        }
        public byte Scene2Value
        {
            set
            {
                if (value != Scene2Slider.Value)
                    Scene2Slider.Value = value;
            }
            get
            {
                return (byte)Scene2Slider.Value ;
            }
        }
        public byte CrossFaderValue
        {
            set
            {
                if (value != CrossfaderSlider.Value)
                    CrossfaderSlider.Value = value;
            }
            get
            {
                return (byte)CrossfaderSlider.Value;
            }
        }

        private bool centralSliderActive;

        

        public CrossFaders()
        {
            InitializeComponent();

            Binding Bnd = new Binding("Value", label1, "Text");
            Bnd.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            Scene1Slider.DataBindings.Add(Bnd);
            Bnd = new Binding("Value", label2, "Text");
            Bnd.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            Scene2Slider.DataBindings.Add(Bnd);           
            centralSliderActive = false;
            CrossfaderSlider.Value = 0;

            
        }

        private void colorSlider1_ValueChanged(object sender, EventArgs e)
        {
            Scene1Value = (byte)Scene1Slider.Value;
            if (!centralSliderActive)
                OnValueChanged();
        }

        private void colorSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            Scene1Value = (byte)Scene1Slider.Value;
            if (!centralSliderActive)
                OnValueChanged();
        }

        private void colorSlider2_ValueChanged(object sender, EventArgs e)
        {
            Scene2Value = (byte)Scene2Slider.Value;
            if (!centralSliderActive)
                OnValueChanged();
        }

        private void Scene2Slider_Scroll(object sender, ScrollEventArgs e)
        {
            Scene2Value = (byte)Scene2Slider.Value;
            if (!centralSliderActive)
                OnValueChanged();
        }

        private void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, new EventArgs());
        }

        private void colorSlider3_Scroll(object sender, ScrollEventArgs e)
        {
            centralSliderActive = true;
            CrossFaderValue = (byte)CrossfaderSlider.Value;
            Scene1Value = CrossFaderValue;
            Scene2Value = (byte)(255 - CrossFaderValue);
            centralSliderActive = false;
            OnValueChanged();
        }

        private void colorSlider3_ValueChanged(object sender, EventArgs e)
        {
            centralSliderActive = true;
            CrossFaderValue = (byte)CrossfaderSlider.Value;
            Scene1Value = CrossFaderValue;
            Scene2Value = (byte)(255 - CrossFaderValue);
            centralSliderActive = false;
            OnValueChanged();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CrossfaderSlider.Enabled = !(sender as CheckBox).Checked;
        }

        
    }
}
