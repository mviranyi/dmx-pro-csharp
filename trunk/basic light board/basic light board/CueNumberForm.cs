using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace basic_light_board
{
    public partial class CueNumberForm : Form
    {
        public int CueNum
        {
            get;
            set;
        }
        public string CueName
        {
            get;
            set;
        }
        public CueNumberForm()
        {
            InitializeComponent();
            Binding b = new Binding("CueNum", numericUpDown1, "Value");
            b.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            b.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
            Binding c = new Binding("CueName", textBox1, "Text");
            c.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            c.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
            this.DataBindings.Add(b);
            this.DataBindings.Add(c);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        
    }
}
