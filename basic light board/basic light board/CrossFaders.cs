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
        public event EventHandler SceneChanged;

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

        public byte CurrentSceneValue
        {
            get
            {
                if (inLeftScene)
                    return Scene1Value;
                else
                    return Scene2Value;
            }
            set
            {
                if (inLeftScene) Scene1Value = value;
                else Scene2Value = value;
            }
        }

        public byte NextSceneValue
        {
            get
            {
                if (inLeftScene)
                    return Scene2Value;
                else
                    return Scene1Value;
            }
            set
            {
                if (inLeftScene) Scene2Value = value;
                else Scene1Value = value;
            }
        }


        private bool centralSliderActive;

        private bool inLeftScene = true;
        private bool inRightScene = false;

        

        public CrossFaders()
        {
            InitializeComponent();
            checkScene();

            Binding Bnd = new Binding("Value", label1, "Text");
            Bnd.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            Scene1Slider.DataBindings.Add(Bnd);
            Bnd = new Binding("Value", label2, "Text");
            Bnd.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            Scene2Slider.DataBindings.Add(Bnd);           
            centralSliderActive = false;
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
            checkScene();

        }

        private void onSceneChange()
        {
            if (inLeftScene)
                Console.WriteLine("now in Left Scene");
            else
                Console.WriteLine("now in Right Scene");
            if (SceneChanged != null) SceneChanged(this, new EventArgs());
        }

        private void checkScene()
        {
            if (Scene1Value == 255 && Scene2Value == 0 && inRightScene)
            {
                inLeftScene = true;
                inRightScene = false;
                lblLeft.Text = "Current";
                lblRight.Text = "Next";
                onSceneChange();
            }
            if (Scene1Value == 0 && Scene2Value == 255 && inLeftScene)
            {
                inLeftScene = false;
                inRightScene = true;
                lblLeft.Text = "Next";
                lblRight.Text = "Current";
                
                onSceneChange();
            }
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
