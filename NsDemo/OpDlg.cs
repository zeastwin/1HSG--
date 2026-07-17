using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;
using NVarConfig;
using NProcUnit;
using NLog;
using NMotionCtrl;
using NStructConfig;
using NCommon;
using NCard;
using NCommunicationTool;
using System.Threading;
using System.Collections;
using System.IO;
using static NsDemo.CoreWork;
using Newtonsoft.Json;
using NsDemo.MachineDebug;
using NsDemo.Utility;
using NsDemo.Virtual_IO;
using NsDemo.MESSys;

namespace NsDemo
{
	public partial class OpDlg : Form
	{
		private System.Windows.Forms.Timer m_tUpdateStatus = new System.Windows.Forms.Timer();
		NProcUnit.SystemStatus m_PreSystemStatus = NProcUnit.SystemStatus.未初始化;
        public MotionCtrl m_motion;
		public VarInteface m_var = VarInteface.GetInstance();
		private delegate void DelegDispLog(String msg);
		private StationData m_stData = new StationData();
        private static OpDlg m_Instance = null;

        private UI_VirtualIO ui_VirtualIO;
        private Virtual_IO_TCP Virtual_IO_TCP;
        public OpDlg()
		{
			InitializeComponent();
		}

		private void OpDlg_Load(object sender, EventArgs e)
		{
			CoreWork.Instance.SendDatas += OnGetData;   //事件
            GlobalEvent.ShowMsgH += PrintInfo;   //事件
			m_motion = MotionCtrl.GetInstance();
			m_tUpdateStatus.Interval = 500;
			m_tUpdateStatus.Tick += new EventHandler(UpdateStatus_Tick);
			m_tUpdateStatus.Start();


            InitTrayTemp();
            GenerateButtonMatrix4PassTray();
            GenerateButtonMatrix4NGTray();
            GenerateButtonMatrix4RestTray();

            Thread t = new Thread(() => Updatas());
            t.Start();

            rBtnLG.Checked = m_var.GetDValue(46) == 1 ? true : false;
            rBtnSM.Checked = m_var.GetDValue(46) == 0 ? true : false;

            CoreWork.Instance.slotInfo = ReadSlotInfo(TrayInfoPath + "PassInfo.json");
            CoreWork.Instance.slotNGInfo = ReadSlotInfo(TrayInfoPath + "NGInfo.json");
            CoreWork.Instance.bcBaseInfo = ReadAutoCreateDataMInfo(TrayInfoPath + "AutoCreateDataM.json"); 
            CoreWork.Instance.RestSlotInfo = ReadRestSlotInfo(TrayInfoPath + "RestInfo.json");
            btnSetMode.BackColor = Color.Yellow;
            btnSetMode.Text = "调机模式";
            m_var.SetDValue("模式选择Flg", 0);
            ShowCurTotal();

            if (m_var.GetDValue("虚拟IO启用标记") == 1)
            {
                ui_VirtualIO = new UI_VirtualIO();
                Virtual_IO_TCP = Virtual_IO_TCP.Instance;
            }
     
        }

        string TrayInfoPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + $"\\TrayInfo\\";

        private void OpDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_var.GetDValue("虚拟IO启用标记") == 1)
            {
                if (Virtual_IO_TCP != null)
                {
                    Virtual_IO_TCP.Dispose();
                }
            }
            Hide();
            e.Cancel = true;
            m_tUpdateStatus.Stop();

            m_motion.WriteIO("三色灯-红灯", 0);
            m_motion.WriteIO("蜂鸣器", 0);

            m_motion.WriteIO("进接驳电机1", 0);
            m_motion.WriteIO("回流接驳电机2", 0);
            m_motion.WriteIO("升降接驳电机3", 0);
            m_motion.WriteIO("机器人载具接驳电机4", 0);
            m_motion.WriteIO("满盘接驳电机5", 0);
            m_motion.WriteIO("空盘接驳电机6", 0);

            WriteSlotInfo(CoreWork.Instance.slotInfo, TrayInfoPath + "PassInfo.json");
            WriteSlotInfo(CoreWork.Instance.slotNGInfo, TrayInfoPath + "NGInfo.json");
            WriteRestSlotInfo(CoreWork.Instance.RestSlotInfo, TrayInfoPath + "RestInfo.json");
        }
        public bool WriteSlotInfo(Dictionary<int, Slot> SlotInfo, string strFilePath)
        {
            try
            {
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(SlotInfo, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(strFilePath, output);
            }
            catch (Exception ex)
            { 

            };
            return true;
        }
        public bool WriteRestSlotInfo(Dictionary<int, RestSlot> SlotInfo, string strFilePath)
        {
            try
            {
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(SlotInfo, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(strFilePath, output);
            }
            catch (Exception ex)
            {

            };
            return true;
        }
        public bc_window_assy_mesinfo ReadAutoCreateDataMInfo(string strFilePath)
        {
            bc_window_assy_mesinfo bcInfo = new bc_window_assy_mesinfo();
            if (!File.Exists(strFilePath))
            {
                return null;
            }

            try
            {
                StreamReader r = new StreamReader(strFilePath);
                JsonTextReader reader = new JsonTextReader(r);
                string json = r.ReadToEnd();

                bcInfo = JsonConvert.DeserializeObject<bc_window_assy_mesinfo>(json);

                r.Close();


            }

            catch (Exception e)
            {

                return null;
            }
            return bcInfo;

        }
        public Dictionary<int, Slot> ReadSlotInfo(string strFilePath)
        {
            Dictionary<int, Slot> SlotInf = new Dictionary<int, Slot>(); ;
            if (!File.Exists(strFilePath))
            {
                return new Dictionary<int, Slot>();
            }

            try
            {
                StreamReader r = new StreamReader(strFilePath);
                JsonTextReader reader = new JsonTextReader(r);
                string json = r.ReadToEnd();

                SlotInf = JsonConvert.DeserializeObject<Dictionary<int, Slot>>(json);

                r.Close();


            }

            catch (Exception e)
            {

                return new Dictionary<int, Slot>(); ;
            }

            if (SlotInf.Count == 0)
            {
                SlotInf.Clear();
                for (int i = 0; i <= 24; i++)
                {
                    SlotInf.Add(i, new Slot());
                }
            }
            return SlotInf;

        }
        public Dictionary<int, RestSlot> ReadRestSlotInfo(string strFilePath)
        {
            Dictionary<int, RestSlot> SlotInf = new Dictionary<int, RestSlot>(); ;
            if (!File.Exists(strFilePath))
            {
                return new Dictionary<int, RestSlot>();
            }

            try
            {
                StreamReader r = new StreamReader(strFilePath);
                JsonTextReader reader = new JsonTextReader(r);
                string json = r.ReadToEnd();

                SlotInf = JsonConvert.DeserializeObject<Dictionary<int, RestSlot>>(json);

                r.Close();


            }

            catch (Exception e)
            {

                return new Dictionary<int, RestSlot>(); ;
            }

            if (SlotInf.Count == 0)
            {
                SlotInf.Clear();
                for (int i = 0; i <= 35; i++)
                {
                    SlotInf.Add(i, new RestSlot());
                }
            }
            return SlotInf;

        }
        public void InitTrayTemp()
        {
            for (int i = 0; i < 25; i++)
            {
                PassTrayTemp[i] = -1;
            }

            for (int i = 0; i < 25; i++)
            {
                NGTrayTemp[i] = -1;
            }
            for (int i = 0; i < 36; i++)
            {
                RestTrayTemp[i] = -1;
            }
        }
        Button[] buttonsPass = new Button[25];
        Button[] buttonsNG = new Button[25];
        Button[] buttonsRest = new Button[36];

        int[] PassTrayTemp = new int[25];
        int[] NGTrayTemp = new int[25];
        int[] RestTrayTemp = new int[36];

        public void Updatas()
        {
            double sysStuteTemp = -1;
          
            while (true)
            {
                try
                {
                    //if (CoreWork.Instance.slotInfo == null)
                    //    continue;
                    //更新pass盘
                    for (int i = 0; i <= 24; i++)
                    {
                        if (CoreWork.Instance.slotInfo[i].isExist != PassTrayTemp[i])
                        {
                            if (CoreWork.Instance.slotInfo[i].isExist == 1)
                            {
                                buttonsPass[i].BackColor = Color.Green;
                                PassTrayTemp[i] = 1;
                            }
                            else if (CoreWork.Instance.slotInfo[i].isExist == 0)
                            {
                                buttonsPass[i].BackColor = Color.Gray;
                                PassTrayTemp[i] = 0;
                            }
                        }
                    }

                    ////更新NG盘
                    //for (int i = 0; i <= 24; i++)
                    //{
                    //    if (CoreWork.Instance.slotNGInfo[i].isExist != NGTrayTemp[i])
                    //    {
                    //        if (CoreWork.Instance.slotNGInfo[i].isExist == 1)
                    //        {
                    //            if(CoreWork.Instance.slotNGInfo[i].Result == "点胶工站NG")
                    //            {
                    //                buttonsNG[i].BackColor = Color.Red;
                    //            }
                    //            else
                    //            {
                    //                buttonsNG[i].BackColor = Color.Green;
                    //            }
                    //            NGTrayTemp[i] = 1;
                    //        }
                    //        else if (CoreWork.Instance.slotNGInfo[i].isExist == 0)
                    //        {
                    //            buttonsNG[i].BackColor = Color.Gray;
                    //            NGTrayTemp[i] = 0;
                    //        }
                    //    }
                    //}

                    //更新NG盘
                    for (int i = 0; i <= 24; i++)
                    {
                        if (CoreWork.Instance.slotNGInfo[i].isExist != NGTrayTemp[i])
                        {
                            if (CoreWork.Instance.slotNGInfo[i].isExist == 1)
                            {
                                if (CoreWork.Instance.slotNGInfo[i].Result == "Gap-测高NG")
                                {
                                    buttonsNG[i].BackColor = Color.Red;
                                }
                                else if (CoreWork.Instance.slotNGInfo[i].Result == "RFID读取失败抛NG")
                                {
                                    buttonsNG[i].BackColor = Color.Yellow;
                                }
                                else if (CoreWork.Instance.slotNGInfo[i].Result == "隧道炉时间异常抛NG")
                                {
                                    buttonsNG[i].BackColor = Color.Blue;
                                }
                                else if (CoreWork.Instance.slotNGInfo[i].Result == "玻璃扫码异常抛NG")
                                {
                                    buttonsNG[i].BackColor = Color.Black;
                                }
                                else if (CoreWork.Instance.slotNGInfo[i].Result == "点胶工站写RFID异常抛NG")
                                {
                                    buttonsNG[i].BackColor = Color.Maroon;
                                }
                                else if (CoreWork.Instance.slotNGInfo[i].Result == "MES异常抛NG")
                                {
                                    buttonsNG[i].BackColor = Color.Magenta;
                                }
                                else if (CoreWork.Instance.slotNGInfo[i].Result == "点胶-保压时间异常抛NG")
                                {
                                    buttonsNG[i].BackColor = Color.DarkOrange;
                                }
                                else if (CoreWork.Instance.slotNGInfo[i].Result == "GapNG")
                                {
                                    buttonsNG[i].BackColor = Color.Aqua;
                                }
                                else if (CoreWork.Instance.slotNGInfo[i].Result == "测高NG")
                                {
                                    buttonsNG[i].BackColor = Color.DarkOliveGreen;
                                }
                                else if (CoreWork.Instance.slotNGInfo[i].Result == "钛大比亚迪防呆NG")
                                {
                                    buttonsNG[i].BackColor = Color.CadetBlue;
                                }

                                NGTrayTemp[i] = 1;
                            }
                            else if (CoreWork.Instance.slotNGInfo[i].isExist == 0)
                            {
                                buttonsNG[i].BackColor = Color.Gray;
                                NGTrayTemp[i] = 0;
                            }
                        }
                    }
                    //更新Rest盘
                    for (int i = 0; i <= 35; i++)
                    {
                        if (CoreWork.Instance.RestSlotInfo[i].isExist != RestTrayTemp[i])
                        {
                            if (CoreWork.Instance.RestSlotInfo[i].isExist == 1)
                            {
                                buttonsRest[i].BackColor = Color.Green;
                                RestTrayTemp[i] = 1;
                            }
                            else if (CoreWork.Instance.RestSlotInfo[i].isExist == 0)
                            {
                                buttonsRest[i].BackColor = Color.Gray;
                                RestTrayTemp[i] = 0;
                            }
                        }
                    }

                    //切换到就绪时停止料仓工站动作
                    double sysStute = m_var.GetDValue("系统状态");
                    if (sysStute != sysStuteTemp)
                    {
                        sysStuteTemp = sysStute;
                        if (sysStute == 4)
                        {
                           m_motion.SetStatus(3, StSetType.Stop);
                           m_motion.SetStatus(4, StSetType.Stop);
                          
                        }
                    }
                    
                }
                 catch (Exception ex) { };

              
                Thread.Sleep(500);
            }
        }

        private void OpDlgClose()
        {
            m_tUpdateStatus.Stop();
        }

        public static OpDlg GetInstance()
        {
            if (null == m_Instance)
            {
                m_Instance = new OpDlg();
            }
            return m_Instance;
        }

        //监控系统、IO状态
        void UpdateStatus_Tick(object sender, EventArgs e)
        {
            long[] lLastStatus = new long[3] { -1, -1, -1 };
            try
            {
                {//更新系统状态
                    NProcUnit.SystemStatus curStatus = NProcUnit.Unit.GetInstance().SystemStatus;
                    if (m_PreSystemStatus != curStatus)
                    {
                        SetSystemStatus(NProcUnit.Unit.GetInstance().SystemStatus);
                        m_PreSystemStatus = curStatus;
                    }
                //更新IO状态
                    if (m_motion != null)
                    {
                        SetIOStatus();
                    }
                }
            }
            catch (System.Exception)
            {

            }
        }

        private void SetSystemStatus(NProcUnit.SystemStatus sts)
        {
            string text = "";
            Color txtColor = Color.Red;
            var str = sts.ToString();
            text += str;

			//string[] statusArr = { "未初始化", "流程报警", "未复位", "复位中", "就绪", "工作中", "暂停工作" };
            if (!string.IsNullOrEmpty(str) && !str.Equals(StatusText.Text))
            {//状态改变就更新
                switch (sts)
                {
                    case NProcUnit.SystemStatus.复位中:
                    case NProcUnit.SystemStatus.就绪:
                    case NProcUnit.SystemStatus.工作中:
                        txtColor = Color.Green;
                        break;
                    case NProcUnit.SystemStatus.暂停工作:
                        txtColor = Color.OrangeRed;
                        break;
                    default:
                        break;
                }
				StatusText.Text = sts.ToString();
                StatusText.ForeColor = txtColor;
            }
        }
      
        private void SetIOStatus()
        {
            short val = 0;
            short val2 = 0;
            val = m_motion.ReadIO("启动感应");
            SetBtnImage(buttonStartSin,val);
            val = m_motion.ReadIO("复位感应");
            SetBtnImage(buttonResetSin, val);
            val = m_motion.ReadIO("暂停感应");
            SetBtnImage(buttonPauseSin, val);
            val = m_motion.ReadIO("急停感应");
            SetBtnImage(buttonStopSin, val);

            val = m_motion.ReadIO("前门禁1");
            val2 = m_motion.ReadIO("前门禁2");

            if (val == 1 && val2 == 1)
                val = 1;
            else
            {
                val = 0;
            }
            SetBtnImage(buttonDoorSin, val);

            val = m_motion.ReadIO("侧门禁1");
            val2 = m_motion.ReadIO("侧门禁2");

            if (val == 1 && val2 == 1)
                val = 1;
            else
            {
                val = 0;
            }
            SetBtnImage(LeftDoorSin, val);

            val = m_motion.ReadIO("后门禁1");
            val2 = m_motion.ReadIO("后门禁2");
            if (val == 1 && val2 == 1)
                val = 1;
            else
            {
                val = 0;
            }
            SetBtnImage(BackDoorSin, val);

            val = m_motion.ReadIO("机器人前门禁");
            SetBtnImage(RbtForeDoorSin, val);

            val = m_motion.ReadIO("机器人后门禁");
            SetBtnImage(RbtBackDoorSin, val);
        }

        private void SetBtnImage(Button btn,short val)
        {
            btn.Image = ((val == 1) ? NsDemo.Properties.Resources.valid : NsDemo.Properties.Resources.unvalid);
        }

     

        //事件处理方法
        private int OnGetData(string name, object obj)
        {
            if (name.Equals("updateData"))
            {
                if (this.IsHandleCreated)
                {
                    this.Invoke(new Action(() =>
                    {
                        UpdateData(obj);
                    }));
                }
            }
            else if (name.Equals("UpdateTimeSpan"))
            {
                double time = (double)obj;
                m_stData._ct = time;
                this.Invoke(new Action(() =>
                {
                    ShowCurTotal();
                }));
            }
            else if (name.Equals("UpdateCount"))
            {
               

                this.Invoke(new Action(() =>
                {
                    ShowCurTotal();
                }));
            }
            else if (name.Equals("test"))
            {
                int num = (int)obj;
            }
			return 0;
        }

        public void UpdateData(object obj)
        {
            //注意类型转换，转换类型跟corwork传参类型一致
            m_stData = (StationData)obj;
            m_stData._total = m_stData._ok + m_stData._ng;
            m_stData._yeild = m_stData._ok / m_stData._total;
            if (this.IsHandleCreated)
            {
                this.Invoke(new Action(() =>
                {
                    ShowCurTotal();
                }));  
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
			var sts = m_motion.GetStatus(0);
			HardwareStatus ss = sts.AxisStatus[0].HardwareStatus;
			if (ss == HardwareStatus.Moving)
			{
				//进行操作
			}
			else if (ss == HardwareStatus.IDLE)
			{

			}
			else if (ss == HardwareStatus.Alarm)
			{

			}

			long vale = 0;
			m_motion.ReadSomeIO(0, out vale, NMotionCtrl.IOType.LimitP);

            labelTotal.Text = "投入总数： 0";//获取产出总数
            labelOutput.Text = "产出总数： 0"; 
            labelNg.Text = "次品总数： 0";//ng
            labelOK.Text = "良品总数： 0";//ok
            labelYield.Text = "良品率： 0";//Yield
            labelCt.Text = "周期： 0 s"; //ct

            m_var.SetDValue("投入", 0);
            m_var.SetDValue("产出", 0);
            m_var.SetDValue("次品", 0);

            return;

            
        }

        //设置统计数据
        private void ShowCurTotal()
        {
            double ALL = m_var.GetDValue("投入");
            double OK = m_var.GetDValue("产出");
            double NG = m_var.GetDValue("次品");


            labelTotal.Text = string.Format("投入总数： {0}", ALL);
            //获取产出总数
            labelOutput.Text = string.Format("产出总数： {0}", OK);
            //ng
            labelNg.Text = string.Format("次品总数： {0}", NG);
            //Yield
            labelYield.Text = $"良品率： {Math.Round(OK / ALL, 4) * 100}%";
            //ct
            labelCt.Text = string.Format("周期： {0:F3} s", m_stData._ct);
        }
  
        //按钮动作响应
        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            int buttonIndex = Array.IndexOf(buttonsPass,clickedButton);

            if (buttonIndex != -1)
            {
                int temp = CoreWork.Instance.slotInfo[buttonIndex].isExist == 1 ? 0 : 1;
                CoreWork.Instance.slotInfo[buttonIndex].isExist = temp;
                if (temp == 1)
                {
                    buttonsPass[buttonIndex].BackColor = Color.Green;
                }
                else if (temp == 0)
                {
                    buttonsPass[buttonIndex].BackColor = Color.Gray;
                }
                return;
            }
            buttonIndex = Array.IndexOf(buttonsNG, clickedButton);

            if (buttonIndex != -1)
            {
                int temp = CoreWork.Instance.slotNGInfo[buttonIndex].isExist == 1 ? 0 : 1;
                CoreWork.Instance.slotNGInfo[buttonIndex].isExist = temp;
                if (temp == 1)
                {
                    buttonsNG[buttonIndex].BackColor = Color.Green;
                }
                else if (temp == 0)
                {
                    buttonsNG[buttonIndex].BackColor = Color.Gray;
                }
                return;
            }

            buttonIndex = Array.IndexOf(buttonsRest, clickedButton);

            if (buttonIndex != -1)
            {
                int temp = CoreWork.Instance.RestSlotInfo[buttonIndex].isExist == 1 ? 0 : 1;
                CoreWork.Instance.RestSlotInfo[buttonIndex].isExist = temp;
                if (temp == 1)
                {
                    buttonsRest[buttonIndex].BackColor = Color.Green;
                }
                else if (temp == 0)
                {
                    buttonsRest[buttonIndex].BackColor = Color.Gray;
                }
                return;
            }
        }
        private void MouseEnterClick_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            int buttonIndex = Array.IndexOf(buttonsPass, clickedButton);
       
            if (buttonIndex != -1)
            {
                //SNT.Visible = true;
                //RT.Visible = true;
                //CARRYT.Visible = true;
                //INT.Visible = true;
                //OUT.Visible = true;
                //SCT.Visible = true;
                //WT.Visible = true;
                //TST.Visible = true;

                SNT.Text = CoreWork.Instance.slotInfo[buttonIndex].sn;
                RT.Text = CoreWork.Instance.slotInfo[buttonIndex].RFOKFlag;
                CARRYT.Text = CoreWork.Instance.slotInfo[buttonIndex].CarrySn;
                INT.Text = CoreWork.Instance.slotInfo[buttonIndex].InputTime;
                OUT.Text = CoreWork.Instance.slotInfo[buttonIndex].OutputTime;
                TST.Text = CoreWork.Instance.slotInfo[buttonIndex].TimeSpan;
                SCT.Text = CoreWork.Instance.slotInfo[buttonIndex].ScanTime;
                WT.Text = CoreWork.Instance.slotInfo[buttonIndex].WorkTime;
                HSGT.Text = CoreWork.Instance.slotInfo[buttonIndex].HSG;
                resultTxt.Text = CoreWork.Instance.slotInfo[buttonIndex].Result;
                return;
            }
            buttonIndex = Array.IndexOf(buttonsNG, clickedButton);

            if (buttonIndex != -1)
            {
                //SNT.Visible = true;
                //RT.Visible = true;
                //CARRYT.Visible = true;
                //INT.Visible = true;
                //OUT.Visible = true;
                //SCT.Visible = true;
                //WT.Visible = true;
                //TST.Visible = true;
                SNT.Text = CoreWork.Instance.slotNGInfo[buttonIndex].sn;
                RT.Text = CoreWork.Instance.slotNGInfo[buttonIndex].RFOKFlag;
                CARRYT.Text = CoreWork.Instance.slotNGInfo[buttonIndex].CarrySn;
                INT.Text = CoreWork.Instance.slotNGInfo[buttonIndex].InputTime;
                OUT.Text = CoreWork.Instance.slotNGInfo[buttonIndex].OutputTime;
                TST.Text = CoreWork.Instance.slotNGInfo[buttonIndex].TimeSpan;
                SCT.Text = CoreWork.Instance.slotNGInfo[buttonIndex].ScanTime;
                WT.Text = CoreWork.Instance.slotNGInfo[buttonIndex].WorkTime;
                HSGT.Text = CoreWork.Instance.slotNGInfo[buttonIndex].HSG;
                resultTxt.Text = CoreWork.Instance.slotNGInfo[buttonIndex].Result;
                return;
            }
        }
        private void GenerateButtonMatrix4PassTray()
        {
            int NumRows = 5;
            int NumCols = 5;
            int buttonWidth = 54;
            int buttonHeight = 47;
            int horizontalGap = 10;
            int verticalGap = 10;
            int startX = 0;
            int startY = 0;
            int index = 0;
            for (int row = 0; row < NumRows; row++)
            {
                for (int col = 0; col < NumCols; col++)
                {
                    Button button = new Button();
                    button.Text = $"{index + 1}";
                    button.Width = buttonWidth;
                    button.Height = buttonHeight;
                    button.Left = startX + (buttonWidth + horizontalGap) * col;
                    button.Top = startY + (buttonHeight + verticalGap) * row;
                    panel3.Controls.Add(button);
                    buttonsPass[index] = button;
                    button.Click += Button_Click;
                    button.MouseEnter += MouseEnterClick_Click;
                    index++;
                }
            }
        }
        private void GenerateButtonMatrix4RestTray()
        {
            int NumRows = 4;
            int NumCols = 9;
            int buttonWidth = 47;
            int buttonHeight = 47;
            int horizontalGap = 5;
            int verticalGap = 5;
            int startX = 0;
            int startY = 0;
            int index = 0;
            for (int row = 0; row < NumRows; row++)
            {
                for (int col = 0; col < NumCols; col++)
                {
                    Button button = new Button();
                    button.Text = $"{index + 1}";
                    button.Width = buttonWidth;
                    button.Height = buttonHeight;
                    button.Left = startX + (buttonWidth + horizontalGap) * col;
                    button.Top = startY + (buttonHeight + verticalGap) * row;
                    panel5.Controls.Add(button);
                    buttonsRest[index] = button;
                    button.Click += Button_Click;
                    //   button.MouseEnter += MouseEnterClick_Click;
                    index++;
                }
            }
        }
        private void GenerateButtonMatrix4NGTray()
        {
            int NumRows = 5;
            int NumCols = 5;
            int buttonWidth = 54;
            int buttonHeight = 47;
            int horizontalGap = 10;
            int verticalGap = 10;
            int startX = 0;
            int startY = 0;
            int index = 0;
            for (int row = 0; row < NumRows; row++)
            {
                for (int col = 0; col < NumCols; col++)
                {
                    Button button = new Button();
                    button.Text = $"{index + 1}";
                    button.Width = buttonWidth;
                    button.Height = buttonHeight;
                    button.Left = startX + (buttonWidth + horizontalGap) * col;
                    button.Top = startY + (buttonHeight + verticalGap) * row;
                    panel4.Controls.Add(button);
                    buttonsNG[index] = button;
                    button.Click += Button_Click;
                    button.MouseEnter += MouseEnterClick_Click;
                    index++;
                }
            }
        }

        private void btnCleanPassCount_Click(object sender, EventArgs e)
        {
            //更新pass盘
            if (DialogResult.OK == MessageBox.Show("确定要收料盘重置吗？", "提示", MessageBoxButtons.OKCancel))
            {
                for (int i = 0; i <= 24; i++)
                {
                    CoreWork.Instance.slotInfo[i] = new Slot() { isExist = 0 };
                    PassTrayTemp[i] = -1;
                }
            }
         
        }

        private void btnCleanNGCount_Click(object sender, EventArgs e)
        {
            //更新NG盘
            if (DialogResult.OK == MessageBox.Show("确定要NG盘重置吗？", "提示", MessageBoxButtons.OKCancel))
            {
                for (int i = 0; i <= 24; i++)
                {
                    CoreWork.Instance.slotNGInfo[i] = new Slot() { isExist = 0 };
                    NGTrayTemp[i] = -1;
                }
            }
        }

        private void btnPutOut_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("确定要收料盘出料吗？", "提示", MessageBoxButtons.OKCancel))
            {
                //设置pass盘满
                for (int i = 0; i <= 24; i++)
                {
                    CoreWork.Instance.slotInfo[i].isExist = 1;
                }
            }
        }

        private void rBtnLG_CheckedChanged(object sender, EventArgs e)
        {
            m_var.SetDValue(46, rBtnLG.Checked ? 1 : 0);
        }

        private void btnFinishWork_Click(object sender, EventArgs e)
        {

            if (DialogResult.OK == MessageBox.Show("确定满料仓出料吗？", "提示", MessageBoxButtons.OKCancel))
            {
                double SysSt = m_var.GetDValue("系统状态");
                double LCachSt = m_var.GetDValue("满料仓可出料标记");
                if (SysSt == 4 || LCachSt == 1)
                {
                    //ProcStatus procStatus = new ProcStatus();
                    //Unit.GetInstance().PGetProcStatus()
                    Unit.GetInstance().PStop(18);
                    Unit.GetInstance().PRun(18);
                }
                else
                {
                    MessageBox.Show("系统未就绪");
                    return;
                }
            }
            else
            {
                return;
            }

        }
        private void btnStopOutput_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("确定停止出料吗？", "提示", MessageBoxButtons.OKCancel))
            {
                Unit.GetInstance().PStop(18);
                m_var.SetDValue("空料仓暂停标记", 0);
                m_var.SetDValue("暂停变量", 0);
                MotionCtrl.GetInstance().WriteIO("满盘接驳电机5", 0);
            }
            else
            {
                return;
            }
        }
        public DebugUI_UPH m_DebugUI_UPH = new DebugUI_UPH();
        private void btnProductInfo_Click(object sender, EventArgs e)
        {
            m_DebugUI_UPH.StartPosition = FormStartPosition.CenterScreen;
            m_DebugUI_UPH.Show();
            m_DebugUI_UPH.BringToFront();
            m_DebugUI_UPH.WindowState = FormWindowState.Normal;
        }

        private void btnSetMode_Click(object sender, EventArgs e)
        {
            double Model = m_var.GetDValue("模式选择Flg");
            if(Model == 0)
            {
                btnSetMode.BackColor = Color.Green;
                btnSetMode.Text = "生产模式";
                m_var.SetDValue("模式选择Flg",1);

                m_var.SetDValue("跳过扫码", 0);
                m_var.SetDValue("启用6轴Rbt标记", 1);
                m_var.SetDValue("启用围栏门禁信号标记", 1);
                m_var.SetDValue("空取空放标记", 0);
                m_var.SetDValue("RFID启用标记", 1);
                m_var.SetDValue("不换盘启用标记", 0);
                m_var.SetDValue("MES启用标记", 1);
                m_var.SetDValue("料盘分区", 1);
                m_var.SetDValue("屏蔽出烤炉时间", 0);
            }
            else
            {
                btnSetMode.BackColor = Color.Yellow;
                btnSetMode.Text = "调机模式";
                m_var.SetDValue("模式选择Flg", 0);
            }
        }

        public enum Level
        {
            Error = 0,
            Normal,
        }
        // InfoLevel 信息级别
        // 0 红色报警
        // 1 普通信息
        public void PrintInfo(string str, Level InfoLevel)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    int length = MsgBox.TextLength;
                    str = $"[{DateTime.Now.ToString("yyyy-MM-dd HH时mm分ss秒")}]：{str}\r\n";
                    MsgBox.AppendText(str);

                    Color color = Color.White;
                    if (InfoLevel == Level.Error)
                    {
                        color = Color.Red;
                    }
                    else if (InfoLevel == Level.Normal)
                    {
                        color = Color.White;
                    }
                    MsgBox.Select(length, str.Length);
                    MsgBox.SelectionBackColor = color;
                    MsgBox.ScrollToCaret();
                }));
            }
            catch (Exception ex)
            {

            }
        }

        private void btnVirtualIO_Click(object sender, EventArgs e)
        {
            if (ui_VirtualIO != null)
            {
                ui_VirtualIO.Show();
                ui_VirtualIO.Start();
            }
        }

        private void btnRbtDoorFore_Click(object sender, EventArgs e)
        {
            short val = 0;
            val = m_motion.ReadIO("机器人前门禁开关");
            if(val == 0)
            {
                m_motion.WriteIO("机器人前门禁开关",1);
                btnRbtDoorFore.BackColor = Color.Green;
            }
            else
            {
                m_motion.WriteIO("机器人前门禁开关", 0);
                btnRbtDoorFore.BackColor = Color.Red;
            }
        }

        private void btnRbtDoorBehind_Click(object sender, EventArgs e)
        {
            short val = 0;
            val = m_motion.ReadIO("机器人后门禁开关");
            if (val == 0)
            {
                m_motion.WriteIO("机器人后门禁开关", 1);
                btnRbtDoorBehind.BackColor = Color.Green;
            }
            else
            {
                m_motion.WriteIO("机器人后门禁开关", 0);
                btnRbtDoorBehind.BackColor = Color.Red;
            }
        }

        private void ForeDoor_Click(object sender, EventArgs e)
        {
            short val = 0;
            val = m_motion.ReadIO("前门禁");
            if (val == 0)
            {
                m_motion.WriteIO("前门禁", 1);
                ForeDoor.BackColor = Color.Green;
            }
            else
            {
                m_motion.WriteIO("前门禁", 0);
                ForeDoor.BackColor = Color.Red;
            }
        }

        private void LeftDoor_Click(object sender, EventArgs e)
        {
            short val = 0;
            val = m_motion.ReadIO("侧门禁");
            if (val == 0)
            {
                m_motion.WriteIO("侧门禁", 1);
                LeftDoor.BackColor = Color.Green;
            }
            else
            {
                m_motion.WriteIO("侧门禁", 0);
                LeftDoor.BackColor = Color.Red;
            }
        }

        private void BackDoor_Click(object sender, EventArgs e)
        {
            short val = 0;
            val = m_motion.ReadIO("后门禁");
            if (val == 0)
            {
                m_motion.WriteIO("后门禁", 1);
                BackDoor.BackColor = Color.Green;
            }
            else
            {
                m_motion.WriteIO("后门禁", 0);
                BackDoor.BackColor = Color.Red;
            }
        }

        private void btnTexClear_Click(object sender, EventArgs e)
        {
            try
            {
                MsgBox.Clear();
            }
            catch (Exception ex)
            {

            }
        }

        private void MESBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (MESBox.Checked)
            {
                m_var.SetDValue("MES启用标记", 1);
            }
            else
            {
                m_var.SetDValue("MES启用标记", 0);
            }
        }

        private void btnResetShelter_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("确定要盖板缓冲台重置吗？", "提示", MessageBoxButtons.OKCancel))
            {
                for (int i = 0; i <= 4; i++)
                {
                    CoreWork.Instance.RestSlotInfo[i] = new RestSlot() { isExist = 0 };
                    RestTrayTemp[i] = -1;
                }
            }
        }

        private void btnResetModel_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("确定要工装缓冲台重置吗？", "提示", MessageBoxButtons.OKCancel))
            {
                for (int i = 5; i <= 35; i++)
                {
                    CoreWork.Instance.RestSlotInfo[i] = new RestSlot() { isExist = 0 };
                    RestTrayTemp[i] = -1;
                }
            }
        }
    }
}
