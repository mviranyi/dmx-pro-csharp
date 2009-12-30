namespace basic_light_board
{
    partial class CrossFaders 
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.Scene1Slider = new MB.Controls.ColorSlider();
            this.Scene2Slider = new MB.Controls.ColorSlider();
            this.CrossfaderSlider = new MB.Controls.ColorSlider();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.Scene1Slider, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Scene2Slider, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.CrossfaderSlider, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBox1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(136, 156);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(106, 137);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.MinimumSize = new System.Drawing.Size(27, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "0";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 137);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.MinimumSize = new System.Drawing.Size(27, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "255";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(36, 137);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(61, 16);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Disable";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Scene1Slider
            // 
            this.Scene1Slider.BackColor = System.Drawing.Color.Transparent;
            this.Scene1Slider.BarInnerColor = System.Drawing.Color.Crimson;
            this.Scene1Slider.BarOuterColor = System.Drawing.Color.Red;
            this.Scene1Slider.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.Scene1Slider.ColorSchema = MB.Controls.ColorSlider.ColorSchemas.PerlRoyalColors;
            this.Scene1Slider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Scene1Slider.ElapsedInnerColor = System.Drawing.Color.Violet;
            this.Scene1Slider.ElapsedOuterColor = System.Drawing.Color.DarkViolet;
            this.Scene1Slider.invertDirection = true;
            this.Scene1Slider.LargeChange = ((uint)(5u));
            this.Scene1Slider.Location = new System.Drawing.Point(3, 3);
            this.Scene1Slider.Maximum = 255;
            this.Scene1Slider.Name = "Scene1Slider";
            this.Scene1Slider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.Scene1Slider.Size = new System.Drawing.Size(27, 128);
            this.Scene1Slider.SmallChange = ((uint)(1u));
            this.Scene1Slider.TabIndex = 0;
            this.Scene1Slider.Text = "colorSlider1";
            this.Scene1Slider.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.Scene1Slider.Value = 255;
            this.Scene1Slider.ValueChanged += new System.EventHandler(this.colorSlider1_ValueChanged);
            this.Scene1Slider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.colorSlider1_Scroll);
            // 
            // Scene2Slider
            // 
            this.Scene2Slider.BackColor = System.Drawing.Color.Transparent;
            this.Scene2Slider.BarInnerColor = System.Drawing.Color.Crimson;
            this.Scene2Slider.BarOuterColor = System.Drawing.Color.Red;
            this.Scene2Slider.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.Scene2Slider.ColorSchema = MB.Controls.ColorSlider.ColorSchemas.PerlRoyalColors;
            this.Scene2Slider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Scene2Slider.ElapsedInnerColor = System.Drawing.Color.Violet;
            this.Scene2Slider.ElapsedOuterColor = System.Drawing.Color.DarkViolet;
            this.Scene2Slider.invertDirection = false;
            this.Scene2Slider.LargeChange = ((uint)(5u));
            this.Scene2Slider.Location = new System.Drawing.Point(106, 3);
            this.Scene2Slider.Maximum = 255;
            this.Scene2Slider.Name = "Scene2Slider";
            this.Scene2Slider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.Scene2Slider.Size = new System.Drawing.Size(27, 128);
            this.Scene2Slider.SmallChange = ((uint)(1u));
            this.Scene2Slider.TabIndex = 1;
            this.Scene2Slider.Text = "colorSlider2";
            this.Scene2Slider.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.Scene2Slider.Value = 0;
            this.Scene2Slider.ValueChanged += new System.EventHandler(this.colorSlider2_ValueChanged);
            this.Scene2Slider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Scene2Slider_Scroll);
            // 
            // CrossfaderSlider
            // 
            this.CrossfaderSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CrossfaderSlider.BackColor = System.Drawing.Color.Transparent;
            this.CrossfaderSlider.BarInnerColor = System.Drawing.Color.Transparent;
            this.CrossfaderSlider.BarOuterColor = System.Drawing.Color.Transparent;
            this.CrossfaderSlider.BarPenColor = System.Drawing.Color.Transparent;
            this.CrossfaderSlider.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.CrossfaderSlider.DrawFocusRectangle = false;
            this.CrossfaderSlider.DrawSemitransparentThumb = false;
            this.CrossfaderSlider.ElapsedInnerColor = System.Drawing.Color.Transparent;
            this.CrossfaderSlider.ElapsedOuterColor = System.Drawing.Color.Transparent;
            this.CrossfaderSlider.invertDirection = true;
            this.CrossfaderSlider.LargeChange = ((uint)(5u));
            this.CrossfaderSlider.Location = new System.Drawing.Point(36, 3);
            this.CrossfaderSlider.Maximum = 255;
            this.CrossfaderSlider.MouseEffects = false;
            this.CrossfaderSlider.Name = "CrossfaderSlider";
            this.CrossfaderSlider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.CrossfaderSlider.Size = new System.Drawing.Size(64, 128);
            this.CrossfaderSlider.SmallChange = ((uint)(1u));
            this.CrossfaderSlider.TabIndex = 2;
            this.CrossfaderSlider.Text = "colorSlider3";
            this.CrossfaderSlider.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.CrossfaderSlider.ThumbSize = 40;
            this.CrossfaderSlider.ValueChanged += new System.EventHandler(this.colorSlider3_ValueChanged);
            this.CrossfaderSlider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.colorSlider3_Scroll);
            // 
            // CrossFaders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CrossFaders";
            this.Size = new System.Drawing.Size(136, 156);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MB.Controls.ColorSlider Scene1Slider;
        private MB.Controls.ColorSlider Scene2Slider;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private MB.Controls.ColorSlider CrossfaderSlider;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
