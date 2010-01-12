namespace basic_light_board
{
    partial class SingleSlider
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.channelLabel = new System.Windows.Forms.Label();
            this.percentLabel = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.mainSlider = new MB.Controls.ColorSlider();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(3, 197);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 24);
            this.button1.TabIndex = 1;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button1_MouseDown);
            this.button1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button1_MouseUp);
            // 
            // channelLabel
            // 
            this.channelLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.channelLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.channelLabel.Location = new System.Drawing.Point(3, 23);
            this.channelLabel.Name = "channelLabel";
            this.channelLabel.Size = new System.Drawing.Size(25, 23);
            this.channelLabel.TabIndex = 2;
            this.channelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // percentLabel
            // 
            this.percentLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.percentLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.percentLabel.Location = new System.Drawing.Point(3, 171);
            this.percentLabel.Name = "percentLabel";
            this.percentLabel.Size = new System.Drawing.Size(25, 23);
            this.percentLabel.TabIndex = 3;
            this.percentLabel.Text = "0";
            this.percentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(31, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // mainSlider
            // 
            this.mainSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mainSlider.BackColor = System.Drawing.Color.Transparent;
            this.mainSlider.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.mainSlider.invertDirection = true;
            this.mainSlider.LargeChange = ((uint)(64u));
            this.mainSlider.Location = new System.Drawing.Point(3, 49);
            this.mainSlider.Maximum = 255;
            this.mainSlider.MouseWheelBarPartitions = 16;
            this.mainSlider.Name = "mainSlider";
            this.mainSlider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.mainSlider.Size = new System.Drawing.Size(25, 119);
            this.mainSlider.SmallChange = ((uint)(1u));
            this.mainSlider.TabIndex = 0;
            this.mainSlider.Text = "colorSlider1";
            this.mainSlider.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.mainSlider.Value = 0;
            this.mainSlider.ValueChanged += new System.EventHandler(this.mainSlider_ValueChanged);
            this.mainSlider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.mainSlider_Scroll);
            // 
            // SingleSlider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.percentLabel);
            this.Controls.Add(this.channelLabel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mainSlider);
            this.Name = "SingleSlider";
            this.Size = new System.Drawing.Size(31, 224);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MB.Controls.ColorSlider mainSlider;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label channelLabel;
        private System.Windows.Forms.Label percentLabel;
        public System.Windows.Forms.TextBox textBox1;
    }
}
