namespace NsDemo
{
    partial class AlamerView
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.dataGridViewAlamer = new System.Windows.Forms.DataGridView();
			this.日期 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.time = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.报警代码 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.报警位置 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.报警信息 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.持续时间 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.textBoxPath = new System.Windows.Forms.TextBox();
			this.dateTimePickerAlamer = new System.Windows.Forms.DateTimePicker();
			this.buttonLogFlush = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.buttonSavePath = new System.Windows.Forms.Button();
			this.buttonLoad = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewAlamer)).BeginInit();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.dataGridViewAlamer, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 81.46067F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.53933F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1191, 712);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// dataGridViewAlamer
			// 
			this.dataGridViewAlamer.AllowUserToAddRows = false;
			this.dataGridViewAlamer.AllowUserToResizeColumns = false;
			this.dataGridViewAlamer.BackgroundColor = System.Drawing.Color.White;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridViewAlamer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridViewAlamer.ColumnHeadersHeight = 30;
			this.dataGridViewAlamer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.日期,
            this.time,
            this.报警代码,
            this.报警位置,
            this.报警信息,
            this.持续时间});
			dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle8.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle8.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridViewAlamer.DefaultCellStyle = dataGridViewCellStyle8;
			this.dataGridViewAlamer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewAlamer.GridColor = System.Drawing.Color.White;
			this.dataGridViewAlamer.Location = new System.Drawing.Point(3, 3);
			this.dataGridViewAlamer.MultiSelect = false;
			this.dataGridViewAlamer.Name = "dataGridViewAlamer";
			this.dataGridViewAlamer.ReadOnly = true;
			this.dataGridViewAlamer.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle9.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle9.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridViewAlamer.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
			this.dataGridViewAlamer.RowHeadersVisible = false;
			dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.MenuHighlight;
			this.dataGridViewAlamer.RowsDefaultCellStyle = dataGridViewCellStyle10;
			this.dataGridViewAlamer.RowTemplate.Height = 23;
			this.dataGridViewAlamer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewAlamer.Size = new System.Drawing.Size(1185, 573);
			this.dataGridViewAlamer.TabIndex = 0;
			// 
			// 日期
			// 
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.日期.DefaultCellStyle = dataGridViewCellStyle2;
			this.日期.HeaderText = "日期";
			this.日期.Name = "日期";
			this.日期.ReadOnly = true;
			this.日期.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.日期.Width = 150;
			// 
			// time
			// 
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.time.DefaultCellStyle = dataGridViewCellStyle3;
			this.time.HeaderText = "时间";
			this.time.Name = "time";
			this.time.ReadOnly = true;
			this.time.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.time.Width = 150;
			// 
			// 报警代码
			// 
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.报警代码.DefaultCellStyle = dataGridViewCellStyle4;
			this.报警代码.HeaderText = "报警代码";
			this.报警代码.Name = "报警代码";
			this.报警代码.ReadOnly = true;
			this.报警代码.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.报警代码.Width = 150;
			// 
			// 报警位置
			// 
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.报警位置.DefaultCellStyle = dataGridViewCellStyle5;
			this.报警位置.HeaderText = "报警位置";
			this.报警位置.Name = "报警位置";
			this.报警位置.ReadOnly = true;
			this.报警位置.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.报警位置.Width = 150;
			// 
			// 报警信息
			// 
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.报警信息.DefaultCellStyle = dataGridViewCellStyle6;
			this.报警信息.HeaderText = "报警信息";
			this.报警信息.Name = "报警信息";
			this.报警信息.ReadOnly = true;
			this.报警信息.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.报警信息.Width = 500;
			// 
			// 持续时间
			// 
			this.持续时间.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Menu;
			this.持续时间.DefaultCellStyle = dataGridViewCellStyle7;
			this.持续时间.HeaderText = "持续时间";
			this.持续时间.Name = "持续时间";
			this.持续时间.ReadOnly = true;
			this.持续时间.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.tableLayoutPanel2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 582);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1185, 127);
			this.panel1.TabIndex = 1;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 5;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 111F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 342F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 131F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
			this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.button1, 2, 0);
			this.tableLayoutPanel2.Controls.Add(this.textBoxPath, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.dateTimePickerAlamer, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.buttonLogFlush, 3, 0);
			this.tableLayoutPanel2.Controls.Add(this.panel2, 1, 2);
			this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 3;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(1185, 127);
			this.tableLayoutPanel2.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(20, 8);
			this.label1.Margin = new System.Windows.Forms.Padding(20, 8, 3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "Log保存路径：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(456, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "select";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// textBoxPath
			// 
			this.textBoxPath.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxPath.Location = new System.Drawing.Point(114, 3);
			this.textBoxPath.Name = "textBoxPath";
			this.textBoxPath.Size = new System.Drawing.Size(336, 21);
			this.textBoxPath.TabIndex = 1;
			this.textBoxPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// dateTimePickerAlamer
			// 
			this.dateTimePickerAlamer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.dateTimePickerAlamer.Location = new System.Drawing.Point(114, 43);
			this.dateTimePickerAlamer.Name = "dateTimePickerAlamer";
			this.dateTimePickerAlamer.Size = new System.Drawing.Size(323, 21);
			this.dateTimePickerAlamer.TabIndex = 3;
			this.dateTimePickerAlamer.ValueChanged += new System.EventHandler(this.dateTimePickerAlamer_ValueChanged);
			// 
			// buttonLogFlush
			// 
			this.buttonLogFlush.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonLogFlush.Location = new System.Drawing.Point(964, 3);
			this.buttonLogFlush.Name = "buttonLogFlush";
			this.tableLayoutPanel2.SetRowSpan(this.buttonLogFlush, 2);
			this.buttonLogFlush.Size = new System.Drawing.Size(154, 61);
			this.buttonLogFlush.TabIndex = 4;
			this.buttonLogFlush.Text = "报警日志";
			this.buttonLogFlush.UseVisualStyleBackColor = true;
			this.buttonLogFlush.Click += new System.EventHandler(this.buttonLogFlush_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.buttonSavePath);
			this.panel2.Controls.Add(this.buttonLoad);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(114, 83);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(336, 41);
			this.panel2.TabIndex = 7;
			// 
			// buttonSavePath
			// 
			this.buttonSavePath.Location = new System.Drawing.Point(25, 0);
			this.buttonSavePath.Name = "buttonSavePath";
			this.buttonSavePath.Size = new System.Drawing.Size(118, 41);
			this.buttonSavePath.TabIndex = 6;
			this.buttonSavePath.Text = "修改Log保存路径";
			this.buttonSavePath.UseVisualStyleBackColor = true;
			this.buttonSavePath.Click += new System.EventHandler(this.buttonSavePath_Click);
			// 
			// buttonLoad
			// 
			this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonLoad.Location = new System.Drawing.Point(180, 0);
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.Size = new System.Drawing.Size(124, 41);
			this.buttonLoad.TabIndex = 5;
			this.buttonLoad.Text = "重新加载报警日志";
			this.buttonLoad.UseVisualStyleBackColor = true;
			this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(20, 48);
			this.label2.Margin = new System.Windows.Forms.Padding(20, 8, 3, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "Log加载日期：";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// AlamerView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(1191, 712);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "AlamerView";
			this.Text = "Alarmview";
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewAlamer)).EndInit();
			this.panel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dataGridViewAlamer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.DateTimePicker dateTimePickerAlamer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonSavePath;
        private System.Windows.Forms.DataGridViewTextBoxColumn 日期;
        private System.Windows.Forms.DataGridViewTextBoxColumn time;
        private System.Windows.Forms.DataGridViewTextBoxColumn 报警代码;
        private System.Windows.Forms.DataGridViewTextBoxColumn 报警位置;
        private System.Windows.Forms.DataGridViewTextBoxColumn 报警信息;
        private System.Windows.Forms.DataGridViewTextBoxColumn 持续时间;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button buttonLogFlush;
        private System.Windows.Forms.Panel panel2;
    }
}