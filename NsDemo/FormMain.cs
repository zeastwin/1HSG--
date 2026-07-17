using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using NLicense;
using NVarConfig;
using NUser;
using NProcUnit;
using NMotionCtrl;
using static NsDemo.CoreWork;


namespace NsDemo
{
	public partial class FormMain : Form
	{
		MLogin m_login;                //登录
		DebugApp m_debugView;          //调试
		public OpDlg m_maindebugView;         //主界面
		LogView m_logView;             //log
		DataView m_dataView;           //data
		AlamerView m_alamerView;       //报警
		public UserManager m_users;
		private LoginStatus m_callLogin;
		NCommon.NCollection<int, WndPosInfo> wndpos;
		SLicense m_license = SLicense.GetInstance();
		BtnSelEnum m_selectID = BtnSelEnum.NO_SEL;

		public NervousSystem.NSMain NMain = new NervousSystem.NSMain();


		public FormMain()
		{
			InitializeComponent();

			NMain.SetExternUse();
			m_login = new MLogin();
			m_debugView = new DebugApp();
			m_maindebugView = new OpDlg();
			m_dataView = new DataView();
			m_alamerView = new AlamerView();
			m_logView = new LogView();
			NervousSystem.HotKey.RegisterHotKey(this.Handle, 120, NervousSystem.HotKey.KeyModifiers.Alt, Keys.L);
			m_users = UserManager.GetInstance();
			m_callLogin = new LoginStatus(LoginCallBack);
			UserManager.GetInstance().RegisterCallBack(m_callLogin);
			LoginCallBack(false);
			timer1.Start();
			GetTime(null, null);
            Thread t = new Thread(() => Updatas());
            t.Start();

        }
        public void Updatas()
        {
            while (true)
            {
                this.Invoke(new Action(() =>
                {
					short val = 0;
					try
                    {
						m_maindebugView.sn1.Text = OpDlg.GetInstance().m_var.GetCValue(28);
						m_maindebugView.sn2.Text = OpDlg.GetInstance().m_var.GetCValue(29);
						m_maindebugView.sn3.Text = OpDlg.GetInstance().m_var.GetCValue(30);
						m_maindebugView.sn4.Text = OpDlg.GetInstance().m_var.GetCValue(31);

						m_maindebugView.slot1.Text = OpDlg.GetInstance().m_var.GetCValue(2);
						m_maindebugView.slot2.Text = OpDlg.GetInstance().m_var.GetCValue(3);
						m_maindebugView.slot3.Text = OpDlg.GetInstance().m_var.GetCValue(4);
						m_maindebugView.slot4.Text = OpDlg.GetInstance().m_var.GetCValue(5);

						m_maindebugView.suck1.Text = OpDlg.GetInstance().m_var.GetCValue(7);
						m_maindebugView.suck2.Text = OpDlg.GetInstance().m_var.GetCValue(8);

						m_maindebugView.carrySn.Text = OpDlg.GetInstance().m_var.GetCValue("载具码");
						m_maindebugView.HSG1.Text = OpDlg.GetInstance().m_var.GetCValue("穴位1-BC码");
						m_maindebugView.HSG2.Text = OpDlg.GetInstance().m_var.GetCValue("穴位2-BC码");
						m_maindebugView.HSG3.Text = OpDlg.GetInstance().m_var.GetCValue("穴位3-BC码");
						m_maindebugView.HSG4.Text = OpDlg.GetInstance().m_var.GetCValue("穴位4-BC码");
						m_maindebugView.InputTime.Text = OpDlg.GetInstance().m_var.GetCValue("进烤炉时间");
						m_maindebugView.OutputTime.Text = OpDlg.GetInstance().m_var.GetCValue("出烤炉时间");
						m_maindebugView.timeSpan.Text = OpDlg.GetInstance().m_var.GetCValue("烤炉用时");

						m_maindebugView.btnHSGResult1.Text = OpDlg.GetInstance().m_var.GetCValue("穴位1-结果");
						m_maindebugView.btnHSGResult2.Text = OpDlg.GetInstance().m_var.GetCValue("穴位2-结果");
						m_maindebugView.btnHSGResult3.Text = OpDlg.GetInstance().m_var.GetCValue("穴位3-结果");
						m_maindebugView.btnHSGResult4.Text = OpDlg.GetInstance().m_var.GetCValue("穴位4-结果");

						m_maindebugView.MESBox.Checked = OpDlg.GetInstance().m_var.GetDValue("MES启用标记")==1?true:false;

						string RFIDFlag = OpDlg.GetInstance().m_var.GetCValue("上站RFID写入动作标记");
						if (RFIDFlag != "5")
                        {
							m_maindebugView.RFIDMsgState.Text ="前站RFID写入异常";
						}
                        else
                        {
							m_maindebugView.RFIDMsgState.Text = "OK";
						}

						if(m_maindebugView.m_motion != null)
                        {
							val = m_maindebugView.m_motion.ReadIO("机器人前门禁开关");
							if (val == 0)
							{
								m_maindebugView.btnRbtDoorFore.BackColor = Color.Red;
							}
							else
							{
								m_maindebugView.btnRbtDoorFore.BackColor = Color.Green;
							}
							val = m_maindebugView.m_motion.ReadIO("机器人后门禁开关");
							if (val == 0)
							{
								m_maindebugView.btnRbtDoorBehind.BackColor = Color.Red;
							}
							else
							{
								m_maindebugView.btnRbtDoorBehind.BackColor = Color.Green;
							}
							val = m_maindebugView.m_motion.ReadIO("前门禁");
							if (val == 0)
							{
								m_maindebugView.ForeDoor.BackColor = Color.Red;
							}
							else
							{
								m_maindebugView.ForeDoor.BackColor = Color.Green;
							}
							val = m_maindebugView.m_motion.ReadIO("侧门禁");
							if (val == 0)
							{
								m_maindebugView.LeftDoor.BackColor = Color.Red;
							}
							else
							{
								m_maindebugView.LeftDoor.BackColor = Color.Green;
							}
							val = m_maindebugView.m_motion.ReadIO("后门禁");
							if (val == 0)
							{
								m_maindebugView.BackDoor.BackColor = Color.Red;
							}
							else
							{
								m_maindebugView.BackDoor.BackColor = Color.Green;
							}

							double Model = OpDlg.GetInstance().m_var.GetDValue("模式选择Flg");
							if (Model == 1)
							{
								OpDlg.GetInstance().m_var.SetDValue("跳过扫码", 0);
							//	OpDlg.GetInstance().m_var.SetDValue("启用6轴Rbt标记", 1);
								OpDlg.GetInstance().m_var.SetDValue("启用围栏门禁信号标记", 1);
								OpDlg.GetInstance().m_var.SetDValue("空取空放标记", 0);
								OpDlg.GetInstance().m_var.SetDValue("RFID启用标记", 1);
								OpDlg.GetInstance().m_var.SetDValue("不换盘启用标记", 0);
								OpDlg.GetInstance().m_var.SetDValue("NG重投模式", 0);
								OpDlg.GetInstance().m_var.SetDValue("启用不取料", 0);
								OpDlg.GetInstance().m_var.SetDValue("保压时间管控启用", 1);
								OpDlg.GetInstance().m_var.SetDValue("启用满盖板报警", 1);
								

							}
						}
						
					}
					catch (Exception ex)
                    {

                    }
                 
				}));


                Thread.Sleep(1000);
            }
        }
        private void MainView_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (DialogResult.Cancel == MessageBox.Show("确定要退出吗？", "提示", MessageBoxButtons.OKCancel))
			{
				this.Visible = true;
				e.Cancel = true;
				return;
			}
			
			if (m_maindebugView != null)
			{
				panelUserCtrl.Controls.Clear();
				m_maindebugView.Close();
			}
			NMain.ExitSystem();
		}
		private void ExitProc()
		{
			int id = (int)VarInteface.GetInstance().GetDValue("退出流程");
			Unit.GetInstance().SetWarmOut(null);//注销状态回调
			//Unit.GetInstance().PRun(id, 3);
			//while (true)
			//{
			//    Unit.GetInstance().PGetProcStatus(id, ref status);
			//    if (status.status == ActionStatus.IDE || status.status == ActionStatus.STOP)
			//    {
			//        break;
			//    }
			//    Thread.Sleep(500);
			//}
		}

		private void InitProc()
		{
			int id = (int)VarInteface.GetInstance().GetDValue("初始化UI流程");
			if (id >= 0)
			{
				Unit.GetInstance().SetWarmOut(null);//注销状态回调
				Unit.GetInstance().PRun(id, 3);
			}
		}

		private void MainView_Load(object sender, EventArgs e)
		{
			UpdataView(BtnSelEnum.HOME_SEL);
			//wndpos = NCommon.NCollection<int, WndPosInfo>.Load("Config\\mainPos.ns");
			//if (null != wndpos)
			//{
			//    //this.Location = wndpos[0].location;
			//    //this.Size = wndpos[0].wndsize;
			//    //this.WindowState = FormWindowState.Maximized;
			//    //this.WindowState = FormWindowState.Normal;
			//}
			InitProc();
		}

		protected override void WndProc(ref Message m)
		{
			const int WM_HOTKEY = 0x0312;
			switch (m.Msg)
			{
				case WM_HOTKEY:
					if ((int)m.WParam == 100)
					{//alt + z

						DisPlay();
					}
					else if ((int)m.WParam == 120)
					{//alt + L
						if (m_login.Visible)
						{
							m_login.Hide();
						}
						else
						{
							m_login.ShowWin(true);
						}
					}
					break;
			}
			base.WndProc(ref m);
		}

		private void LoginCallBack(bool flag)
		{
			string levelstr = "OP";
			if (flag)
			{
				if (m_users.GetGurUserLevel() == (short)UserGroup.OpLimit)
				{
					levelstr = "操作员:";
				}
				else if (m_users.GetGurUserLevel() == (short)UserGroup.AdminLimit)
				{
					levelstr = "管理员:";
				}
				else if (m_users.GetGurUserLevel() == (short)UserGroup.SystemLimit)
				{
					levelstr = "系统管理员:";
				}
				else if (m_users.GetGurUserLevel() == (short)UserGroup.RootLimit)
				{
					levelstr = "超级用户:";
				}

				LabSWUser.Text = levelstr + m_users.GetCurUser();
				SetBtnSelect(BtnSelEnum.LOGIN_SEL, true);
			}
			else
			{
				LabSWUser.Text = "请登录";
				SetBtnSelect(BtnSelEnum.LOGIN_SEL, false);
			}

			VarInfo vinfo = VarCommu.GetInstance().GetVarInfo("登录用户名称");
			if (null != vinfo)
			{
				vinfo.varCval = m_users.GetCurUser();
				VarCommu.GetInstance().SetVarInfo(vinfo);
			}
		}

		private void GetTime(object sender, EventArgs e)
		{
			string date, time;
			DateTime dt = DateTime.Now;

			date = dt.ToString("yyyy-MM-dd");
			time = dt.ToString("HH:mm:ss");

			Date.Text = date;
			Time.Text = time;

			Login_out();

        }
        DateTime _LoginOperation_LastTime;
        private void Login_out()
        {
            string userName = UserManager.GetInstance().GetCurUser();
            if (string.IsNullOrEmpty(userName))
            {
                _LoginOperation_LastTime = DateTime.Now;
                string uid = "";
                string errmsg = "";

                int Login_model = (int)VarInteface.GetInstance().GetDValue("登录模式");
                if (Login_model == 1)
                {
                    NFCReader.Instance.ReadID(out uid, out errmsg);
                }
                else if (Login_model == 2)
                {
                    ZKFinger.Instance.ReadID(out uid, out errmsg);

                }
                else
                {
                    return;
                }


                if (!string.IsNullOrEmpty(uid))
                {
                    if (UserManager.GetInstance().Login(uid, uid))
                    {
                        string strUID = uid.Substring(4, uid.Length - 4);
                        short level = UserManager.GetInstance().GetGurUserLevel();
                        if (level == 0)
                        {
                            strUID = "****" + strUID + "   技术员登录";
                            label_login_level.Text = "Level" + level.ToString();
                        }
                        else if (level == 1)
                        {
                            strUID = "****" + strUID + "   管理员登录";
                            label_login_level.Text = "Level" + level.ToString();
                        }
                        else if (level == 2)
                        {
                            strUID = "****" + strUID + "   超级管理员登录";
                            label_login_level.Text = "Level" + level.ToString();
                        }
                        else
                        {
                            strUID = "****" + strUID + "   技术员登录";
                            label_login_level.Text = "Level" + level.ToString();
                        }

                        button_finger_login.Text = strUID;
                    }
                    else
                    {
                        string strUID = uid + "    无登录权限";
                        button_finger_login.Text = strUID;
                        label_login_level.Text = "Level";
                    }
                }
            }
            else
            {
                if (_LoginOperation_LastTime == null)
                {
                    _LoginOperation_LastTime = DateTime.Now;
                }
                TimeSpan Diff = DateTime.Now - _LoginOperation_LastTime;


                if (Diff.TotalSeconds > 1000000)
                {
                    if (UserManager.GetInstance().Exit(userName))
                    {
                        button_finger_login.Text = "";
                    }

                }
            }
        }

        private void DisPlay()
		{
			long res = m_license.CheckLicense();
			if (res > 0)
			{
				if (this.Visible)
				{
					this.Visible = false;
				}

				else
				{
					this.Visible = true;
					this.WindowState = FormWindowState.Normal;
					this.TopMost = true;
					this.Invalidate();
					this.TopMost = false;
				}
			}
			else
			{
				this.Visible = false;
			}
		}

		private void panelUserCtrl_Paint(object sender, PaintEventArgs e)
		{
			ControlPaint.DrawBorder(e.Graphics, panelUserCtrl.ClientRectangle, Color.FromArgb(255, 213, 223, 229), ButtonBorderStyle.Solid);
		}

		private void SetBtnSelect(BtnSelEnum selID, bool bsel)
		{
			switch (selID)
			{
				case BtnSelEnum.HOME_SEL:
					buttonHome.BackgroundImage = bsel ? NsDemo.Properties.Resources.Home_sel : NsDemo.Properties.Resources.Home;
					break;
				case BtnSelEnum.PARAMTER_SEL:
					buttonDebug.BackgroundImage = bsel ? NsDemo.Properties.Resources.Paramter_sel : NsDemo.Properties.Resources.Pamerter;
					break;
				case BtnSelEnum.ALAMER_SEL:
					buttonWarm.BackgroundImage = bsel ? NsDemo.Properties.Resources.Alamer_sel : NsDemo.Properties.Resources.Alamer;
					break;
				case BtnSelEnum.DATA_SEL:
					buttonData.BackgroundImage = bsel ? NsDemo.Properties.Resources.Data_sel : NsDemo.Properties.Resources.Data;
					break;
				case BtnSelEnum.LOG_SEL:
					buttonLog.BackgroundImage = bsel ? NsDemo.Properties.Resources.Log_sel : NsDemo.Properties.Resources.Log;
					break;
				case BtnSelEnum.START_SEL:
					buttonStart.BackgroundImage = bsel ? NsDemo.Properties.Resources.Start_sel : NsDemo.Properties.Resources.Start;
					break;
				case BtnSelEnum.PAUSE_SEL:
					buttonPause.BackgroundImage = bsel ? NsDemo.Properties.Resources.Pause_sel : NsDemo.Properties.Resources.Pause;
					break;
				case BtnSelEnum.STOP_SEL:
					buttonStop.BackgroundImage = bsel ? NsDemo.Properties.Resources.Stop_sel : NsDemo.Properties.Resources.Stop;
					break;
				case BtnSelEnum.LOGIN_SEL:
					buttonLogin.BackgroundImage = bsel ? NsDemo.Properties.Resources.Login_sel : NsDemo.Properties.Resources.Login;
					break;
				default:
					break;
			}
		}

        public BtnSelEnum GetBtnSelect()
        {
            return m_selectID;
        }
		
		public void UpdataView(BtnSelEnum selID)
		{
			Form form = null;
			try
			{
				switch (selID)
				{
					case BtnSelEnum.HOME_SEL:
						form = m_maindebugView;
						break;
					case BtnSelEnum.PARAMTER_SEL:
						form = m_debugView;
						break;
					case BtnSelEnum.ALAMER_SEL:
						form = m_alamerView;
						break;
					case BtnSelEnum.DATA_SEL:
						form = m_dataView;
						break;
					case BtnSelEnum.LOG_SEL:
						form = m_logView;
						break;
					case BtnSelEnum.START_SEL:
						break;
					case BtnSelEnum.PAUSE_SEL:
						break;
					case BtnSelEnum.STOP_SEL:
						break;
					case BtnSelEnum.LOGIN_SEL:
						{
							if (m_login.Visible)
							{
								m_login.Hide();
							}
							else
							{
								m_login.ShowWin();
							}
						}
						return;
					default:
						break;
				}

				if (null != form)
				{
					if (panelUserCtrl.Contains(form))
					{
						return;
					}
					panelUserCtrl.Controls.Clear();
					form.TopLevel = false;
					form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
					form.Dock = DockStyle.Fill;
					form.Visible = true;
					panelUserCtrl.Controls.Add(form);
				}

			}
			catch (System.Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void ChangeSelUI(BtnSelEnum btnsel)
		{
            short sLevel = UserManager.GetInstance().GetGurUserLevel(); // 判断权限是否登录！！
            if (sLevel == -1)
            {
                return;
            }

            bool bsel = false;
			if (m_selectID == btnsel)
			{
				bsel = false;
				return;
			}
			else
			{
				SetBtnSelect(m_selectID, false);  //把之前点击的设置未选择状态
				bsel = true;
			}
			m_selectID = btnsel;
			SetBtnSelect(m_selectID, bsel);   //设置现在点击的为选择状态  
			UpdataView(m_selectID);
		}

		private void buttonHome_Click(object sender, EventArgs e)
		{
			ChangeSelUI(BtnSelEnum.HOME_SEL);

			m_maindebugView.rBtnLG.Checked = m_maindebugView.m_var.GetDValue(46) == 1 ? true : false;
			m_maindebugView.rBtnSM.Checked = m_maindebugView.m_var.GetDValue(46) == 0 ? true : false;
			//	OpDlg.GetInstance().rBtnSM.Checked = OpDlg.GetInstance().m_var.GetDValue(46) == 0 ? true : false;

		}

		private void buttonDebug_Click(object sender, EventArgs e)
		{
			ChangeSelUI(BtnSelEnum.PARAMTER_SEL);
		}

		private void buttonWarm_Click(object sender, EventArgs e)
		{
			ChangeSelUI(BtnSelEnum.ALAMER_SEL);
		}

		private void buttonData_Click(object sender, EventArgs e)
		{
			ChangeSelUI(BtnSelEnum.DATA_SEL);
		}

		private void buttonStart_Click(object sender, EventArgs e)
		{
			SetBtnSelect(BtnSelEnum.START_SEL, true);
			SetBtnSelect(BtnSelEnum.PAUSE_SEL, false);
			SetBtnSelect(BtnSelEnum.STOP_SEL, false);
            //这时设置“自动/手动”系统变量为1，自动状态
            VarInteface.GetInstance().SetDValue("自动/手动", 1);
		}

		private void buttonPause_Click(object sender, EventArgs e)
		{
			SetBtnSelect(BtnSelEnum.START_SEL, false);
			SetBtnSelect(BtnSelEnum.PAUSE_SEL, true);
			SetBtnSelect(BtnSelEnum.STOP_SEL, false);
            
		}

		private void buttonStop_Click(object sender, EventArgs e)
		{
			

			if (DialogResult.OK == MessageBox.Show("确定要停止所有流程吗？", "提示", MessageBoxButtons.OKCancel))
			{
				int procCount = Unit.GetInstance().PGetProcCount();

				SetBtnSelect(BtnSelEnum.START_SEL, false);
				SetBtnSelect(BtnSelEnum.PAUSE_SEL, false);
				SetBtnSelect(BtnSelEnum.STOP_SEL, true);
				//这时设置“自动/手动”系统变量为0，手动状态
				VarInteface.GetInstance().SetDValue("自动/手动", 0);
				VarInteface.GetInstance().SetDValue("暂停变量", 0);

				MotionCtrl.GetInstance().WriteIO("三色灯-红灯", 0);
				MotionCtrl.GetInstance().WriteIO("蜂鸣器", 0);

				MotionCtrl.GetInstance().WriteIO("进接驳电机1", 0);
				MotionCtrl.GetInstance().WriteIO("回流接驳电机2", 0);
				MotionCtrl.GetInstance().WriteIO("升降接驳电机3", 0);
				MotionCtrl.GetInstance().WriteIO("机器人载具接驳电机4", 0);
				MotionCtrl.GetInstance().WriteIO("满盘接驳电机5", 0);
				MotionCtrl.GetInstance().WriteIO("空盘接驳电机6", 0);

				for (int i = 0; i < procCount; i++)
				{
					if (i == 2 || i == 3 || i == 4 || i == 17 || i == 22)
						continue;
					Unit.GetInstance().PStop(i);
				}

			}
		}

		private void buttonLog_Click(object sender, EventArgs e)
		{
			ChangeSelUI(BtnSelEnum.LOG_SEL);
		}

		private void buttonLogin_Click(object sender, EventArgs e)
		{
			UpdataView(BtnSelEnum.LOGIN_SEL);
		}

        private void buttonLog_MouseUp(object sender, MouseEventArgs e)
        {
			if (e.Button == MouseButtons.Right)
			{
				if (panelUserCtrl.Controls.Count <= 0)
				{
					return;
				}
				Form thf = panelUserCtrl.Controls[0] as Form;
				if (thf is LogView)
				{
					panelUserCtrl.Controls.Clear();
					thf.TopLevel = true;
					thf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
					thf.Dock = DockStyle.None;
					thf.Size = new Size(1000, 700);
					thf.WindowState = FormWindowState.Normal;
					thf.Location = new Point(300, 100);
					thf.Visible = true;
				}
			}
		}

        private void button_finger_login_Click(object sender, EventArgs e)
        {
            string userName = UserManager.GetInstance().GetCurUser();
            if (!string.IsNullOrEmpty(userName))
            {
                if (UserManager.GetInstance().Exit(userName))
                {
                    button_finger_login.Text = "Logout";
                    label_login_level.Text = "已登出";
                }
            }
        }
    }

    [Serializable]
	public class WndPosInfo
	{
		public Point location;
		public Size wndsize;
	}

	public enum BtnSelEnum
	{
		NO_SEL = -1,
		HOME_SEL = 0,
		PARAMTER_SEL,
		ALAMER_SEL,
		DATA_SEL,
		EXCEL_SEL,
		LOG_SEL,
		LOGIN_SEL,
		START_SEL,
		PAUSE_SEL,
		STOP_SEL,

	}
}
