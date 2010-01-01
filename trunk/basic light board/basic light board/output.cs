using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VPB;

namespace basic_light_board
{
    public partial class output : Form
    {
        public List<VerticalProgressBar> m_Bars;
        public const int barW = 10;
        public const int barH = 100;
        public const int barM = 3;
        public int m_num;
        public output(int num)
        {
            m_num=num;
            InitializeComponent();
            m_Bars = new List<VerticalProgressBar>(m_num);
            VerticalProgressBar temp;
            for (int i = 0; i < m_num; i++) 
            {
                temp = new VerticalProgressBar();
                temp.Top = 0;
                temp.Width = barW;
                temp.Height=barH;
                temp.Left = (barW + barM) * i;
                temp.Value = 0;
                temp.Maximum = 255;
                temp.Style = Styles.Solid;

                m_Bars.Add(temp);
                this.Controls.Add(temp);
            }
        }
    }
}
