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
        :this(LightingConstants.numChannelSliders,defaultBarW,defaultBarH,defaultBarM)
        {}

        
        public OutputLevels(int num,int w,int h,int m)
        {
            bool d = this.Dock == DockStyle.Fill;
            mNum=num;
            if (d)
            {
                mBarMargin = m;
                mBarHeight = this.Height - 25;
                mBarWidth = this.Width / mNum;
            }
            else
            {
                mBarWidth = w;
                mBarHeight = h;
                mBarMargin = m;
            }
            InitializeComponent();
            mBars = new List<VerticalProgressBar>(mNum);
            mLabels = new List<Label>(mNum);
            VerticalProgressBar temp;
            Label lbl;
            this.SuspendLayout();
            for (int i = 0; i < mNum; i++) 
            {
                temp = new VerticalProgressBar();
                temp.Top = 0;
                temp.Width = mBarWidth;
                temp.Height=mBarHeight;
                temp.Left = (this.Width + mBarMargin) * i / mNum;
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
            this.ResumeLayout(false);
        }
        public void updateLevels(byte[] levels)
        {
            if (levels.Length != LightingConstants.maxDimmers)
                throw new ArgumentException("levels does not have the right number of elements");
            this.SuspendLayout();
            int i;
            for (i = 0; i < mNum ; i++)
            {
                mBars[i].PaintingSuspended = true;
            }
            for (i = 0; i < mNum ; i++)
            {
                mBars[i].Value = levels[i];
            }
            for (i = 0; i < mNum; i++)
            {
                mBars[i].PaintingSuspended = false;
            }
            this.ResumeLayout(false);
        }

        private void OutputLevels_Resize(object sender, EventArgs e)
        {
            bool d = this.Dock == DockStyle.Fill;
            if (d)
            {
                mBarMargin = 0;
                mBarHeight = (this.Height) - 25;
                mBarWidth = (this.Width) / mNum ;
            }
            else
            {
                mBarWidth = defaultBarW;
                mBarHeight = defaultBarH;
                mBarMargin = defaultBarM ;
            }
            VerticalProgressBar temp;
            Label lbl;
            this.SuspendLayout();
            for (int i = 0; i < mNum; i++)
            {
                temp = mBars[i];
                if (temp == null) continue;
                temp.Width = mBarWidth;
                temp.Height = mBarHeight;
                temp.Left = (this.Width + mBarMargin) * i/ mNum ;

                lbl = mLabels[i];
                if (lbl==null) continue;
                lbl.Top = temp.Bottom + lbl.Margin.Top;
                lbl.Left = temp.Left;
                lbl.Width = mBarWidth;
            }
            this.ResumeLayout(false);
        }
    }
}
