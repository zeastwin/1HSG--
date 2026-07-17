namespace NsDemo.RFIDSys
{
    partial class RFIDForm
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
            this.uS_RFID1 = new NsDemo.RFIDSys.US_RFID();
            this.SuspendLayout();
            // 
            // uS_RFID1
            // 
            this.uS_RFID1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uS_RFID1.Location = new System.Drawing.Point(0, 0);
            this.uS_RFID1.Name = "uS_RFID1";
            this.uS_RFID1.Size = new System.Drawing.Size(1203, 805);
            this.uS_RFID1.TabIndex = 0;
            // 
            // RFIDForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1203, 805);
            this.Controls.Add(this.uS_RFID1);
            this.Name = "RFIDForm";
            this.Text = "RFIDForm";
            this.ResumeLayout(false);

        }

        #endregion

        private US_RFID uS_RFID1;
    }
}