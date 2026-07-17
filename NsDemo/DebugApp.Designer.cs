namespace NsDemo
{
    partial class DebugApp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugApp));
            this.btnFrmFinger = new System.Windows.Forms.Button();
            this.ReceiveBox = new System.Windows.Forms.RichTextBox();
            this.SendBox = new System.Windows.Forms.RichTextBox();
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnUpLoad = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRePOST = new System.Windows.Forms.Button();
            this.btnOutPut = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFrmFinger
            // 
            this.btnFrmFinger.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFrmFinger.Location = new System.Drawing.Point(12, 12);
            this.btnFrmFinger.Name = "btnFrmFinger";
            this.btnFrmFinger.Size = new System.Drawing.Size(123, 46);
            this.btnFrmFinger.TabIndex = 0;
            this.btnFrmFinger.Text = "指纹配置";
            this.btnFrmFinger.UseVisualStyleBackColor = true;
            this.btnFrmFinger.Click += new System.EventHandler(this.btnFrmFinger_Click);
            // 
            // ReceiveBox
            // 
            this.ReceiveBox.BackColor = System.Drawing.Color.White;
            this.ReceiveBox.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ReceiveBox.Location = new System.Drawing.Point(6, 20);
            this.ReceiveBox.Name = "ReceiveBox";
            this.ReceiveBox.ReadOnly = true;
            this.ReceiveBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.ReceiveBox.Size = new System.Drawing.Size(516, 217);
            this.ReceiveBox.TabIndex = 38;
            this.ReceiveBox.Text = "";
            // 
            // SendBox
            // 
            this.SendBox.BackColor = System.Drawing.Color.White;
            this.SendBox.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SendBox.Location = new System.Drawing.Point(6, 243);
            this.SendBox.Name = "SendBox";
            this.SendBox.ReadOnly = true;
            this.SendBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.SendBox.Size = new System.Drawing.Size(516, 134);
            this.SendBox.TabIndex = 39;
            this.SendBox.Text = "";
            // 
            // btnQuery
            // 
            this.btnQuery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnQuery.Font = new System.Drawing.Font("黑体", 13F);
            this.btnQuery.Location = new System.Drawing.Point(366, 383);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 31);
            this.btnQuery.TabIndex = 40;
            this.btnQuery.Text = "查询";
            this.btnQuery.UseVisualStyleBackColor = false;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnUpLoad
            // 
            this.btnUpLoad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnUpLoad.Font = new System.Drawing.Font("黑体", 13F);
            this.btnUpLoad.Location = new System.Drawing.Point(447, 383);
            this.btnUpLoad.Name = "btnUpLoad";
            this.btnUpLoad.Size = new System.Drawing.Size(75, 31);
            this.btnUpLoad.TabIndex = 41;
            this.btnUpLoad.Text = "上传";
            this.btnUpLoad.UseVisualStyleBackColor = false;
            this.btnUpLoad.Click += new System.EventHandler(this.btnUpLoad_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ReceiveBox);
            this.groupBox1.Controls.Add(this.btnUpLoad);
            this.groupBox1.Controls.Add(this.SendBox);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Location = new System.Drawing.Point(12, 78);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(529, 419);
            this.groupBox1.TabIndex = 42;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mes测试";
            this.groupBox1.Visible = false;
            // 
            // btnRePOST
            // 
            this.btnRePOST.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnRePOST.Font = new System.Drawing.Font("黑体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRePOST.Location = new System.Drawing.Point(166, 12);
            this.btnRePOST.Name = "btnRePOST";
            this.btnRePOST.Size = new System.Drawing.Size(136, 46);
            this.btnRePOST.TabIndex = 40;
            this.btnRePOST.Text = "补传";
            this.btnRePOST.UseVisualStyleBackColor = false;
            this.btnRePOST.Click += new System.EventHandler(this.btnRePOST_Click);
            // 
            // btnOutPut
            // 
            this.btnOutPut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOutPut.Font = new System.Drawing.Font("黑体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOutPut.Location = new System.Drawing.Point(336, 12);
            this.btnOutPut.Name = "btnOutPut";
            this.btnOutPut.Size = new System.Drawing.Size(136, 46);
            this.btnOutPut.TabIndex = 43;
            this.btnOutPut.Text = "一键导出";
            this.btnOutPut.UseVisualStyleBackColor = false;
            this.btnOutPut.Click += new System.EventHandler(this.btnOutPut_Click);
            // 
            // DebugApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1264, 730);
            this.Controls.Add(this.btnOutPut);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnRePOST);
            this.Controls.Add(this.btnFrmFinger);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DebugApp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "9999";
            this.Text = "调试界面";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainView_FormClosing);
            this.Load += new System.EventHandler(this.MainView_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnFrmFinger;
        public System.Windows.Forms.RichTextBox ReceiveBox;
        public System.Windows.Forms.RichTextBox SendBox;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnUpLoad;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRePOST;
        private System.Windows.Forms.Button btnOutPut;
    }
}