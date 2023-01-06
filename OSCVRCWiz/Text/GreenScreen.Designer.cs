namespace OSCVRCWiz
{
    partial class GreenScreen
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.customrtb1 = new OSCVRCWiz.RJControls.CustomRTB();
            this.translucentPanel1 = new OSCVRCWiz.RJControls.TranslucentPanel();
            this.translucentPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // customrtb1
            // 
            this.customrtb1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customrtb1.BackColor = System.Drawing.Color.Black;
            this.customrtb1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.customrtb1.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.customrtb1.ForeColor = System.Drawing.Color.White;
            this.customrtb1.Location = new System.Drawing.Point(12, 12);
            this.customrtb1.Name = "customrtb1";
            this.customrtb1.ReadOnly = true;
            this.customrtb1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.customrtb1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.customrtb1.Size = new System.Drawing.Size(951, 426);
            this.customrtb1.TabIndex = 1;
            this.customrtb1.Text = "Use OBS chroma key filter set to green";
            // 
            // translucentPanel1
            // 
            this.translucentPanel1.BackColor = System.Drawing.Color.Transparent;
            this.translucentPanel1.Controls.Add(this.customrtb1);
            this.translucentPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.translucentPanel1.Location = new System.Drawing.Point(0, 0);
            this.translucentPanel1.Name = "translucentPanel1";
            this.translucentPanel1.Size = new System.Drawing.Size(975, 450);
            this.translucentPanel1.TabIndex = 2;
            // 
            // PopupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 450);
            this.Controls.Add(this.translucentPanel1);
            this.Name = "PopupForm";
            this.Text = "TTS Voice Wizard Text Overlay ";
            this.translucentPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private RJControls.TranslucentPanel translucentPanel1;
        public RJControls.CustomRTB customrtb1;
    }
}