using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace NsDemo
{
	public partial class FormStart : Form
	{
		public FormStart()
		{
            InitializeComponent();
		}
        private void FormStart_Load(object sender, EventArgs e)
        {
            StartMain();
        }
        MainView mv;
        LogCall call;
        bool loadFinish = false;
        int tickTime = 0;
        public delegate void LogCall(string msg);
        
        private void StartMain()
        {
            this.TopMost = false;
            axWindowsMediaPlayer1.URL = "startvideo.MP4";
            axWindowsMediaPlayer1.Ctlcontrols.play();
            timer1.Start();            
            call = new LogCall(Log);
            mv = new MainView();
            mv.Hide();
            mv.NMain.WindowState = FormWindowState.Normal;
            mv.NMain.Hide();
            ReportLog("开始启动");
        }
        public void ReportLog(string msg)
        {
            string defMsg = "..." + msg + ".........................................................................................";
            this.Invoke(call, defMsg);
        }
        private void Log(string msg)
        {
            listBoxlog.Items.Add(msg);
            listBoxlog.SelectedIndex = listBoxlog.Items.Count - 1;
            if (msg.Contains("未激活"))
            {
                axWindowsMediaPlayer1.Ctlcontrols.stop();
                timer1.Stop();
                Thread.Sleep(2000);
                this.Close();
            }
            else if (msg.Contains("加载完成"))
            {
                loadFinish = true;
            }
            
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            tickTime++;
            if (axWindowsMediaPlayer1.Ctlcontrols.currentPosition > 4)
            {
                axWindowsMediaPlayer1.Ctlcontrols.pause();
            }
            if (loadFinish)
            {
                this.Hide();
                Thread.Sleep(1000);
                mv.Show();                
                axWindowsMediaPlayer1.Ctlcontrols.stop();
                timer1.Stop();
            }
            else if (2 == tickTime)
            {
                try
                {
                    mv.NMain.Init(this);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
	}
}
