namespace NsDemo
{
    partial class DataView
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonUPH = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonNG = new System.Windows.Forms.Button();
            this.panelDisp = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelDisp, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.22222F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 77.77778F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1222, 702);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1216, 149);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonUPH);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(602, 143);
            this.panel1.TabIndex = 0;
            // 
            // buttonUPH
            // 
            this.buttonUPH.BackColor = System.Drawing.Color.Transparent;
            this.buttonUPH.BackgroundImage = global::NsDemo.Properties.Resources.btn;
            this.buttonUPH.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonUPH.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonUPH.FlatAppearance.BorderSize = 0;
            this.buttonUPH.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonUPH.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonUPH.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUPH.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonUPH.Location = new System.Drawing.Point(238, 34);
            this.buttonUPH.Name = "buttonUPH";
            this.buttonUPH.Size = new System.Drawing.Size(200, 70);
            this.buttonUPH.TabIndex = 0;
            this.buttonUPH.Text = "UPH/CT";
            this.buttonUPH.UseVisualStyleBackColor = false;
            this.buttonUPH.Click += new System.EventHandler(this.buttonUPH_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonNG);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(611, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(602, 143);
            this.panel2.TabIndex = 1;
            // 
            // buttonNG
            // 
            this.buttonNG.BackColor = System.Drawing.Color.Transparent;
            this.buttonNG.BackgroundImage = global::NsDemo.Properties.Resources.btn;
            this.buttonNG.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonNG.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonNG.FlatAppearance.BorderSize = 0;
            this.buttonNG.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonNG.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonNG.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNG.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonNG.Location = new System.Drawing.Point(117, 34);
            this.buttonNG.Name = "buttonNG";
            this.buttonNG.Size = new System.Drawing.Size(200, 70);
            this.buttonNG.TabIndex = 0;
            this.buttonNG.Text = "Tossing/NG";
            this.buttonNG.UseVisualStyleBackColor = false;
            this.buttonNG.Click += new System.EventHandler(this.buttonNG_Click);
            // 
            // panelDisp
            // 
            this.panelDisp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDisp.Location = new System.Drawing.Point(3, 158);
            this.panelDisp.Name = "panelDisp";
            this.panelDisp.Size = new System.Drawing.Size(1216, 541);
            this.panelDisp.TabIndex = 1;
            // 
            // DataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1222, 702);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DataView";
            this.Text = "Data";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button buttonUPH;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonNG;
        private System.Windows.Forms.Panel panelDisp;

    }
}