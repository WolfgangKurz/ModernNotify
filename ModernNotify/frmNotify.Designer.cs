namespace ModernNotify
{
    partial class frmNotify
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
            this.components = new System.ComponentModel.Container();
            this.tmrEnter = new System.Windows.Forms.Timer(this.components);
            this.tmrClose = new System.Windows.Forms.Timer(this.components);
            this.tmrScale = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmrEnter
            // 
            this.tmrEnter.Enabled = true;
            this.tmrEnter.Interval = 10;
            this.tmrEnter.Tick += new System.EventHandler(this.tmrEnter_Tick);
            // 
            // tmrClose
            // 
            this.tmrClose.Interval = 6000;
            this.tmrClose.Tick += new System.EventHandler(this.tmrClose_Tick);
            // 
            // tmrScale
            // 
            this.tmrScale.Enabled = true;
            this.tmrScale.Interval = 20;
            this.tmrScale.Tick += new System.EventHandler(this.tmrScale_Tick);
            // 
            // frmNotify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(294, 124);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("맑은 고딕", 11.25F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmNotify";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "frmNotify";
            this.TopMost = true;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmNotify_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmNotify_MouseDown);
            this.MouseEnter += new System.EventHandler(this.frmNotify_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.frmNotify_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmNotify_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmNotify_MouseUp);
            this.Resize += new System.EventHandler(this.frmNotify_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer tmrEnter;
        private System.Windows.Forms.Timer tmrClose;
        private System.Windows.Forms.Timer tmrScale;
    }
}