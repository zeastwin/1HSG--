namespace NsDemo
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.LabSWUser = new System.Windows.Forms.Label();
            this.Date = new System.Windows.Forms.Label();
            this.Time = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonHome = new System.Windows.Forms.Button();
            this.buttonDebug = new System.Windows.Forms.Button();
            this.buttonWarm = new System.Windows.Forms.Button();
            this.buttonName = new System.Windows.Forms.Button();
            this.buttonData = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonLog = new System.Windows.Forms.Button();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label_login_level = new System.Windows.Forms.Label();
            this.button_finger_login = new System.Windows.Forms.Button();
            this.panelUserCtrl = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabSWUser
            // 
            this.LabSWUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LabSWUser.Location = new System.Drawing.Point(829, 1);
            this.LabSWUser.Name = "LabSWUser";
            this.LabSWUser.Size = new System.Drawing.Size(172, 23);
            this.LabSWUser.TabIndex = 2;
            this.LabSWUser.Text = "User";
            this.LabSWUser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Date
            // 
            this.Date.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Date.AutoSize = true;
            this.Date.Font = new System.Drawing.Font("宋体", 10.5F);
            this.Date.Location = new System.Drawing.Point(1, 6);
            this.Date.Name = "Date";
            this.Date.Size = new System.Drawing.Size(77, 14);
            this.Date.TabIndex = 3;
            this.Date.Text = "2020-06-19";
            // 
            // Time
            // 
            this.Time.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Time.AutoSize = true;
            this.Time.Font = new System.Drawing.Font("宋体", 10.5F);
            this.Time.Location = new System.Drawing.Point(86, 6);
            this.Time.Name = "Time";
            this.Time.Size = new System.Drawing.Size(84, 14);
            this.Time.TabIndex = 4;
            this.Time.Text = "09：20 ：01";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.GetTime);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BackgroundImage = global::NsDemo.Properties.Resources.EW1;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(1011, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(17, 15);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(1033, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 14);
            this.label1.TabIndex = 6;
            this.label1.Text = "联合东创科技有限公司";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(1214, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 14);
            this.label2.TabIndex = 7;
            this.label2.Text = "电话:0769—39026833";
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.AutoSize = true;
            this.labelVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelVersion.Font = new System.Drawing.Font("宋体", 11F);
            this.labelVersion.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelVersion.Location = new System.Drawing.Point(1374, 5);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(116, 15);
            this.labelVersion.TabIndex = 16;
            this.labelVersion.Text = "版本号: V3.0.0";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.LabSWUser);
            this.splitContainer1.Panel2.Controls.Add(this.labelVersion);
            this.splitContainer1.Panel2.Controls.Add(this.Date);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.Time);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Size = new System.Drawing.Size(1798, 650);
            this.splitContainer1.SplitterDistance = 623;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 17;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelUserCtrl, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1798, 623);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 13;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.buttonHome, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonDebug, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonWarm, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonName, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonData, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonStart, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonPause, 7, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonStop, 8, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonLog, 9, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonLogin, 10, 0);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label_login_level, 11, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_finger_login, 12, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1798, 100);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // buttonHome
            // 
            this.buttonHome.BackColor = System.Drawing.Color.Transparent;
            this.buttonHome.BackgroundImage = global::NsDemo.Properties.Resources.Home;
            this.buttonHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonHome.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonHome.FlatAppearance.BorderSize = 0;
            this.buttonHome.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonHome.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHome.Location = new System.Drawing.Point(13, 10);
            this.buttonHome.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(81, 80);
            this.buttonHome.TabIndex = 0;
            this.buttonHome.UseVisualStyleBackColor = false;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // buttonDebug
            // 
            this.buttonDebug.BackColor = System.Drawing.Color.Transparent;
            this.buttonDebug.BackgroundImage = global::NsDemo.Properties.Resources.Pamerter;
            this.buttonDebug.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonDebug.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonDebug.FlatAppearance.BorderSize = 0;
            this.buttonDebug.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonDebug.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonDebug.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDebug.Location = new System.Drawing.Point(113, 10);
            this.buttonDebug.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.buttonDebug.Name = "buttonDebug";
            this.buttonDebug.Size = new System.Drawing.Size(80, 80);
            this.buttonDebug.TabIndex = 0;
            this.buttonDebug.UseVisualStyleBackColor = false;
            this.buttonDebug.Click += new System.EventHandler(this.buttonDebug_Click);
            // 
            // buttonWarm
            // 
            this.buttonWarm.BackColor = System.Drawing.Color.Transparent;
            this.buttonWarm.BackgroundImage = global::NsDemo.Properties.Resources.Alamer;
            this.buttonWarm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonWarm.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonWarm.FlatAppearance.BorderSize = 0;
            this.buttonWarm.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonWarm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonWarm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonWarm.Location = new System.Drawing.Point(213, 10);
            this.buttonWarm.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.buttonWarm.Name = "buttonWarm";
            this.buttonWarm.Size = new System.Drawing.Size(80, 80);
            this.buttonWarm.TabIndex = 0;
            this.buttonWarm.UseVisualStyleBackColor = false;
            this.buttonWarm.Click += new System.EventHandler(this.buttonWarm_Click);
            // 
            // buttonName
            // 
            this.buttonName.BackColor = System.Drawing.Color.Transparent;
            this.buttonName.FlatAppearance.BorderSize = 0;
            this.buttonName.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonName.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonName.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonName.Location = new System.Drawing.Point(415, 10);
            this.buttonName.Margin = new System.Windows.Forms.Padding(5, 10, 0, 3);
            this.buttonName.Name = "buttonName";
            this.buttonName.Size = new System.Drawing.Size(220, 80);
            this.buttonName.TabIndex = 0;
            this.buttonName.Text = "HSG下料机";
            this.buttonName.UseVisualStyleBackColor = false;
            // 
            // buttonData
            // 
            this.buttonData.BackColor = System.Drawing.Color.Transparent;
            this.buttonData.BackgroundImage = global::NsDemo.Properties.Resources.Data;
            this.buttonData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonData.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonData.FlatAppearance.BorderSize = 0;
            this.buttonData.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonData.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonData.Location = new System.Drawing.Point(313, 10);
            this.buttonData.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.buttonData.Name = "buttonData";
            this.buttonData.Size = new System.Drawing.Size(80, 80);
            this.buttonData.TabIndex = 0;
            this.buttonData.UseVisualStyleBackColor = false;
            this.buttonData.Visible = false;
            this.buttonData.Click += new System.EventHandler(this.buttonData_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.BackColor = System.Drawing.Color.Transparent;
            this.buttonStart.BackgroundImage = global::NsDemo.Properties.Resources.Start;
            this.buttonStart.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonStart.FlatAppearance.BorderSize = 0;
            this.buttonStart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonStart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStart.Location = new System.Drawing.Point(653, 10);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(80, 80);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.UseVisualStyleBackColor = false;
            this.buttonStart.Visible = false;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.BackColor = System.Drawing.Color.Transparent;
            this.buttonPause.BackgroundImage = global::NsDemo.Properties.Resources.Pause;
            this.buttonPause.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonPause.FlatAppearance.BorderSize = 0;
            this.buttonPause.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonPause.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPause.Location = new System.Drawing.Point(753, 10);
            this.buttonPause.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(80, 80);
            this.buttonPause.TabIndex = 0;
            this.buttonPause.UseVisualStyleBackColor = false;
            this.buttonPause.Visible = false;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.BackColor = System.Drawing.Color.Transparent;
            this.buttonStop.BackgroundImage = global::NsDemo.Properties.Resources.Stop;
            this.buttonStop.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonStop.FlatAppearance.BorderSize = 0;
            this.buttonStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStop.Location = new System.Drawing.Point(853, 10);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(80, 80);
            this.buttonStop.TabIndex = 0;
            this.buttonStop.UseVisualStyleBackColor = false;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonLog
            // 
            this.buttonLog.BackColor = System.Drawing.Color.Transparent;
            this.buttonLog.BackgroundImage = global::NsDemo.Properties.Resources.Log;
            this.buttonLog.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonLog.FlatAppearance.BorderSize = 0;
            this.buttonLog.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonLog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLog.Location = new System.Drawing.Point(953, 10);
            this.buttonLog.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.Size = new System.Drawing.Size(80, 80);
            this.buttonLog.TabIndex = 0;
            this.buttonLog.UseVisualStyleBackColor = false;
            this.buttonLog.Click += new System.EventHandler(this.buttonLog_Click);
            this.buttonLog.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonLog_MouseUp);
            // 
            // buttonLogin
            // 
            this.buttonLogin.BackColor = System.Drawing.Color.Transparent;
            this.buttonLogin.BackgroundImage = global::NsDemo.Properties.Resources.Login;
            this.buttonLogin.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonLogin.FlatAppearance.BorderSize = 0;
            this.buttonLogin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonLogin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLogin.Location = new System.Drawing.Point(1053, 10);
            this.buttonLogin.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(80, 80);
            this.buttonLogin.TabIndex = 0;
            this.buttonLogin.UseVisualStyleBackColor = false;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("微软雅黑", 15F);
            this.label3.Location = new System.Drawing.Point(3, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(4, 80);
            this.label3.TabIndex = 1;
            this.label3.Text = "Admin NoramlRun";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_login_level
            // 
            this.label_login_level.Font = new System.Drawing.Font("微软雅黑", 15F);
            this.label_login_level.Location = new System.Drawing.Point(1153, 10);
            this.label_login_level.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.label_login_level.Name = "label_login_level";
            this.label_login_level.Size = new System.Drawing.Size(124, 87);
            this.label_login_level.TabIndex = 3;
            this.label_login_level.Text = "Login_level";
            this.label_login_level.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_finger_login
            // 
            this.button_finger_login.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_finger_login.Location = new System.Drawing.Point(1283, 3);
            this.button_finger_login.Name = "button_finger_login";
            this.button_finger_login.Size = new System.Drawing.Size(252, 94);
            this.button_finger_login.TabIndex = 7;
            this.button_finger_login.Text = "指纹登录";
            this.button_finger_login.UseVisualStyleBackColor = true;
            this.button_finger_login.Click += new System.EventHandler(this.button_finger_login_Click);
            // 
            // panelUserCtrl
            // 
            this.panelUserCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelUserCtrl.Location = new System.Drawing.Point(3, 103);
            this.panelUserCtrl.Name = "panelUserCtrl";
            this.panelUserCtrl.Padding = new System.Windows.Forms.Padding(5);
            this.panelUserCtrl.Size = new System.Drawing.Size(1792, 517);
            this.panelUserCtrl.TabIndex = 1;
            this.panelUserCtrl.Paint += new System.Windows.Forms.PaintEventHandler(this.panelUserCtrl_Paint);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1798, 650);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "9999";
            this.Text = "NeuralSys-主界面";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainView_FormClosing);
            this.Load += new System.EventHandler(this.MainView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LabSWUser;
        private System.Windows.Forms.Label Date;
        private System.Windows.Forms.Label Time;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button buttonHome;
        private System.Windows.Forms.Button buttonDebug;
        private System.Windows.Forms.Button buttonWarm;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Button buttonName;
        private System.Windows.Forms.Button buttonData;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonLog;
        private System.Windows.Forms.Panel panelUserCtrl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_login_level;
        private System.Windows.Forms.Button button_finger_login;
    }
}