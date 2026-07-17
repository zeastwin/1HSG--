namespace NsDemo
{
    partial class LogView
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
            this.rtLog1 = new RTLog.RTLog();
            this.SuspendLayout();
            // 
            // rtLog1
            // 
            this.rtLog1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(234)))), ((int)(((byte)(248)))));
            this.rtLog1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtLog1.Location = new System.Drawing.Point(0, 0);
            this.rtLog1.Name = "rtLog1";
            this.rtLog1.Size = new System.Drawing.Size(1041, 649);
            this.rtLog1.TabIndex = 0;
            // 
            // LogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1041, 649);
            this.Controls.Add(this.rtLog1);
            this.Name = "LogView";
            this.Text = "LogInfo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogView_FormClosing);
            this.Load += new System.EventHandler(this.LogView_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private RTLog.RTLog rtLog1;
    }
}