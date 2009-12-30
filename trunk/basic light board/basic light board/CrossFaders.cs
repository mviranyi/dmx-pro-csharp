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

        public byte Scene1Value { set; get; }
        public byte Scene2Value { set; get; }
        public byte CrossFaderValue { set; get; }


        private bool centralSliderActive;

        public CrossFaders()
        {
            InitializeComponent();
            Binding Bnd1 = new Binding("Value", label1, "Text");
            Bnd1.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            Scene1Slider.DataBindings.Add(Bnd1);
            Binding Bnd2 = new Binding("Value", label2, "Text");
            Bnd2.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            Scene2Slider.DataBindings.Add(Bnd2);


            
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
            Scene2Value = (byte)Scene1Slider.Value;
            if (!centralSliderActive)
                OnValueChanged();
        }

        private void Scene2Slider_Scroll(object sender, ScrollEventArgs e)
        {
            Scene2Value = (byte)Scene1Slider.Value;
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
            Scene1Slider.Value = CrossfaderSlider.Value;
            Scene2Slider.Value = 255 - CrossfaderSlider.Value;
            CrossFaderValue = (byte)CrossfaderSlider.Value;
            centralSliderActive = false;
            OnValueChanged();
        }

        private void colorSlider3_ValueChanged(object sender, EventArgs e)
        {
            centralSliderActive = true;
            Scene1Slider.Value = CrossfaderSlider.Value;
            Scene2Slider.Value = 255 - CrossfaderSlider.Value;
            CrossFaderValue = (byte)CrossfaderSlider.Value;
            centralSliderActive = false;
            OnValueChanged();
        }

        
    }
}
