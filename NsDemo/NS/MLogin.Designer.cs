namespace NsDemo
{
    partial class MLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MLogin));
            this.loginManager1 = new NLogin.LoginManager();
            this.SuspendLayout();
            // 
            // loginManager1
            // 
            this.loginManager1.BackColor = System.Drawing.Color.Transparent;
            this.loginManager1.Location = new System.Drawing.Point(-3, -1);
            this.loginManager1.Name = "loginManager1";
            this.loginManager1.Size = new System.Drawing.Size(446, 258);
            this.loginManager1.TabIndex = 0;
            this.loginManager1.Tag = "9999";
            this.loginManager1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.loginManager1_MouseMove);
            // 
            // MLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(439, 248);
            this.Controls.Add(this.loginManager1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "9999";
            this.Text = "用户登录";
            this.TransparencyKey = System.Drawing.Color.Gray;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MLogin_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private NLogin.LoginManager loginManager1;
    }
}