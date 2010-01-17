using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using VPB;

namespace basic_light_board
{
    public partial class OutputLevels : UserControl
    {
        private const int defaultChannels = 96;
        private const int defaultBarW=10;
        private const int defaultBarH = 100;
        private const int defaultBarM = 3;

        private int mNum;
        private int mBarWidth;
        private int mBarHeight;
        private int mBarMargin;

        public List<VerticalProgressBar> mBars;
        public List<Label> mLabels;

        public OutputLevels()
        :this(defaultChannels,defaultBarW,defaultBarH,defaultBarM)
        {}

        
        public OutputLevels(int num,int w,int h,int m)
        {
            mNum=num;
            mBarWidth = w;
            mBarHeight = h;
            mBarMargin = m;
            InitializeComponent();
            mBars = new List<VerticalProgressBar>(mNum);
            mLabels = new List<Label>(mNum);
            VerticalProgressBar temp;
            Label lbl;
            for (int i = 0; i < mNum; i++) 
            {
                temp = new VerticalProgressBar();
                temp.Top = 0;
                temp.Width = mBarWidth;
                temp.Height=mBarHeight;
                temp.Left = (mBarWidth + mBarMargin) * i;
                temp.Value = 0;
                temp.Maximum = 255;
                temp.Style = Styles.Solid;

                lbl = new Label();
                lbl.Text = string.Format("{0}", i + 1);
                lbl.Top = temp.Bottom + lbl.Margin.Top;
                lbl.Left = temp.Left;
                lbl.Width = mBarWidth;
                lbl.Height = 25;
                mLabels.Add(lbl);
                mBars.Add(temp);
                this.Controls.Add(temp);
                this.Controls.Add(lbl);
            }
        }
    }
}
