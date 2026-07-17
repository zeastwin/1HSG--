using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using NProcUnit;
using NProc;
using NVarConfig;
using NLog;
using NDataBase;
using NStructConfig;
using NMotionCtrl;

using NsDemo.RFIDSys;
using System.IO;
using NsDemo.Product;
using static NsDemo.OpDlg;
using NsDemo.Utility;
using Newtonsoft.Json;
using NsDemo.MESSys;
using System.Net;
using Newtonsoft.Json.Linq;
using NsDemo.MesSys;
using NsDemo.PCode;
using NsDemo.Virtual_IO;
using Result = NsDemo.MESSys.Result;

namespace NsDemo
{
    public delegate int SendDataHandler(string name, object obj);

    public class CoreWork
    {
        public event SendDataHandler SendDatas;//事件

        private Unit _procunit;
        private VarInteface m_reg = VarInteface.GetInstance();
        private StructManage m_structMgr = StructManage.GetInstance();

        private DataBaseInterface m_dataBase = DataBaseInterface.GetInstance();

        int n = 50;
        //private OpDlg m_op = OpDlg.GetInstance();

        public static CoreWork Instance { get; private set; }


        public static int startNumber = 0;
        public static int endNumber = 0;
        DateTime[] ctStart = new DateTime[7];
        DateTime[] ctEnd = new DateTime[7];
        private RFTool _RFTool = new RFTool();
        //Trac模板信息
        public bc_window_assy_mesinfo bcBaseInfo = new bc_window_assy_mesinfo();
        static CoreWork()
        {
            Instance = new CoreWork();
        }

        private CoreWork()
        {
            _procunit = Unit.GetInstance();
            _procunit.RegistNotifyEvent(Notify);
            InitSlotInfo();
            InitSlotNGInfo();
            _RFTool.Init();
        }

        /// <summary>
        /// 回调到上一层处理, 比如OpDlg里面
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        private void Do(string name, object obj)
        {
            SendDatas(name, obj);
        }

        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        private int Notify(NotifyInfo info)
        {
            if (info != null)
            {
                return PtProcess(info);
            }
            return 0;
        }

        private int PtProcess(NotifyInfo info)
        {

            //判断跳转，判断名称，有重名指令，则多条同名称指令执行同一处理
            //跳转唯一，需要判断流程，步骤，点位id
            if (info.Name.Equals("发送消息0"))
            {//所有点位指令名称为发送消息0的都会从这边处理
                //sleep(1000);
                m_reg.SetDValue(4, m_reg.GetDValue(4) + 1);

                //更新数据到OP
                StationData st = new StationData();
                st._ng = 100;
                st._ok = n++;
                st._ct = 0.5;
                //m_op.UpdateData(st as object);
                Do("updateData", st as object); //用实例去调用
                return 0;
            }
            if (info.Name.Equals("发送消息1") && info.StepID == 0 && info.PointID == 1)
            {
                //sleep(5000);
                m_reg.SetDValue(5, m_reg.GetDValue(5) + 1);
                Do("test", 555);
                _procunit.PJumpStep(info.ProcID, (short)info.StepID, (short)(info.PointID + 1));
            }
            if (info.ProcID == 0 && info.StepID == 0 && info.PointID == 4)//流程id需要对应指令上的流程id，可修改，proid默认为3112356，后改为0
            {
                m_reg.SetDValue(6, m_reg.GetDValue(6) + 1);
                _procunit.PJumpStep(info.ProcID, 0, 0);
            }
            if (info.Name.Equals("数据库操作"))
            {
                // 创建表
                ColParam[] col = new ColParam[3] { new ColParam(), new ColParam(), new ColParam() };
                col[0].colName = "日期";
                col[0].colType = SQLiteTypeEnum.String;
                col[1].colName = "SN";
                col[1].colType = SQLiteTypeEnum.String;
                col[2].colName = "VAL";
                col[2].colType = SQLiteTypeEnum.String;
                string tablename = DateTime.Now.ToString("yyyy-MM-dd");
                m_dataBase.CreateTable(tablename, col);
                //或者可以这样创建
                bool res = false;
                m_dataBase.IsTableExist("2022-09-14", ref res);

                m_dataBase.ExecuteSQLStatement("create table fil11m(title, length, year, starring)");
                _procunit.PJumpStep(info.ProcID, 0, 0);
            }
            if (info.Name.Equals("发送消息2"))
            {
                _procunit.PJumpStep(info.ProcID, (short)info.StepID, (short)(info.PointID + 1));
            }
            if (info.Name.Equals("发送处理消息Pan"))
            {
                int proc = info.ProcID;
                _procunit.PJumpStep(proc, (short)info.StepID, (short)(info.PointID + 2));
            }
            else if (info.Name.Contains("消息 PASS++") || info.Name.Contains("消息 NG++"))
            {
                int structID = (int)m_reg.GetDValue("产量数据");
                if (structID <= 0)
                    return 0;
                NStruct stData = m_structMgr.StructList[(short)structID];
                if (stData.ItemCount != 25)
                    return 0;

                string name = stData.ItemList[0].DataList[0].Cval;
                string strDate = DateTime.Now.ToString("yyyyMMdd");
                if (string.IsNullOrEmpty(name) || !name.Contains(strDate))
                {
                    Do("DataUph.SaveYieldData", null);
                    m_structMgr.SetStructCval((short)structID, 0, 0, strDate);
                    for (int i = 1; i <= 24; i++)
                    {
                        m_structMgr.SetStructDval((short)structID, (short)i, 0, 0);
                        m_structMgr.SetStructDval((short)structID, (short)i, 1, 0);
                    }
                }

                double dok = 0, dng = 0;
                m_structMgr.GetStructDval((short)structID, (short)(DateTime.Now.Hour + 1), 0, ref dok);
                m_structMgr.GetStructDval((short)structID, (short)(DateTime.Now.Hour + 1), 1, ref dng);
                if (info.Name.Contains("消息 PASS++"))
                    m_structMgr.SetStructDval((short)structID, (short)(DateTime.Now.Hour + 1), 0, ++dok);
                else if (info.Name.Contains("消息 NG++"))
                    m_structMgr.SetStructDval((short)structID, (short)(DateTime.Now.Hour + 1), 01, ++dng);
                _procunit.PJumpStep(info.ProcID, (short)info.StepID, (short)(info.PointID + 1));
            }
            if (info.Name.Equals("初始化收料盘信息"))
            {
                InitSlotInfo();
            }
            if (info.Name.Equals("计算收料盘运动位置"))
            {
                CalMovPosAndEnableSucker();
            }
            if (info.Name.Equals("寻找收料盘第一个可用的空穴位"))
            {
                int Num = FindAvailableSlot();
                OpDlg.GetInstance().m_var.SetDValue(33, Num);
            }
            if (info.Name.Equals("更新收料盘穴位状态"))
            {
                UpdateSlotInfo();
            }
            if (info.Name.Contains("检查收料盘是否已满"))
            {
                int Num = FindAvailableSlot();
                OpDlg.GetInstance().m_var.SetDValue(32, Num);
            }
            if (info.Name.Equals("初始化NG料盘信息"))
            {
                InitSlotNGInfo();
            }
            if (info.Name.Equals("寻找NG盘第一个可用的空穴位"))
            {
                int Num = FindAvailableSlotNG();
                OpDlg.GetInstance().m_var.SetDValue(34, Num);
            }
            if (info.Name.Equals("计算NG料盘运动位置"))
            {
                CalMovPosAndEnableSuckerNG();
            }
            if (info.Name.Equals("更新NG料盘穴位状态"))
            {
                UpdateSlotNGInfo();
            }
            if (info.Name.Equals("更新缓冲台穴位状态"))
            {
                UpdateRestSlotNGInfo();
            }
            if (info.Name.Equals("清空缓冲台穴位状态"))
            {
                ClearRestSlotNGInfo();
            }
            if (info.Name.Contains("寻找盖板缓冲空穴位"))
            {
                int Num = FindAvailableSlotRestS();
                OpDlg.GetInstance().m_var.SetDValue("缓冲台可用穴序号", Num);
            }
            if (info.Name.Contains("寻找工装缓冲空穴位"))
            {
                int Num = FindAvailableSlotRestM();
                OpDlg.GetInstance().m_var.SetDValue("缓冲台可用穴序号", Num);
            }
            if (info.Name.Contains("寻找盖板缓冲有料穴位"))
            {
                int Num = FindAvailableSlotRestExistS();
                OpDlg.GetInstance().m_var.SetDValue("缓冲台可用穴序号", Num);
            }
            if (info.Name.Contains("寻找工装缓冲有料穴位"))
            {
                int Num = FindAvailableSlotRestExistM();
                OpDlg.GetInstance().m_var.SetDValue("缓冲台可用穴序号", Num);
            }
            if (info.Name.Contains("开始计算CT"))
            {
                ctStart[startNumber] = DateTime.Now;
                startNumber++;
                if (startNumber > 6)
                {
                    startNumber = 0;
                }
            }
            if (info.Name.Contains("结束计算CT"))
            {
                ctEnd[endNumber] = DateTime.Now;
                TimeSpan res = ctEnd[endNumber] - ctStart[endNumber];
                SendDatas.Invoke("UpdateTimeSpan", res.TotalSeconds);
                endNumber++;
                if (endNumber > 6)
                {
                    endNumber = 0;
                }
            }
            if (info.Name.Contains("寻找NG盘吸头1空穴位"))
            {
                int Num = FindAvailableSlotNG1();
                OpDlg.GetInstance().m_var.SetDValue(34, Num);
            }
            if (info.Name.Contains("寻找NG盘吸头2空穴位"))
            {
                int Num = FindAvailableSlotNG2();
                OpDlg.GetInstance().m_var.SetDValue(34, Num);
            }

            //工装数据清除(60003,1000,3,1,100,RF清除成功标志)
            //参数(通讯ID,超时时间,写入失败次数,开始清除地址,清除数据长度,RF清除成功标志[寄存器])
            if (info.Name.Contains("工装数据清除"))
            {
                _RFTool.RF_Clear(info.Name);
                return 0;
            }

            //工装数据获取(60002,1000,3,获取的工装数据,HSG码,穴位1-BC码,穴位2-BC码,穴位3-BC码,穴位4-BC码)
            //参数(通讯ID, 读取超时时间, 读取失败继续读取几次, 获取到的数据集合[寄存器], 需要读取的数据项1[配置文件项], 需要读取的数据项2[配置文件项], ...)
            if (info.Name.Contains("工装数据获取"))
            {
                _RFTool.ReadRFData(info.Name);
                return 0;
            }

            //工装数据写入(60003,1000,5,RF写入成功标志位,HSG码,HSG码)
            //参数(通讯ID, 读取超时时间, 读取失败继续读取几次, 写入成功标志[寄存器], 需要写入的数据项1[配置文件项], 待写入的数据项数据1[寄存器], 需要写入的数据项2[配置文件项], 待写入的数据项数据2[寄存器] ...)
            if (info.Name.Contains("工装数据写入"))
            {
                _RFTool.RF_WriteData(info.Name);
                return 0;
            }
            if (info.Name.Contains("检查烤炉时长并赋值扫码SN"))
            {
                double IgnoreTime = OpDlg.GetInstance().m_var.GetDValue("忽略烤炉时间");
                if (IgnoreTime > 0)
                {
                    OpDlg.GetInstance().m_var.SetDValue("抛NG标记", 0);
                    for (int i = 68; i < 72; i++)
                    {
                        string SN = OpDlg.GetInstance().m_var.GetCValue(i);
                        if (SN == "NA")
                            OpDlg.GetInstance().m_var.SetCValue(i - 40, "NULL");
                    }
                    OpDlg.GetInstance().m_var.SetCValue("烤炉用时", "40");
                    GlobalEvent.ShowMsg("忽略烤炉时间！", OpDlg.Level.Normal);
                    return 0;
                }
                try
                {
                    try
                    {
                        string timeInput = OpDlg.GetInstance().m_var.GetCValue("进烤炉时间");
                        string timeOutput = OpDlg.GetInstance().m_var.GetCValue("出烤炉时间");
                        double TimeCount = OpDlg.GetInstance().m_var.GetDValue("烤炉时长设定");
                        double TimeCountUpLimit = OpDlg.GetInstance().m_var.GetDValue("烤炉时长上限设定");
                        string BeforeStationRFFlag = OpDlg.GetInstance().m_var.GetCValue("上站RFID写入动作标记");
                        double RFIDFlag = OpDlg.GetInstance().m_var.GetDValue("上站RFID写入动作标记判断点");
                        double RFIDEnableFlag = OpDlg.GetInstance().m_var.GetDValue("上站RFID写入动作标记判断点启用标记");

                        DateTime dtInput = DateTime.ParseExact(timeInput, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime dtOutput = DateTime.ParseExact(timeOutput, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                        TimeSpan timeSpan = dtOutput - dtInput;
                        OpDlg.GetInstance().m_var.SetCValue("烤炉用时", Math.Round(timeSpan.TotalMinutes, 2).ToString());
                        if (timeSpan.TotalMinutes >= TimeCount && timeSpan.TotalMinutes <= TimeCountUpLimit)
                        {
                            OpDlg.GetInstance().m_var.SetDValue("抛NG标记", 0);
                            if (RFIDEnableFlag == 1 && BeforeStationRFFlag != RFIDFlag.ToString())
                            {
                                OpDlg.GetInstance().m_var.SetDValue("抛NG标记", 3);
                                for (int i = 68; i < 72; i++)
                                {
                                    OpDlg.GetInstance().m_var.SetCValue(i - 40, "NG");
                                }
                                return 0;
                            }
                        }
                        else
                        {
                            OpDlg.GetInstance().m_var.SetDValue("抛NG标记", 2);
                        }
                    }
                    catch (Exception ex)
                    {
                        //   MessageBox.Show("时间读取异常");
                        OpDlg.GetInstance().m_var.SetDValue("抛NG标记", 2);
                    }

                    for (int i = 68; i < 72; i++)
                    {
                        string SN = OpDlg.GetInstance().m_var.GetCValue(i);
                        if (SN == "NA")
                            OpDlg.GetInstance().m_var.SetCValue(i - 40, "NULL");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("检查烤炉时长并赋值扫码SN报警：" + ex.Message);
                }
            }
            if (info.Name.Contains("获取待写入出烤炉时间"))
            {
                string dt = DateTime.Now.ToString("yyyyMMddHHmmss");
                OpDlg.GetInstance().m_var.SetCValue("待写入出烤炉时间", dt);
            }
            if (info.Name.Contains("获取扫码时间"))
            {
                string dt = DateTime.Now.ToString("yyyyMMddHHmmss");
                OpDlg.GetInstance().m_var.SetCValue("扫码时间", dt);
            }
            if (info.Name.Contains("获取收料时间"))
            {
                string dt = DateTime.Now.ToString("yyyyMMddHHmmss");
                OpDlg.GetInstance().m_var.SetCValue("收料时间", dt);
            }
            if (info.Name.Contains("SN写入CSV"))
            {
                WriteCsv();
            }
            if (info.Name.Contains("扫码失败SN写入CSV"))
            {
                WriteCsv4FailSN();
            }
            if (info.Name.Contains("RFID信息写入CSV"))
            {
                WriteCsv4RFID();
            }
            if (info.Name.Contains("扫码Z轴写入CSV"))
            {
                WriteCsv4ScanZ();
            }
            if (info.Name.Contains("更新收料盘穴位SN1"))
            {
                try
                {
                    string SN = OpDlg.GetInstance().m_var.GetCValue("吸头1SNTemp");
                    string RFOKFlag = "";
                    string CarrySn = "";
                    string HSG = "";
                    string InputTime = "";
                    string OutputTime = "";
                    string TimeSpan = "";
                    string ScanTime = "";
                    string WorkTime = "";
                    double ResultType = -1;
                    string ResultStr = "";
                    string BStationResult = ""; //点胶工站结果

                    m_structMgr.GetStructCval(2, 0, 4, ref RFOKFlag);
                    m_structMgr.GetStructCval(2, 0, 5, ref CarrySn);


                    slotInfo[MovSlotIndex].sn = SN;

                    double GetMaterialTime = OpDlg.GetInstance().m_var.GetDValue("取料次数");
                    if (GetMaterialTime == 1)
                    {
                        m_structMgr.GetStructCval(2, 0, 7, ref HSG);
                        m_structMgr.GetStructCval(2, 0, 26, ref BStationResult);
                    }
                    else if (GetMaterialTime == 2)
                    {
                        m_structMgr.GetStructCval(2, 0, 9, ref HSG);
                        m_structMgr.GetStructCval(2, 0, 28, ref BStationResult);
                    }
                    m_structMgr.GetStructCval(2, 0, 10, ref InputTime);
                    m_structMgr.GetStructCval(2, 0, 11, ref OutputTime);
                    m_structMgr.GetStructCval(2, 0, 12, ref TimeSpan);
                    m_structMgr.GetStructCval(2, 0, 13, ref ScanTime);
                    m_structMgr.GetStructCval(2, 0, 14, ref WorkTime);
                    m_structMgr.GetStructDval(2, 0, 15, ref ResultType);

                    if (BStationResult == "NG")
                    {
                        ResultStr = "点胶工站NG";
                    }
                    else
                    {
                        if (ResultType == 0)
                        {
                            if (SN != "NG")
                            {
                                ResultStr = "OK";
                            }
                            else
                            {
                                ResultStr = "玻璃扫码异常抛NG";
                            }
                        }
                        else if (ResultType == 1)
                        {
                            ResultStr = "RFID读取失败抛NG";
                        }
                        else if (ResultType == 2)
                        {
                            ResultStr = "隧道炉时间异常抛NG";
                        }
                        else if (ResultType == 3)
                        {
                            ResultStr = "点胶工站写RFID异常抛NG";
                        }
                    }
                    slotInfo[MovSlotIndex].sn = SN;
                    slotInfo[MovSlotIndex].RFOKFlag = RFOKFlag;
                    slotInfo[MovSlotIndex].CarrySn = CarrySn;
                    slotInfo[MovSlotIndex].HSG = HSG;
                    slotInfo[MovSlotIndex].InputTime = InputTime;
                    slotInfo[MovSlotIndex].OutputTime = OutputTime;
                    slotInfo[MovSlotIndex].TimeSpan = TimeSpan;
                    slotInfo[MovSlotIndex].ScanTime = ScanTime;
                    slotInfo[MovSlotIndex].WorkTime = WorkTime;
                    slotInfo[MovSlotIndex].Result = ResultStr;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("更新穴位状态异常：" + ex.Message);
                }

            }
            if (info.Name.Contains("更新收料盘穴位SN2"))
            {
                try
                {
                    string SN = OpDlg.GetInstance().m_var.GetCValue("吸头2SNTemp");
                    slotInfo[MovSlotIndex].sn = SN;

                    string RFOKFlag = "";
                    string CarrySn = "";
                    string HSG = "";
                    string InputTime = "";
                    string OutputTime = "";
                    string TimeSpan = "";
                    string ScanTime = "";
                    string WorkTime = "";
                    double ResultType = -1;
                    string ResultStr = "";
                    string BStationResult = ""; //点胶工站结果
                    m_structMgr.GetStructCval(2, 0, 4, ref RFOKFlag);
                    m_structMgr.GetStructCval(2, 0, 5, ref CarrySn);

                    double GetMaterialTime = OpDlg.GetInstance().m_var.GetDValue("取料次数");
                    if (GetMaterialTime == 1)
                    {
                        m_structMgr.GetStructCval(2, 0, 6, ref HSG);
                        m_structMgr.GetStructCval(2, 0, 25, ref BStationResult);
                    }
                    else if (GetMaterialTime == 2)
                    {
                        m_structMgr.GetStructCval(2, 0, 8, ref HSG);
                        m_structMgr.GetStructCval(2, 0, 27, ref BStationResult);
                    }
                    m_structMgr.GetStructCval(2, 0, 10, ref InputTime);
                    m_structMgr.GetStructCval(2, 0, 11, ref OutputTime);
                    m_structMgr.GetStructCval(2, 0, 12, ref TimeSpan);
                    m_structMgr.GetStructCval(2, 0, 13, ref ScanTime);
                    m_structMgr.GetStructCval(2, 0, 14, ref WorkTime);
                    m_structMgr.GetStructDval(2, 0, 15, ref ResultType);

                    if (BStationResult == "NG")
                    {
                        ResultStr = "点胶工站NG";
                    }
                    else
                    {
                        if (ResultType == 0)
                        {
                            if (SN != "NG")
                            {
                                ResultStr = "OK";
                            }
                            else
                            {
                                ResultStr = "玻璃扫码异常抛NG";
                            }
                        }
                        else if (ResultType == 1)
                        {
                            ResultStr = "RFID读取失败抛NG";
                        }
                        else if (ResultType == 2)
                        {
                            ResultStr = "隧道炉时间异常抛NG";
                        }
                        else if (ResultType == 3)
                        {
                            ResultStr = "点胶工站写RFID异常抛NG";
                        }
                    }

                    slotInfo[MovSlotIndex].RFOKFlag = RFOKFlag;
                    slotInfo[MovSlotIndex].CarrySn = CarrySn;
                    slotInfo[MovSlotIndex].HSG = HSG;
                    slotInfo[MovSlotIndex].InputTime = InputTime;
                    slotInfo[MovSlotIndex].OutputTime = OutputTime;
                    slotInfo[MovSlotIndex].TimeSpan = TimeSpan;
                    slotInfo[MovSlotIndex].ScanTime = ScanTime;
                    slotInfo[MovSlotIndex].WorkTime = WorkTime;
                    slotInfo[MovSlotIndex].Result = ResultStr;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("更新穴位状态异常：" + ex.Message);
                }


            }
            if (info.Name.Contains("更新NG盘穴位SN1"))
            {

                try
                {
                    string SN = OpDlg.GetInstance().m_var.GetCValue("吸头1SNTemp");
                    slotNGInfo[MovNGSlotIndex].sn = SN;

                    string RFOKFlag = "";
                    string CarrySn = "";
                    string HSG = "";
                    string InputTime = "";
                    string OutputTime = "";
                    string TimeSpan = "";
                    string ScanTime = "";
                    string WorkTime = "";
                    double ResultType = -1;
                    string ResultStr = "";
                    string BStationResult = ""; //点胶工站结果
                    string mesResult = "";         //Mes上传结果
                    string bydResult = "";         //钛大比亚迪防呆结果

                    m_structMgr.GetStructCval(2, 0, 4, ref RFOKFlag);
                    m_structMgr.GetStructCval(2, 0, 5, ref CarrySn);

                    double GetMaterialTime = OpDlg.GetInstance().m_var.GetDValue("取料次数");
                    double MESFlag = OpDlg.GetInstance().m_var.GetDValue("MES启用标记");
                    if (GetMaterialTime == 1)
                    {
                        m_structMgr.GetStructCval(2, 0, 7, ref HSG);
                        m_structMgr.GetStructCval(2, 0, 26, ref BStationResult);
                        m_structMgr.GetStructCval(2, 0, 18, ref mesResult);
                        m_structMgr.GetStructCval(2, 0, 22, ref bydResult);
                    }
                    else if (GetMaterialTime == 2)
                    {
                        m_structMgr.GetStructCval(2, 0, 9, ref HSG);
                        m_structMgr.GetStructCval(2, 0, 28, ref BStationResult);
                        m_structMgr.GetStructCval(2, 0, 20, ref mesResult);
                        m_structMgr.GetStructCval(2, 0, 24, ref bydResult);
                    }
                    m_structMgr.GetStructCval(2, 0, 10, ref InputTime);
                    m_structMgr.GetStructCval(2, 0, 11, ref OutputTime);
                    m_structMgr.GetStructCval(2, 0, 12, ref TimeSpan);
                    m_structMgr.GetStructCval(2, 0, 13, ref ScanTime);
                    m_structMgr.GetStructCval(2, 0, 14, ref WorkTime);
                    m_structMgr.GetStructDval(2, 0, 15, ref ResultType);


                    if (BStationResult.Contains("NG"))
                    {
                        ResultStr = BStationResult;
                    }
                    else
                    {
                        if (ResultType == 0)
                        {
                            if (SN != "NG")
                            {
                                if (MESFlag == 1 && mesResult != "OK")
                                {
                                    ResultStr = "MES异常抛NG";
                                }
                                else
                                {
                                    //if (mesh_lot == "NG")
                                    //{
                                    //    Result = "防尘网抛NG";
                                    //    _input += 1;
                                    //    _ngOut += 1;
                                    //}


                                    ResultStr = "OK";
                                }
                            }
                            else
                            {
                                ResultStr = "玻璃扫码异常抛NG";
                            }
                        }
                        else if (ResultType == 1)
                        {
                            ResultStr = "RFID读取失败抛NG";
                        }
                        else if (ResultType == 2)
                        {
                            ResultStr = "隧道炉时间异常抛NG";
                        }
                        else if (ResultType == 3)
                        {
                            ResultStr = "点胶工站写RFID异常抛NG";
                        }
                        else if (ResultType == 4)
                        {
                            ResultStr = "点胶-保压时间异常抛NG";
                        }
                    }
                    if (bydResult == "NG")
                    {
                        ResultStr = "钛大比亚迪防呆NG";
                    }

                    slotNGInfo[MovNGSlotIndex].RFOKFlag = RFOKFlag;
                    slotNGInfo[MovNGSlotIndex].CarrySn = CarrySn;
                    slotNGInfo[MovNGSlotIndex].HSG = HSG;
                    slotNGInfo[MovNGSlotIndex].InputTime = InputTime;
                    slotNGInfo[MovNGSlotIndex].OutputTime = OutputTime;
                    slotNGInfo[MovNGSlotIndex].TimeSpan = TimeSpan;
                    slotNGInfo[MovNGSlotIndex].ScanTime = ScanTime;
                    slotNGInfo[MovNGSlotIndex].WorkTime = WorkTime;
                    slotNGInfo[MovNGSlotIndex].Result = ResultStr;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("更新穴位状态异常：" + ex.Message);
                }


            }
            if (info.Name.Contains("更新NG盘穴位SN2"))
            {
                try
                {
                    string SN = OpDlg.GetInstance().m_var.GetCValue("吸头2SNTemp");
                    slotNGInfo[MovNGSlotIndex].sn = SN;

                    string RFOKFlag = "";
                    string CarrySn = "";
                    string HSG = "";
                    string InputTime = "";
                    string OutputTime = "";
                    string TimeSpan = "";
                    string ScanTime = "";
                    string WorkTime = "";
                    double ResultType = -1;
                    string ResultStr = "";
                    string mesResult = "";         //Mes上传结果
                    string bydResult = "";         //钛大比亚迪防呆结果
                    string BStationResult = ""; //点胶工站结果

                    m_structMgr.GetStructCval(2, 0, 4, ref RFOKFlag);
                    m_structMgr.GetStructCval(2, 0, 5, ref CarrySn);

                    double GetMaterialTime = OpDlg.GetInstance().m_var.GetDValue("取料次数");
                    double MESFlag = OpDlg.GetInstance().m_var.GetDValue("MES启用标记");
                    if (GetMaterialTime == 1)
                    {
                        m_structMgr.GetStructCval(2, 0, 6, ref HSG);
                        m_structMgr.GetStructCval(2, 0, 25, ref BStationResult);
                        m_structMgr.GetStructCval(2, 0, 17, ref mesResult);
                        m_structMgr.GetStructCval(2, 0, 21, ref bydResult);
                    }
                    else if (GetMaterialTime == 2)
                    {
                        m_structMgr.GetStructCval(2, 0, 8, ref HSG);
                        m_structMgr.GetStructCval(2, 0, 27, ref BStationResult);
                        m_structMgr.GetStructCval(2, 0, 19, ref mesResult);
                        m_structMgr.GetStructCval(2, 0, 23, ref bydResult);
                    }
                    m_structMgr.GetStructCval(2, 0, 10, ref InputTime);
                    m_structMgr.GetStructCval(2, 0, 11, ref OutputTime);
                    m_structMgr.GetStructCval(2, 0, 12, ref TimeSpan);
                    m_structMgr.GetStructCval(2, 0, 13, ref ScanTime);
                    m_structMgr.GetStructCval(2, 0, 14, ref WorkTime);
                    m_structMgr.GetStructDval(2, 0, 15, ref ResultType);

                    if (BStationResult.Contains("NG"))
                    {
                        ResultStr = BStationResult;
                    }
                    else
                    {
                        if (ResultType == 0)
                        {
                            if (SN != "NG")
                            {
                                if (MESFlag == 1 && mesResult != "OK")
                                {
                                    ResultStr = "MES异常抛NG";
                                }
                                else
                                {
                                    //if (mesh_lot == "NG")
                                    //{
                                    //    Result = "防尘网抛NG";
                                    //    _input += 1;
                                    //    _ngOut += 1;
                                    //}


                                    ResultStr = "OK";
                                }
                            }
                            else
                            {
                                ResultStr = "玻璃扫码异常抛NG";
                            }
                        }
                        else if (ResultType == 1)
                        {
                            ResultStr = "RFID读取失败抛NG";
                        }
                        else if (ResultType == 2)
                        {
                            ResultStr = "隧道炉时间异常抛NG";
                        }
                        else if (ResultType == 3)
                        {
                            ResultStr = "点胶工站写RFID异常抛NG";
                        }
                        else if (ResultType == 4)
                        {
                            ResultStr = "点胶-保压时间异常抛NG";
                        }
                    }

                    if (bydResult == "NG")
                    {
                        ResultStr = "钛大比亚迪防呆NG";
                    }
                    slotNGInfo[MovNGSlotIndex].RFOKFlag = RFOKFlag;
                    slotNGInfo[MovNGSlotIndex].CarrySn = CarrySn;
                    slotNGInfo[MovNGSlotIndex].HSG = HSG;
                    slotNGInfo[MovNGSlotIndex].InputTime = InputTime;
                    slotNGInfo[MovNGSlotIndex].OutputTime = OutputTime;
                    slotNGInfo[MovNGSlotIndex].TimeSpan = TimeSpan;
                    slotNGInfo[MovNGSlotIndex].ScanTime = ScanTime;
                    slotNGInfo[MovNGSlotIndex].WorkTime = WorkTime;
                    slotNGInfo[MovNGSlotIndex].Result = ResultStr;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("更新穴位状态异常：" + ex.Message);
                }

            }

            if (info.Name == "数据结构到对象吸嘴1")
            {
                Products.Instance.SetProductInfo(0);
                return 0;
            }
            if (info.Name == "数据结构到对象吸嘴2")
            {
                Products.Instance.SetProductInfo(1);
                return 0;
            }
            if (info.Name == "生成产品数据")
            {
                Products.Instance.RecordingProductsData();
                return 0;
            }
            if (info.Name == "记录界面产出数据")
            {
                Products.Instance.RecordUphToUI();
                return 0;
            }
            if (info.Name == "清除收料数据缓存")
            {
                Products.Instance.ProductInfosClear();
                return 0;
            }
            if (info.Name == "更新界面产量数据")
            {
                //RefleshUI();
                SendDatas.Invoke("UpdateCount", 0);
                return 0;
            }
            if (info.Name == "初始化CT计数")
            {
                startNumber = 0;
                endNumber = 0;
                return 0;
            }
            if (info.Name == "生成出烤炉时间")
            {
                string dt = DateTime.Now.ToString("yyyyMMddHHmmss");
                OpDlg.GetInstance().m_var.SetCValue("出烤炉时间", dt);

                return 0;
            }
            if (info.Name == "LF信息查询上传")
            {
                double MesFlag = OpDlg.GetInstance().m_var.GetDValue("MES启用标记");
                double OvenTimeFlag = OpDlg.GetInstance().m_var.GetDValue("保压时间管控启用");
                double ScanFlag = OpDlg.GetInstance().m_var.GetDValue("跳过扫码");
                OpDlg.GetInstance().m_var.SetCValue("MES1穴结果", "OK");
                OpDlg.GetInstance().m_var.SetCValue("MES2穴结果", "OK");
                OpDlg.GetInstance().m_var.SetCValue("MES3穴结果", "OK");
                OpDlg.GetInstance().m_var.SetCValue("MES4穴结果", "OK");
                if (MesFlag == 0)
                {
                    GlobalEvent.ShowMsg("MES未启用！", OpDlg.Level.Normal);
                    return 0;
                }

                if (ScanFlag == 1)
                {
                    GlobalEvent.ShowMsg("跳过扫码，MES取消上传！", OpDlg.Level.Normal);
                    return 0;
                }
                GlobalEvent.ShowMsg($"=========MES流程启动=========", OpDlg.Level.Normal);
                string LastStationRFID = OpDlg.GetInstance().m_var.GetCValue("上站RFID写入动作标记");


                OpDlg.GetInstance().m_var.SetDValue("MES上传完成标记", 0);

                for (int i = 0; i < 4; i++)
                {
                    while (true)
                    {
                        ProcStatus procStatus = new ProcStatus();
                        Unit.GetInstance().PGetProcStatus(5, ref procStatus);
                        if (procStatus.status == ActionStatus.IDE || procStatus.status == ActionStatus.STOP)
                        {
                            return 0;
                        }
                        double ScanPoint = OpDlg.GetInstance().m_var.GetDValue("扫码检查点");
                        if (i <= ScanPoint)
                        {
                            break;
                        }
                        Thread.Sleep(100);
                    }

                    OpDlg.GetInstance().m_var.SetCValue($"MES{i + 1}穴结果", "");
                    string Result = OpDlg.GetInstance().m_var.GetCValue($"穴位{i + 1}-结果");
                    //if (Result == "NG")
                    //{
                    //    continue;
                    //}
                    if (Result == "NA")
                    {
                        OpDlg.GetInstance().m_var.SetCValue($"MES{i + 1}穴结果", "NA");
                        continue;
                    }
                    //  OpDlg.GetInstance().m_var.SetCValue($"查询防尘网结果{i + 1}", "OK");
                    //查询
                    string strHsgSN = OpDlg.GetInstance().m_var.GetCValue(68 + i);
                    string strWDSN = OpDlg.GetInstance().m_var.GetCValue(28 + i);

                    if (strHsgSN != "" && strHsgSN != "NA" && strHsgSN != "NG")
                    {
                        if (strWDSN != "" && strWDSN != "NA" && strWDSN != "NG" && strWDSN != "NULL")
                        {
                            OpDlg.GetInstance().m_var.SetCValue($"MES{i + 1}穴结果", "NG");
                            //   string strJson = "{\"cBarCode\": \"FP2041300PBP2M66629\"}"; 
                            string strJson = "{\"cBarCode\":\"" + strHsgSN + "\"}";
                            WriteLog("查询" + strJson + "\r\n");
                            string strRecive = "";
                            for (int k = 0; k < 3; k++)
                            {
                                strRecive = MES_Fun.Instance.API_MES_Action("查询", strJson);
                                WriteLog("查询结果" + strRecive + "\r\n");
                                if (strRecive.Contains("发生一个"))
                                {
                                    GlobalEvent.ShowMsg($"{i + 1}号穴物料查询失败！发起重复查询", OpDlg.Level.Normal);
                                    Thread.Sleep(300);
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            bc_window_assy_mesinfo obj = SerializeToVariable(strRecive);
                            try
                            {
                                if (obj != null)
                                {
                                    if (obj.data != null && obj.data.insight != null && obj.data.insight.uut_attributes != null)
                                    {
                                        //     OpDlg.GetInstance().m_var.SetCValue("MES查询结果", "OK");
                                        //     GlobalEvent.ShowMsg($"{i + 1}号穴物料查询成功！", OpDlg.Level.Normal);

                                        //if (LastStationRFID != "5")
                                        //{
                                        //if (obj.data.insight.results != null)
                                        //{
                                        //    if (obj.data.insight.results.Length == 0)
                                        //    {
                                        //        continue;
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    continue;
                                        //}

                                        //}
                                        //  obj = CoreWork.Instance.SetLimitParam(obj);
                                    }
                                    else
                                    {
                                        OpDlg.GetInstance().m_var.SetCValue("MES查询结果", "NG");
                                        GlobalEvent.ShowMsg($"{i + 1}号穴物料查询失败:{strJson}", OpDlg.Level.Normal);
                                        continue;
                                    }
                                }
                                else
                                {
                                    OpDlg.GetInstance().m_var.SetCValue("MES查询结果", "NG");
                                    GlobalEvent.ShowMsg($"{i + 1}号穴物料查询失败:{strJson}", OpDlg.Level.Normal);
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                OpDlg.GetInstance().m_var.SetCValue("MES查询结果", "NG");
                                GlobalEvent.ShowMsg($"{i + 1}号穴物料查询失败:{strJson}", OpDlg.Level.Normal);
                                continue;
                            }
                            //更新数据
                            //obj.data.insight.test_station_attributes.line_id = VarInteface.GetInstance().GetCValue("线体ID");
                            //obj.data.insight.test_station_attributes.software_name = VarInteface.GetInstance().GetCValue("设备软件名");
                            //obj.data.insight.test_station_attributes.software_version = VarInteface.GetInstance().GetCValue("设备软件版本");
                            //obj.data.insight.test_station_attributes.station_id = VarInteface.GetInstance().GetCValue("工站ID");

                            DateTime timeTemp = DateTime.Now;
                            bool isRFflag = false;
                            try
                            {
                                timeTemp = DateTime.ParseExact(obj.data.insight.uut_attributes.bc_window_assy_start_time, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            }
                            catch (Exception ex)
                            {
                                timeTemp = DateTime.Now;
                            }
                            try
                            {
                                obj.data.insight.uut_attributes.bc_window_sn = strWDSN;
                                string timeInput = OpDlg.GetInstance().m_var.GetCValue("进烤炉时间");
                                string timeOutput = OpDlg.GetInstance().m_var.GetCValue("出烤炉时间");
                                string TimeSpan = OpDlg.GetInstance().m_var.GetCValue("烤炉用时");
                                double TimeCount = OpDlg.GetInstance().m_var.GetDValue("烤炉时长设定");
                                double TimeCountUpLimit = OpDlg.GetInstance().m_var.GetDValue("烤炉时长上限设定");
                                DateTime dtInput;
                                DateTime dtOutput;
                                bool TimeScrap = false;
                                try
                                {
                                    dtInput = DateTime.ParseExact(timeInput, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                                    dtOutput = DateTime.ParseExact(timeOutput, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                                }
                                catch (Exception ex)
                                {
                                    dtInput = timeTemp;
                                    dtOutput = timeTemp;
                                    TimeScrap = true;
                                }
                                if (obj.data.insight.results != null)
                                {
                                    if (obj.data.insight.results.Length != 0)
                                    {
                                        bool isExistFail = false;
                                        //==========================================================//
                                        for (int K = 0; K < obj.data.insight.results.Length; K++)
                                        {
                                            if (obj.data.insight.results[K].result == "fail")
                                            {
                                                isExistFail = true;
                                                break;
                                            }

                                        }
                                        if (isExistFail == false)
                                        {
                                            MESSys.Result resultItem = new MESSys.Result();
                                            resultItem.units = "m";
                                            resultItem.value = TimeSpan;
                                            resultItem.test = "oven_time";
                                            resultItem.sub_test = "oven_time";

                                            double DTimeSpan = 0;
                                            if (TimeScrap == true)
                                            {
                                                TimeSpan = "-1";
                                            }
                                            try
                                            {
                                                DTimeSpan = double.Parse(TimeSpan);
                                            }
                                            catch (Exception)
                                            {

                                            }
                                            if (DTimeSpan >= TimeCount && DTimeSpan <= TimeCountUpLimit)
                                            {
                                                resultItem.result = "pass";
                                            }
                                            else
                                            {
                                                resultItem.result = "fail";
                                            }
                                            resultItem.lower_limit = TimeCount.ToString();
                                            resultItem.upper_limit = TimeCountUpLimit.ToString();


                                            MESSys.Result resultItem2 = new MESSys.Result();
                                            resultItem2.units = "-1";
                                            resultItem2.test = "RFID_BC_WD_ASSY";
                                            resultItem2.sub_test = "RFID_BC_WD_ASSY";
                                            resultItem2.upper_limit = "-1";
                                            resultItem2.lower_limit = "-1";
                                            if (LastStationRFID != "5" && TimeScrap != true)
                                            {
                                                if (obj.data.insight.results != null)
                                                {
                                                    if (obj.data.insight.results.Length < 3)
                                                    {
                                                        resultItem2.value = "fail";
                                                        resultItem2.result = "fail";
                                                    }
                                                    else
                                                    {
                                                        resultItem2.value = "pass";
                                                        resultItem2.result = "pass";
                                                        isRFflag = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                resultItem2.value = "pass";
                                                resultItem2.result = "pass";
                                            }

                                            try
                                            {
                                                int index = obj.data.insight.results.Length;
                                                Result[] results = new Result[index + 2];
                                                obj.data.insight.results.CopyTo(results, 0);
                                                results[index] = resultItem;
                                                results[index + 1] = resultItem2;
                                                obj.data.insight.results = results;
                                            }
                                            catch (Exception ex)
                                            {
                                                if (obj.data.insight.results == null)
                                                {
                                                    Result[] results = new Result[2];
                                                    results[0] = resultItem;
                                                    results[1] = resultItem2;
                                                    obj.data.insight.results = results;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MESSys.Result resultItem = new MESSys.Result();
                                        resultItem.units = "NA";
                                        resultItem.value = "0.000";
                                        resultItem.test = "Communication_Overtime";
                                        resultItem.sub_test = "Communication_Overtime";
                                        resultItem.lower_limit = "-1";
                                        resultItem.upper_limit = "-1";
                                        resultItem.result = "fail";

                                        MESSys.Result resultItem2 = new MESSys.Result();
                                        resultItem.units = "NA";
                                        resultItem.value = "0.000";
                                        resultItem.test = "Communication_Overtime";
                                        resultItem.sub_test = "Communication_Mes_Overtime";
                                        resultItem.lower_limit = "-1";
                                        resultItem.upper_limit = "-1";
                                        resultItem.result = "fail";

                                        Result[] results = new Result[2];
                                        results[0] = resultItem;
                                        results[1] = resultItem2;
                                        obj.data.insight.results = results;
                                    }
                                }
                                else
                                {
                                    MESSys.Result resultItem = new MESSys.Result();
                                    resultItem.units = "NA";
                                    resultItem.value = "0.000";
                                    resultItem.test = "Communication_Overtime";
                                    resultItem.sub_test = "Communication_Overtime";
                                    resultItem.lower_limit = "-1";
                                    resultItem.upper_limit = "-1";
                                    resultItem.result = "fail";

                                    MESSys.Result resultItem2 = new MESSys.Result();
                                    resultItem.units = "NA";
                                    resultItem.value = "0.000";
                                    resultItem.test = "Communication_Overtime";
                                    resultItem.sub_test = "Communication_Mes_Overtime";
                                    resultItem.lower_limit = "-1";
                                    resultItem.upper_limit = "-1";
                                    resultItem.result = "fail";

                                    Result[] results = new Result[2];
                                    results[0] = resultItem;
                                    results[1] = resultItem2;
                                    obj.data.insight.results = results;
                                }

                                OpDlg.GetInstance().m_var.SetCValue("点胶完成时间", timeTemp.ToString("yyyyMMddHHmmss"));
                                if (OvenTimeFlag == 1)
                                {
                                    double NGFlag = OpDlg.GetInstance().m_var.GetDValue("抛NG标记");
                                    OpDlg.GetInstance().m_var.SetCValue("点胶完成时间", "");
                                    string OvenTime = OpDlg.GetInstance().m_var.GetCValue("压盖板时间");
                                    double OvenTimeSet1 = OpDlg.GetInstance().m_var.GetDValue("点胶保压时间设定");
                                    double OvenTimeSet2 = OpDlg.GetInstance().m_var.GetDValue("保压烤炉时间设定");

                                    if (NGFlag != 0 && NGFlag != 4 && NGFlag != 3)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            DateTime dtOvenTime = DateTime.ParseExact(OvenTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                                            DateTime uutTimeTemp;//点胶完成时间
                                            if (!String.IsNullOrEmpty(obj.data.insight.uut_attributes.bc_window_assy_start_time))
                                            {
                                                uutTimeTemp = DateTime.ParseExact(obj.data.insight.uut_attributes.bc_window_assy_start_time, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);

                                                if (NGFlag != 4)
                                                    OpDlg.GetInstance().m_var.SetCValue("点胶完成时间", uutTimeTemp.ToString("yyyyMMddHHmmss"));

                                                TimeSpan ts = dtInput - dtOvenTime;
                                                obj.data.insight.uut_attributes.oven_assy_time = dtOvenTime.ToString("yyyy-MM-dd HH:mm:ss");
                                                obj.data.insight.uut_attributes.oven_start_time = dtInput.ToString("yyyy-MM-dd HH:mm:ss");
                                                obj.data.insight.uut_attributes.oven_stop_time = dtOutput.ToString("yyyy-MM-dd HH:mm:ss");
                                                if (ts.TotalSeconds > OvenTimeSet1)
                                                {
                                                    //
                                                    GlobalEvent.ShowMsg($"{i + 1}号穴物料点胶-保压时间异常", OpDlg.Level.Normal);
                                                    OpDlg.GetInstance().m_var.SetDValue("抛NG标记", 4);
                                                    continue;
                                                }
                                                else if (ts.TotalSeconds > OvenTimeSet2)
                                                {
                                                    //
                                                    GlobalEvent.ShowMsg($"{i + 1}号穴物料保压-进烤炉时间异常", OpDlg.Level.Normal);
                                                    OpDlg.GetInstance().m_var.SetDValue("抛NG标记", 4);
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                //OpDlg.GetInstance().m_var.SetCValue("点胶完成时间","");
                                                //OpDlg.GetInstance().m_var.SetDValue("抛NG标记", 4);
                                            }


                                        }
                                        catch (Exception ex)
                                        {
                                            OpDlg.GetInstance().m_var.SetDValue("抛NG标记", 4);
                                        }
                                    }
                                }
                                if (obj.data.insight.uut_attributes.oven_assy_time == "")
                                    obj.data.insight.uut_attributes.oven_assy_time = dtInput.ToString("yyyy-MM-dd HH:mm:ss");
                                if (obj.data.insight.uut_attributes.oven_start_time == "")
                                    obj.data.insight.uut_attributes.oven_start_time = dtInput.ToString("yyyy-MM-dd HH:mm:ss");
                                if (obj.data.insight.uut_attributes.oven_stop_time == "")
                                    obj.data.insight.uut_attributes.oven_stop_time = dtInput.ToString("yyyy-MM-dd HH:mm:ss");
                                obj.data.insight.test_attributes.uut_stop = dtOutput.ToString("yyyy-MM-dd HH:mm:ss");



                                double MESAutoCreate = OpDlg.GetInstance().m_var.GetDValue("MES自动填充");
                                if (MESAutoCreate == 1)
                                {
                                    if (obj.data.insight.uut_attributes.fixture_id == "")
                                        obj.data.insight.uut_attributes.fixture_id = "ABC0001";
                                    if (obj.data.insight.uut_attributes.prima_expiry_date == "")
                                        obj.data.insight.uut_attributes.prima_expiry_date = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.prima_start_time == "")
                                        obj.data.insight.uut_attributes.prima_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.prima_stop_time == "")
                                        obj.data.insight.uut_attributes.prima_stop_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.standing_start_time == "")
                                        obj.data.insight.uut_attributes.standing_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.standing_stop_time == "")
                                        obj.data.insight.uut_attributes.standing_stop_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.bc_window_assy_start_time == "")
                                        obj.data.insight.uut_attributes.bc_window_assy_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.plasma_start_time == "")
                                        obj.data.insight.uut_attributes.plasma_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.plasma_stop_time == "")
                                        obj.data.insight.uut_attributes.plasma_stop_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.prima_lot == "")
                                        obj.data.insight.uut_attributes.prima_lot = "ABC0001";
                                    if (obj.data.insight.uut_attributes.prima_vendor == "")
                                        obj.data.insight.uut_attributes.prima_vendor = "EW";

                                    //try
                                    //{
                                    //    DateTime dt = DateTime.ParseExact(obj.data.insight.test_attributes.uut_start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                                    //    obj.data.insight.test_attributes.uut_start = dt.ToString("yyyy-MM-dd HH:mm:ss");
                                    //}
                                    //catch (Exception ex)
                                    //{

                                    //}

                                }
                                string BYDResult = OpDlg.GetInstance().m_var.GetCValue($"钛大壳防呆结果{i + 1}");
                                if (Result.Contains("NG"))
                                {
                                    obj.data.insight.test_attributes.test_result = "scrap";
                                    //   GlobalEvent.ShowMsg($"{i + 1}号穴物料判定NG", OpDlg.Level.Normal);
                                }
                                else if (BYDResult == "NG")
                                {
                                    obj.data.insight.test_attributes.test_result = "scrap";
                                    continue;
                                }
                                else
                                {
                                    double ResultFlag = OpDlg.GetInstance().m_var.GetDValue("抛NG标记");
                                    if (ResultFlag == 0)
                                    {
                                        obj.data.insight.test_attributes.test_result = "pass";
                                        //  GlobalEvent.ShowMsg($"{i + 1}号穴物料判定OK", OpDlg.Level.Normal);

                                    }
                                    else if (ResultFlag == 3)
                                    {
                                        if (isRFflag)
                                            obj.data.insight.test_attributes.test_result = "pass";
                                        else
                                            obj.data.insight.test_attributes.test_result = "scrap";
                                    }
                                    else
                                    {
                                        obj.data.insight.test_attributes.test_result = "scrap";
                                        //    GlobalEvent.ShowMsg($"{i + 1}号穴物料判定NG", OpDlg.Level.Normal);
                                    }
                                }

                                if (obj.data.insight.results != null)
                                {
                                    if (obj.data.insight.results.Length < 3)
                                    {
                                        obj.data.insight.test_attributes.test_result = "scrap";
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                obj.data.insight.test_attributes.test_result = "scrap";
                                //     GlobalEvent.ShowMsg($"{i + 1}号穴物料判定NG", OpDlg.Level.Normal);
                            }

                            //查询物料材质并生成url
                            string procuctCode = CheckProductCode(strHsgSN);
                            // string procuctCode = "N217-TI";
                            WriteLog("查询材质结果" + procuctCode + "\r\n");
                            if (procuctCode == "NG")
                            {
                                OpDlg.GetInstance().m_var.SetCValue($"MES{i + 1}穴结果", "NG");
                                GlobalEvent.ShowMsg($"{i + 1}号穴物料材质查询失败！", OpDlg.Level.Normal);
                                continue;
                            }
                            //string str = "{\"data\":{\"insight\":{\"results\":[{\"result\":\"pass\",\"test\":\"glue_open_time\",\"units\":\"s\",\"value\":\"40\"}],\"test_attributes\":{\"test_result\":\"pass\",\"unit_serial_number\":\"FP2041300PBP2M66629\",\"uut_start\":\"2018 - 09 - 18 20:41:33\",\"uut_stop\":\"2018 - 09 - 18 20:41:33\"},\"test_station_attributes\":{\"line_id\":\"IPGL_C09 - 3FA\",\"software_name\":\"DEVELOPMENT1\",\"software_version\":\"V1.111\",\"station_id\":\"Site_LineID_MachineID_StationName\"},\"uut_attributes\":{\"bc_window_assy_start_time\":\"yyyy - mm - dd hh: mm: ss\",\"bc_window_sn\":\"\",\"cavity_id\":\"TBD\",\"fixture_id\":\"ABC0001\",\"glue_dispense_start_time\":\"yyyy - mm - dd hh: mm: ss\",\"oven_start_time\":\"\",\"oven_stop_time\":\"\",\"plasma_start_time\":\"yyyy - mm - dd hh: mm: ss\",\"plasma_stop_time\":\"yyyy - mm - dd hh: mm: ss\",\"prima_expiry_date\":\"yyyy - mm - dd hh: mm: ss\",\"prima_lot\":\"ABC0001\",\"prima_open_time\":\"yyyy - mm - dd hh: mm: ss\",\"prima_vendor\":\"JQS\",\"standing_start_time\":\"yyyy - mm - dd hh: mm: ss\",\"standing_stop_time\":\"yyyy - mm - dd hh: mm: ss\",\"station_vendor\":\"JQS\"}}},\"serials\":{\"fg\":\"FP2041300PBP2M66629\"}}";

                            //上传
                            string strUpLoadRecive = "";
                            try
                            {
                                string objJson = obj.Serialize2Json();
                                WriteLog("上传" + objJson + "\r\n");
                                // string strUpLoadRecive = MES_Fun.Instance.API_MES_Action("过站", objJson);
                                string PDCAurl = OpDlg.GetInstance().m_var.GetCValue("PDCAurl");
                                PDCAurl = PDCAurl + procuctCode;
                                strUpLoadRecive = SendToPDCA(PDCAurl, objJson);
                                WriteLog("上传结果" + strUpLoadRecive + "\r\n");
                            }
                            catch (Exception ex)
                            {
                                WriteLog("上传结果" + strUpLoadRecive + "\r\n");
                                OpDlg.GetInstance().m_var.SetCValue("MES上传结果", "NG");
                                OpDlg.GetInstance().m_var.SetCValue($"MES{i + 1}穴结果", "NG");
                                GlobalEvent.ShowMsg($"{i + 1}号穴物料上传失败:{strUpLoadRecive}", OpDlg.Level.Normal);
                                continue;
                            }

                            try
                            {
                                //MesResultJson mesJson = new MesResultJson();
                                //mesJson = mesJson.SerializeToVariable(strUpLoadRecive);
                                bool isSucceed = CheckIsUploaded(strUpLoadRecive);
                                if (isSucceed)
                                {
                                    OpDlg.GetInstance().m_var.SetCValue("MES上传结果", "OK");
                                    OpDlg.GetInstance().m_var.SetCValue($"MES{i + 1}穴结果", "OK");
                                    GlobalEvent.ShowMsg($"{i + 1}号穴物料上传成功！", OpDlg.Level.Normal);

                                    if (obj.data.insight.results != null)
                                    {
                                        if (obj.data.insight.results.Length < 3)
                                        {
                                            OpDlg.GetInstance().m_var.SetCValue("MES上传结果", "NG");
                                            OpDlg.GetInstance().m_var.SetCValue($"MES{i + 1}穴结果", "NG");
                                            GlobalEvent.ShowMsg($"{i + 1}号穴物料:点胶测试项不足", OpDlg.Level.Normal);
                                            WriteLog($"{i + 1}号穴物料:{strHsgSN}点胶测试项不足");
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    OpDlg.GetInstance().m_var.SetCValue("MES上传结果", "NG");
                                    OpDlg.GetInstance().m_var.SetCValue($"MES{i + 1}穴结果", "NG");
                                    GlobalEvent.ShowMsg($"{i + 1}号穴物料上传失败:{strUpLoadRecive}", OpDlg.Level.Normal);
                                    //    WriteCsv4Mes(strUpLoadRecive, "NG");
                                    continue;
                                }
                            }
                            catch
                            {
                                OpDlg.GetInstance().m_var.SetCValue("MES上传结果", "NG");
                                OpDlg.GetInstance().m_var.SetCValue($"MES{i + 1}穴结果", "NG");
                                GlobalEvent.ShowMsg($"{i + 1}号穴物料上传失败:{strUpLoadRecive}", OpDlg.Level.Normal);
                                //   WriteCsv4Mes(strUpLoadRecive, "NG");
                                continue;
                            }

                        }
                        else
                        {
                            GlobalEvent.ShowMsg("SN码异常，取消当前料MES上传！", OpDlg.Level.Normal);
                        }
                    }
                    else
                    {
                        GlobalEvent.ShowMsg("HSG码异常，取消当前料MES上传！", OpDlg.Level.Normal);
                    }
                }

                OpDlg.GetInstance().m_var.SetDValue("MES上传完成标记", 1);
                return 0;
            }
            if (info.Name.Contains("钛大比亚迪防呆"))
            {
                double WarnFlag = OpDlg.GetInstance().m_var.GetDValue("钛大壳防呆启用标记");
                if (WarnFlag == 0)
                    return 0;
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        string strHsgSN = OpDlg.GetInstance().m_var.GetCValue(68 + i);
                        string strWDSN = OpDlg.GetInstance().m_var.GetCValue(28 + i);

                        string subHsg = strHsgSN.Substring(strHsgSN.Length - 7, 7);
                        string subWd = strWDSN.Substring(0, 2);
                        if (subHsg == "0000JXS" || subHsg == "0000JXU" || subHsg == "0000JXQ" || subHsg == "0000JXR")
                        {
                            if (subWd == "DW")
                            {
                                //MessageBox.Show($"穴位{i + 1}钛大壳组装比亚迪玻璃报警，请通知上游更换玻璃：HSG:{strHsgSN} 玻璃：{strWDSN}");
                                OpDlg.GetInstance().m_var.SetCValue($"钛大壳防呆结果{i + 1}", "NG");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }

            }
            if (info.Name == "LF信息上传重投")
            {
                GlobalEvent.ShowMsg($"=========MES重投流程启动=========", OpDlg.Level.Normal);
                OpDlg.GetInstance().m_var.SetCValue("RF读取成功标志", "OK");
                OpDlg.GetInstance().m_var.SetDValue("抛NG标记", 0);
                OpDlg.GetInstance().m_var.SetCValue("上站RFID写入动作标记", "5");
                for (int i = 0; i < 4; i++)
                {
                    OpDlg.GetInstance().m_var.SetCValue($"MES{i + 1}穴结果", "");
                    string strHsgSN = OpDlg.GetInstance().m_var.GetCValue($"穴位{i + 1}-BC码");  //HSG
                    string strWDSN = OpDlg.GetInstance().m_var.GetCValue($"扫码SN{i + 1}");  //WD

                    if (strHsgSN != "" && strHsgSN != "NA" && strHsgSN != "NG")
                    {
                        if (strWDSN != "" && strWDSN != "NA" && strWDSN != "NG" && strWDSN != "NULL")
                        {
                            string strJson = "{\"cBarCode\":\"" + strHsgSN + "\"}";
                            WriteLog("查询" + strJson + "\r\n");
                            string strRecive = MES_Fun.Instance.API_MES_Action("查询", strJson);
                            WriteLog("查询结果" + strRecive + "\r\n");
                            bc_window_assy_mesinfo obj = SerializeToVariable(strRecive);
                            try
                            {
                                if (obj != null)
                                {
                                    if (obj.data != null && obj.data.insight != null && obj.data.insight.uut_attributes != null)
                                    {

                                    }
                                    else
                                    {
                                        GlobalEvent.ShowMsg($"数据查询失败-{i + 1}号穴", OpDlg.Level.Normal);
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {

                                continue;
                            }
                            try
                            {
                                DateTime timeTemp = DateTime.Now;

                                obj.data.insight.uut_attributes.bc_window_sn = strWDSN;
                                DateTime dtInput;
                                DateTime dtOutput;

                                dtInput = timeTemp;
                                dtOutput = timeTemp;



                                obj.data.insight.uut_attributes.oven_assy_time = dtInput.ToString("yyyy-MM-dd HH:mm:ss");
                                obj.data.insight.uut_attributes.oven_start_time = dtInput.ToString("yyyy-MM-dd HH:mm:ss");
                                obj.data.insight.uut_attributes.oven_stop_time = dtInput.ToString("yyyy-MM-dd HH:mm:ss");
                                obj.data.insight.test_attributes.uut_stop = dtOutput.ToString("yyyy-MM-dd HH:mm:ss");
                                obj.data.insight.test_station_attributes.station_id = VarInteface.GetInstance().GetCValue("工站ID");

                                double MESAutoCreate = OpDlg.GetInstance().m_var.GetDValue("MES自动填充");
                                if (MESAutoCreate == 1)
                                {

                                    if (obj.data.insight.uut_attributes.fixture_id == "")
                                        obj.data.insight.uut_attributes.fixture_id = "ABC0001";
                                    if (obj.data.insight.uut_attributes.prima_expiry_date == "")
                                        obj.data.insight.uut_attributes.prima_expiry_date = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.prima_start_time == "")
                                        obj.data.insight.uut_attributes.prima_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.prima_stop_time == "")
                                        obj.data.insight.uut_attributes.prima_stop_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.standing_start_time == "")
                                        obj.data.insight.uut_attributes.standing_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.standing_stop_time == "")
                                        obj.data.insight.uut_attributes.standing_stop_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.bc_window_assy_start_time == "")
                                        obj.data.insight.uut_attributes.bc_window_assy_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.plasma_start_time == "")
                                        obj.data.insight.uut_attributes.plasma_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.plasma_stop_time == "")
                                        obj.data.insight.uut_attributes.plasma_stop_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (obj.data.insight.uut_attributes.prima_lot == "")
                                        obj.data.insight.uut_attributes.prima_lot = "ABC0001";
                                    if (obj.data.insight.uut_attributes.prima_vendor == "")
                                        obj.data.insight.uut_attributes.prima_vendor = "EW";
                                    //try
                                    //{
                                    //    DateTime dt = DateTime.ParseExact(obj.data.insight.test_attributes.uut_start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                                    //    obj.data.insight.test_attributes.uut_start = dt.ToString("yyyy-MM-dd HH:mm:ss");
                                    //}
                                    //catch (Exception ex)
                                    //{

                                    //}

                                }
                                string strHsgResult = OpDlg.GetInstance().m_var.GetCValue($"穴位{i + 1}-结果");
                                if (strHsgResult.Contains("NG"))
                                {
                                    obj.data.insight.test_attributes.test_result = "scrap";
                                    GlobalEvent.ShowMsg($"{i + 1}号穴物料判定NG", OpDlg.Level.Normal);
                                }
                                else
                                {
                                    obj.data.insight.test_attributes.test_result = "pass";
                                    GlobalEvent.ShowMsg($"{i + 1}号穴物料判定OK", OpDlg.Level.Normal);
                                }

                            }
                            catch (Exception ex)
                            {
                                GlobalEvent.ShowMsg($"数据设置失败-{i + 1}号穴", OpDlg.Level.Normal);
                            }

                            //查询物料材质并生成url
                            string procuctCode = CheckProductCode(strHsgSN);
                            // string procuctCode = "N217-TI";
                            WriteLog("查询材质结果" + procuctCode + "\r\n");
                            if (procuctCode == "NG")
                            {
                                GlobalEvent.ShowMsg($"数据查询失败-{i + 1}号穴", OpDlg.Level.Normal);
                                continue;
                            }
                            //string str = "{\"data\":{\"insight\":{\"results\":[{\"result\":\"pass\",\"test\":\"glue_open_time\",\"units\":\"s\",\"value\":\"40\"}],\"test_attributes\":{\"test_result\":\"pass\",\"unit_serial_number\":\"FP2041300PBP2M66629\",\"uut_start\":\"2018 - 09 - 18 20:41:33\",\"uut_stop\":\"2018 - 09 - 18 20:41:33\"},\"test_station_attributes\":{\"line_id\":\"IPGL_C09 - 3FA\",\"software_name\":\"DEVELOPMENT1\",\"software_version\":\"V1.111\",\"station_id\":\"Site_LineID_MachineID_StationName\"},\"uut_attributes\":{\"bc_window_assy_start_time\":\"yyyy - mm - dd hh: mm: ss\",\"bc_window_sn\":\"\",\"cavity_id\":\"TBD\",\"fixture_id\":\"ABC0001\",\"glue_dispense_start_time\":\"yyyy - mm - dd hh: mm: ss\",\"oven_start_time\":\"\",\"oven_stop_time\":\"\",\"plasma_start_time\":\"yyyy - mm - dd hh: mm: ss\",\"plasma_stop_time\":\"yyyy - mm - dd hh: mm: ss\",\"prima_expiry_date\":\"yyyy - mm - dd hh: mm: ss\",\"prima_lot\":\"ABC0001\",\"prima_open_time\":\"yyyy - mm - dd hh: mm: ss\",\"prima_vendor\":\"JQS\",\"standing_start_time\":\"yyyy - mm - dd hh: mm: ss\",\"standing_stop_time\":\"yyyy - mm - dd hh: mm: ss\",\"station_vendor\":\"JQS\"}}},\"serials\":{\"fg\":\"FP2041300PBP2M66629\"}}";

                            //上传
                            string strUpLoadRecive = "";
                            try
                            {
                                string objJson = obj.Serialize2Json();
                                //              string MESRecive = MES_Fun.Instance.API_MES_Action("过站", strJson);
                                WriteLog("上传" + objJson + "\r\n");
                                // string strUpLoadRecive = MES_Fun.Instance.API_MES_Action("过站", objJson);
                                string PDCAurl = OpDlg.GetInstance().m_var.GetCValue("PDCAurl");
                                PDCAurl = PDCAurl + procuctCode;
                                strUpLoadRecive = SendToPDCA(PDCAurl, objJson);
                                WriteLog("上传结果" + strUpLoadRecive + "\r\n");
                            }
                            catch (Exception ex)
                            {
                                WriteLog("上传结果" + strUpLoadRecive + "\r\n");

                                continue;
                            }
                            try
                            {
                                //MesResultJson mesJson = new MesResultJson();
                                //mesJson = mesJson.SerializeToVariable(strUpLoadRecive);
                                bool isSucceed = CheckIsUploaded(strUpLoadRecive);
                                if (isSucceed)
                                {
                                    GlobalEvent.ShowMsg($"数据上传成功-{i + 1}号穴", OpDlg.Level.Normal);
                                    OpDlg.GetInstance().m_var.SetCValue($"MES{i + 1}穴结果", "OK");
                                }
                                else
                                {
                                    GlobalEvent.ShowMsg($"数据上传失败-{i + 1}号穴", OpDlg.Level.Normal);
                                    continue;
                                }
                            }
                            catch
                            {
                                GlobalEvent.ShowMsg($"数据上传失败-{i + 1}号穴", OpDlg.Level.Normal);
                                continue;
                            }
                        }
                        else
                        {
                            GlobalEvent.ShowMsg($"玻璃码异常-{i + 1}号穴", OpDlg.Level.Normal);
                        }
                    }
                    else
                    {
                        GlobalEvent.ShowMsg($"HSG码异常-{i + 1}号穴", OpDlg.Level.Normal);
                    }
                }
                GlobalEvent.ShowMsg($"=========MES重投流程结束=========", OpDlg.Level.Normal);
                return 0;
            }
            if (info.Name == "查询防尘网")
            {
                CheckProductCode("F9WH690008G0000JXS");



                return 0;
            }
            if (info.Name.Contains("设置输出信号"))
            {
                return SetIO(info.Name);
            }
            else if (info.Name.Contains("读取输入信号"))
            {
                return GetIO(info.Name);
            }
            if (info.Name.Contains("转换点胶NG信息"))
            {
                for (int i = 0; i < 4; i++)
                {
                    string Result = OpDlg.GetInstance().m_var.GetCValue($"穴位{i + 1}-结果");
                    if (Result == "11")
                    {
                        OpDlg.GetInstance().m_var.SetCValue($"穴位{i + 1}-结果", "GapNG");
                    }
                    else if (Result == "12")
                    {
                        OpDlg.GetInstance().m_var.SetCValue($"穴位{i + 1}-结果", "测高NG");
                    }
                    else if (Result == "13")
                    {
                        OpDlg.GetInstance().m_var.SetCValue($"穴位{i + 1}-结果", "Gap-测高NG");
                    }
                    else if (Result == "NG")
                    {
                        OpDlg.GetInstance().m_var.SetCValue($"穴位{i + 1}-结果", "Gap-测高NG");
                    }
                }
            }
            Log.WriteLine("消息:{0} 触发", Log.Custom, info.Name);
            return 0;
        }
        public void WriteLog4VIO(string str)
        {
            //查询是否有文件存在，如果不存在则创建
            string path = "D:\\TestLog\\VirtualIOLog\\" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                //判断文件是否存在，没有则创建。
                if (!System.IO.File.Exists(path))
                {
                    FileStream stream = System.IO.File.Create(path);
                    stream.Close();
                    stream.Dispose();
                }

                //在文件中写入
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]     {str}");
                }
            }
            catch
            {

            }


        }
        private int SetIO(string strinfo)
        {
            try
            {
                string[] strParam = strinfo.Split(new[] { "(", ")", "," }, StringSplitOptions.RemoveEmptyEntries);
                if (strParam.Length == 3)
                {
                    int Index = int.Parse(strParam[1]);
                    int Status = int.Parse(strParam[2]);
                    if (Virtual_IO_TCP.Instance.SetOutput(Index, Status))
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }

                }
                return -2;

            }
            catch
            {
                return -1;
            }


        }

        private int GetIO(string strinfo)
        {
            try
            {
                string[] strParam = strinfo.Split(new[] { "(", ")", "," }, StringSplitOptions.RemoveEmptyEntries);
                if (strParam.Length == 4)
                {
                    DateTime StartTime = DateTime.Now;
                    int Index = int.Parse(strParam[1]);
                    int Status = int.Parse(strParam[2]);
                    int Timeout = int.Parse(strParam[3]);
                    int CurrentSytatus = Virtual_IO_TCP.Instance.GetInput(Index);
                    while (true)
                    {
                        TimeSpan Diff = DateTime.Now - StartTime;
                        if (Diff.TotalMilliseconds > Timeout)
                        {
                            return -3;
                        }
                        if (Status == CurrentSytatus)
                        {
                            string source = "";
                            if (Index == 8)
                            {
                                source = "BC点胶(镜像)";
                            }
                            else if (Index == 9)
                            {
                                source = "BC点胶(正向)";
                            }
                            else
                            {
                                source = Index.ToString();
                            }
                            WriteLog4VIO($"接收到信号：{source},{Status}");
                            return 0;
                        }
                    }

                }
                return -2;

            }
            catch
            {
                return -1;
            }


        }
        public bool CheckIsUploaded(string str)
        {
            //  string kkk = "{"id": "2b112ce5 - 9a48 - 11ee - 9b28 - 6cb3116e9b10"}";
            try
            {
                string result = str.Substring(0, 5);
                if (result == "{\"id\"")
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        public string SendToPDCA(string url, string strData)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var data = Encoding.UTF8.GetBytes(strData);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                request.KeepAlive = false;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }

            catch (Exception ex)
            {
                return "NG";
            }
        }
        public void CheckMeshLot(int index)
        {
            string sUrl, sTestSN, sMESname, spassword;
            sUrl = OpDlg.GetInstance().m_var.GetCValue("防尘网URL");
            sMESname = OpDlg.GetInstance().m_var.GetCValue("MES_username");
            spassword = OpDlg.GetInstance().m_var.GetCValue("MES_password");

            sTestSN = OpDlg.GetInstance().m_var.GetCValue(index + 68);
            OpDlg.GetInstance().m_var.SetCValue($"查询防尘网结果{index + 1}", "NG");

            sUrl = sUrl + $"?serial={sTestSN.Trim().ToUpper()}&serial_type=fg" + "&last_log=true";
            WriteLog("查询防尘网" + sUrl + "\r\n");
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(sUrl) as HttpWebRequest;
                request.Credentials = new NetworkCredential(sMESname, spassword);
                request.Method = "GET";
                request.Timeout = 1000;
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                WriteLog("查询防尘网结果" + responseString + "\r\n");
                mesh_lot mesJson = new mesh_lot();
                mesJson = SerializeToMesh_lot(responseString);
                if (mesJson != null)
                {

                    for (int i = 0; i < mesJson.history.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(mesJson.history[i].data.insight.uut_attributes.mesh_lot))
                        {
                            OpDlg.GetInstance().m_var.SetCValue($"查询防尘网结果{i + 1}", "OK");
                            GlobalEvent.ShowMsg($"防尘网查询成功:{i + 1}！", OpDlg.Level.Normal);
                            break;
                        }
                    }
                }
                else
                {
                    GlobalEvent.ShowMsg($"防尘网查询失败:{index + 1}！", OpDlg.Level.Normal);
                }

            }
            catch (Exception ex)
            {
                GlobalEvent.ShowMsg($"防尘网查询失败:{index + 1}！", OpDlg.Level.Normal);
                OpDlg.GetInstance().m_var.SetCValue($"查询防尘网结果{index + 1}", "NG");
                WriteLog("防尘网查询失败：" + ex.Message + "\r\n");
            }
        }
        public string CheckProductCode(string HSG)
        {
            string sUrl = "http://17.80.224.220/api/v2/parts?serial_type=fg&serial=";
            sUrl = sUrl + $"{HSG.Trim().ToUpper()}" + "&last_log=true";

            string sMESname = OpDlg.GetInstance().m_var.GetCValue("MES_username");
            string spassword = OpDlg.GetInstance().m_var.GetCValue("MES_password");
            string procuctCodestr = "";
            WriteLog("查询ProductCode" + sUrl + "\r\n");
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(sUrl) as HttpWebRequest;
                request.Credentials = new NetworkCredential(sMESname, spassword);
                request.Method = "GET";
                request.Timeout = 1000;
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                //解析出procuctCode
                procuctCode procuctC = new procuctCode();
                procuctC = SerializeToProcuctCode(responseString);
                if (procuctC != null && procuctC.properties != null)
                {
                    if (!String.IsNullOrEmpty(procuctC.properties.Project_Code))
                    {
                        //  GlobalEvent.ShowMsg($"物料材质查询成功:{procuctC.properties.Project_Code}", OpDlg.Level.Normal);
                        return procuctC.properties.Project_Code.ToLower();
                    }
                    else
                    {
                        GlobalEvent.ShowMsg($"物料材质查询失败", OpDlg.Level.Normal);
                        return "NG";
                    }
                }
                else
                {
                    GlobalEvent.ShowMsg($"物料材质查询失败", OpDlg.Level.Normal);
                    return "NG";
                }
                return procuctCodestr;
            }
            catch (Exception ex)
            {
                GlobalEvent.ShowMsg($"物料材质查询失败:{ex.Message}", OpDlg.Level.Normal);
                return "NG";
            }
        }
        public mesh_lot SerializeToMesh_lot(string json)
        {
            try
            {

                return JsonConvert.DeserializeObject<mesh_lot>(json);
            }
            catch (Exception e)
            {

                return null;
            }

        }
        public procuctCode SerializeToProcuctCode(string json)
        {
            try
            {

                return JsonConvert.DeserializeObject<procuctCode>(json);
            }
            catch (Exception e)
            {

                return null;
            }

        }
        public void WriteLog(string str)
        {
            // 设置特定文件夹的路径
            string folderPath = "D:\\TestLog\\Meslog\\";


            // 检查文件夹是否存在
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            //查询是否有文件存在，如果不存在则创建
            string path = "D:\\TestLog\\Meslog\\" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                //判断文件是否存在，没有则创建。
                if (!System.IO.File.Exists(path))
                {
                    FileStream stream = System.IO.File.Create(path);
                    stream.Close();
                    stream.Dispose();
                }

                //在文件中写入
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]     {str}");
                }
            }
            catch
            {

            }


        }
        public bc_window_assy_mesinfo SerializeToVariable(string json)
        {
            try
            {

                return JsonConvert.DeserializeObject<bc_window_assy_mesinfo>(json);
            }
            catch (Exception e)
            {

                return null;
            }

        }
        public class MesResultJson
        {
            public string Data { get; set; }
            public string Message { get; set; }
            public string result { get; set; }

            public MesResultJson SerializeToVariable(string json)
            {
                try
                {

                    return JsonConvert.DeserializeObject<MesResultJson>(json);
                }
                catch (Exception e)
                {

                    return new MesResultJson();
                }

            }
        }
        public void WriteCsv4Mes(string str, string result)
        {
            try
            {
                // 设置特定文件夹的路径
                string folderPath = "D:\\TestLog\\MesData\\";


                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

                // 检查文件夹是否存在
                if (!Directory.Exists(folderPath + currentDate))
                {
                    Directory.CreateDirectory(folderPath + currentDate);
                }
                // 构建文件路径，将文件名设置为当前时间
                string currentTime = DateTime.Now.ToString("HH");
                // DateTime startTime = DateTime.Now;
                string filePath = $"{folderPath + currentDate}\\{currentTime}.csv";

                // 创建一个StreamWriter以写入CSV文件
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length == 0)
                    {
                        // 写入CSV文件的标题行
                        writer.WriteLine("记录时间(s),上传数据,上传结果");

                    }
                    string TimeNow = DateTime.Now.ToLongTimeString();



                    writer.WriteLine($"{TimeNow},{str},{result}");
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void RefleshUI()
        {
            OpDlg.GetInstance().Invoke(new Action(() =>
            {
                double ALL = OpDlg.GetInstance().m_var.GetDValue("投入");
                double OK = OpDlg.GetInstance().m_var.GetDValue("产出");
                double NG = OpDlg.GetInstance().m_var.GetDValue("次品");


                OpDlg.GetInstance().labelTotal.Text = string.Format("投入总数： {0}", ALL);
                //获取产出总数
                OpDlg.GetInstance().labelOutput.Text = string.Format("产出总数： {0}", OK);
                //ng
                OpDlg.GetInstance().labelNg.Text = string.Format("次品总数： {0}", NG);
                //Yield
                OpDlg.GetInstance().labelYield.Text = $"良品率： {Math.Round(OK / ALL, 4) * 100}%";
                //ct
                // OpDlg.GetInstance().labelCt.Text = string.Format("周期： {0:F3} s", m_stData._ct);
            }));
        }
        public void WriteCsv()
        {
            try
            {
                string sn1 = "";// = OpDlg.GetInstance().m_var.GetCValue(28);
                string sn2 = "";//= OpDlg.GetInstance().m_var.GetCValue(29);
                string sn3 = "";//= OpDlg.GetInstance().m_var.GetCValue(30);
                string sn4 = "";// = OpDlg.GetInstance().m_var.GetCValue(31);

                string RFOKFlag = "";

                string CarrySn = ""; //= OpDlg.GetInstance().m_var.GetCValue("载具码");
                string HSG1 = ""; //= OpDlg.GetInstance().m_var.GetCValue("穴位1-BC码");
                string HSG2 = "";// = OpDlg.GetInstance().m_var.GetCValue("穴位2-BC码");
                string HSG3 = "";// = OpDlg.GetInstance().m_var.GetCValue("穴位3-BC码");
                string HSG4 = ""; //= OpDlg.GetInstance().m_var.GetCValue("穴位4-BC码");

                string InputTime = ""; //= OpDlg.GetInstance().m_var.GetCValue("进烤炉时间");
                string OutputTime = "";// = OpDlg.GetInstance().m_var.GetCValue("出烤炉时间");
                string TimeSpan = ""; //= OpDlg.GetInstance().m_var.GetCValue("烤炉用时");

                string ScanTime = "";// = OpDlg.GetInstance().m_var.GetCValue("扫码时间");
                string WorkTime = ""; //= OpDlg.GetInstance().m_var.GetCValue("收料时间");

                double AllNGFlag = 0; //= OpDlg.GetInstance().m_var.GetCValue("收料时间");

                m_structMgr.GetStructCval(2, 0, 0, ref sn1);
                m_structMgr.GetStructCval(2, 0, 1, ref sn2);
                m_structMgr.GetStructCval(2, 0, 2, ref sn3);
                m_structMgr.GetStructCval(2, 0, 3, ref sn4);
                m_structMgr.GetStructCval(2, 0, 4, ref RFOKFlag);
                m_structMgr.GetStructCval(2, 0, 5, ref CarrySn);
                m_structMgr.GetStructCval(2, 0, 6, ref HSG1);
                m_structMgr.GetStructCval(2, 0, 7, ref HSG2);
                m_structMgr.GetStructCval(2, 0, 8, ref HSG3);
                m_structMgr.GetStructCval(2, 0, 9, ref HSG4);
                m_structMgr.GetStructCval(2, 0, 10, ref InputTime);
                m_structMgr.GetStructCval(2, 0, 11, ref OutputTime);
                m_structMgr.GetStructCval(2, 0, 12, ref TimeSpan);
                m_structMgr.GetStructCval(2, 0, 13, ref ScanTime);
                m_structMgr.GetStructCval(2, 0, 14, ref WorkTime);
                m_structMgr.GetStructDval(2, 0, 15, ref AllNGFlag);

                DateTime dtInput = DateTime.ParseExact(InputTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                DateTime dtOutput = DateTime.ParseExact(OutputTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                DateTime St = DateTime.ParseExact(ScanTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                DateTime Wt = DateTime.ParseExact(WorkTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);

                // 设置特定文件夹的路径
                string folderPath = "D:\\TestLog\\SNData\\";

                // 检查文件夹是否存在
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");


                string filePath = $"{folderPath}{currentDate}.csv";

                // 创建一个StreamWriter以写入CSV文件
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length == 0)
                    {
                        // 写入CSV文件的标题行
                        writer.WriteLine("记录时间(s),载具码,穴位1-BC码,穴位2-BC码,穴位3-BC码,穴位4-BC码,SN1,SN2,SN3,SN4,进烤炉时间,出烤炉时间,烤炉用时(分钟),扫码时间,收料时间,抛NG标记");

                    }
                    string TimeNow = DateTime.Now.ToLongTimeString();



                    writer.WriteLine($"{TimeNow},{CarrySn},{HSG1},{HSG2},{HSG3},{HSG4},{sn1},{sn2},{sn3},{sn4},{dtInput},{dtOutput},{TimeSpan},{St},{Wt},{AllNGFlag}");
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void WriteCsv4RFID()
        {
            try
            {

                string RFMsg = OpDlg.GetInstance().m_var.GetCValue("获取的工装数据");
                string MsgResult = OpDlg.GetInstance().m_var.GetCValue("点胶工站穴位结果");

                // 设置特定文件夹的路径
                string folderPath = "D:\\TestLog\\RFIDData\\";


                // 检查文件夹是否存在
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");


                string filePath = $"{folderPath}{currentDate}.csv";

                // 创建一个StreamWriter以写入CSV文件
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length == 0)
                    {
                        // 写入CSV文件的标题行
                        writer.WriteLine("记录时间(s),获取的工装数据,点胶工站穴位结果");

                    }
                    string TimeNow = DateTime.Now.ToLongTimeString();



                    writer.WriteLine($"{TimeNow},{RFMsg},{MsgResult}");
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void WriteCsv4ScanZ()
        {
            try
            {

                double ScanPos = OpDlg.GetInstance().m_var.GetDValue("扫码成功Z轴坐标");
                // 设置特定文件夹的路径
                string folderPath = "D:\\TestLog\\ScanPosZ\\";


                // 检查文件夹是否存在
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");


                string filePath = $"{folderPath}{currentDate}.csv";

                // 创建一个StreamWriter以写入CSV文件
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length == 0)
                    {
                        // 写入CSV文件的标题行
                        writer.WriteLine("记录时间(s),扫码成功Z轴坐标");

                    }
                    string TimeNow = DateTime.Now.ToLongTimeString();
                    writer.WriteLine($"{TimeNow},{ScanPos}");
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void WriteCsv4FailSN()
        {
            try
            {
                // 设置特定文件夹的路径
                string folderPath = "D:\\TestLog\\FailSNData\\";

                // 检查文件夹是否存在
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");


                string filePath = $"{folderPath}{currentDate}.csv";

                // 创建一个StreamWriter以写入CSV文件
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length == 0)
                    {
                        // 写入CSV文件的标题行
                        writer.WriteLine("记录时间(s),扫码失败SN");

                    }
                    string TimeNow = DateTime.Now.ToLongTimeString();

                    for (int i = 0; i < 4; i++)
                    {
                        string SN = OpDlg.GetInstance().m_var.GetCValue($"扫码SN{i + 1}");
                        double Result = OpDlg.GetInstance().m_var.GetDValue($"扫码{i + 1}结果记录");
                        if (Result == 1 && SN != "NG")
                        {
                            writer.WriteLine($"{TimeNow},{SN}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        //===========================收料盘点位计算相关==================================================//

        //第一个可用的空穴位序号
        public int MovSlotIndex = 1;
        //定义穴位的信息
        public class Slot
        {
            public int isExist = 0;
            public string sn;

            public string RFOKFlag = "";

            public string CarrySn = "";

            public string HSG = "";

            public string InputTime = "";
            public string OutputTime = "";
            public string TimeSpan = "";

            public string ScanTime = "";
            public string WorkTime = "";
            public string Result = "";
        }

        //保存料盘信息
        public Dictionary<int, Slot> slotInfo = new Dictionary<int, Slot>();

        //初始化料盘信息
        public void InitSlotInfo()
        {
            if (slotInfo.Count != 25)
            {
                slotInfo.Clear();
                for (int i = 0; i <= 24; i++)
                {
                    slotInfo.Add(i, new Slot());
                }
            }
            else
            {
                for (int i = 0; i <= 24; i++)
                {
                    slotInfo[i] = new Slot();
                }
            }
        }
        //更新穴位状态
        public void UpdateSlotInfo()
        {
            slotInfo[MovSlotIndex].isExist = 1;
        }
        //寻找第一个可用的空穴位
        public int FindAvailableSlot()
        {
            foreach (var kvp in slotInfo)
            {
                if (kvp.Value.isExist == 0)
                {
                    MovSlotIndex = kvp.Key;
                    return kvp.Key;
                }
            }
            return -1;
        }

        //计算可用空穴行列 返回第几行第几个
        public void CalSlotPos(int Num, out int Row, out int Col)
        {
            int quotient = (int)(Num / 5); // 商
            int remainder = (int)(Num % 5); // 余数

            Row = quotient + 1;
            Col = remainder;

            if (remainder == 0)
            {
                Row = Row - 1;
                Col = 5;
            }
        }
        //计算运动位置与可放料吸头
        public void CalMovPosAndEnableSucker()
        {
            int Num = FindAvailableSlot();
            if (Num == -1)
            {
                //料盘已满
                OpDlg.GetInstance().m_var.SetDValue(9, -1);
                return;
            }
            MovSlotIndex = Num;
            int Row = -1;
            int Col = -1;
            CalSlotPos(Num, out Row, out Col);

            ////所有穴位状态
            // double[] SlotState = new double[2];

            //for (int i = 0; i < 2; i++)
            //{
            //SlotState[i] = OpDlg.GetInstance().m_var.GetDValue(i + 7);
            // OpDlg.GetInstance().m_var.SetDValue(8 + i, 0);
            //}

            //位置偏移
            int OffsetPos = -1;
            for (int i = 0; i < 2; i++)
            {
                string SN = OpDlg.GetInstance().m_var.GetCValue(i + 7);

                if (SN != "NG" && SN != "NULL")
                {
                    // OffsetPos = i;
                    break;
                }
            }
            if (OffsetPos == -1)
            {
                //无可放的料
                OpDlg.GetInstance().m_var.SetDValue(9, 0);
                return;
            }
            else
            {
                //可放料
                OpDlg.GetInstance().m_var.SetDValue(9, 1);
            }
            //料盘基准点
            double PosX;
            double PosY;
            double PosZ;

            NPoint pt = new NPoint();
            pt.index = 5;
            NMotionCtrl.MotionCtrl.GetInstance().GetPoint(0, ref pt);
            pt.index = 5;
            PosX = pt.pos[0];
            PosY = pt.pos[1];
            PosZ = pt.pos[2];

            //计算点位
            NPoint ptC = new NPoint();
            ptC.index = 6;
            NMotionCtrl.MotionCtrl.GetInstance().GetPoint(0, ref ptC);
            ptC.index = 6;
            ptC.pos[0] = PosX + (Col - 1 - OffsetPos) * OpDlg.GetInstance().m_var.GetDValue(10);
            ptC.pos[1] = PosY + (Row - 1) * OpDlg.GetInstance().m_var.GetDValue(11);
            ptC.pos[2] = PosZ;
            ptC.name = "放料点";
            NMotionCtrl.MotionCtrl.GetInstance().SetPoint(0, ptC);


            //for (int i = 1; i <= 4; i++)
            //{
            //    if (OffsetPos + i < 4)
            //    {
            //        if (SlotState[OffsetPos + i] == 1)
            //        {
            //            var SlotPosTemp = CalSlotPos(Num + i);
            //            if (SlotPosTemp.Item1 == SlotPos.Item1)
            //            {
            //                OpDlg.GetInstance().m_var.SetDValue(8 + OffsetPos + i, 1);
            //            }
            //            else
            //            {
            //                break;
            //            }
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        break;
            //    }

            //}
        }

        //===========================收料盘点位计算相关==================================================//


        //===========================NG料盘点位计算相关==================================================//

        //第一个可用的空穴位序号
        public int MovNGSlotIndex = -1;

        //保存料盘信息
        public Dictionary<int, Slot> slotNGInfo = new Dictionary<int, Slot>();

        //初始化料盘信息
        public void InitSlotNGInfo()
        {


            if (slotNGInfo.Count != 25)
            {
                slotNGInfo.Clear();
                for (int i = 0; i <= 24; i++)
                {
                    slotNGInfo.Add(i, new Slot());
                }
            }
            else
            {
                for (int i = 0; i <= 24; i++)
                {
                    slotNGInfo[i] = new Slot();
                }
            }

        }
        //更新穴位状态
        public void UpdateSlotNGInfo()
        {
            slotNGInfo[MovNGSlotIndex].isExist = 1;
        }
        //寻找第一个可用的空穴位
        public int FindAvailableSlotNG()
        {
            foreach (var kvp in slotNGInfo)
            {
                if (kvp.Value.isExist == 0)
                {
                    MovNGSlotIndex = kvp.Key;
                    return kvp.Key;
                }
            }
            return -1;
        }
        //寻找NG盘吸头1空穴位
        public int FindAvailableSlotNG1()
        {
            double BStationNGFlag = OpDlg.GetInstance().m_var.GetDValue("点胶工站抛NG启用标记");
            double TrayBin = OpDlg.GetInstance().m_var.GetDValue("料盘分区");
            if (BStationNGFlag == 1 && TrayBin == 1)
            {
                string slot1Result = OpDlg.GetInstance().m_var.GetCValue("穴位-结果Temp1");
                if (slot1Result.Contains("NG"))
                {
                    for (int i = 19; i > 4; i--)
                    {
                        if (slotNGInfo[i].isExist == 0)
                        {
                            MovNGSlotIndex = i;
                            return i - 5;
                            //Console.WriteLine(i);
                            //Console.WriteLine(i - 10);
                            //Console.WriteLine("==================");
                        }
                    }
                }
                else
                {
                    for (int i = 24; i > 19; i--)
                    {
                        if (slotNGInfo[i].isExist == 0)
                        {
                            MovNGSlotIndex = i;
                            return i - 5;
                            //Console.WriteLine(i);
                            //Console.WriteLine(i - 10);
                            //Console.WriteLine("==================");
                        }
                    }
                }

            }
            else
            {
                for (int i = 24; i > 4; i--)
                {
                    if (slotNGInfo[i].isExist == 0)
                    {
                        MovNGSlotIndex = i;
                        return i - 5;
                        //Console.WriteLine(i);
                        //Console.WriteLine(i - 10);
                        //Console.WriteLine("==================");
                    }
                }
            }
            return -1;
        }
        //寻找NG盘吸头2空穴位
        public int FindAvailableSlotNG2()
        {
            double BStationNGFlag = OpDlg.GetInstance().m_var.GetDValue("点胶工站抛NG启用标记");
            double TrayBin = OpDlg.GetInstance().m_var.GetDValue("料盘分区");
            if (BStationNGFlag == 1 && TrayBin == 1)
            {
                string slot2Result = OpDlg.GetInstance().m_var.GetCValue("穴位-结果Temp2");
                if (slot2Result.Contains("NG"))
                {
                    for (int i = 5; i <= 19; i++)
                    {
                        if (slotNGInfo[i].isExist == 0)
                        {
                            MovNGSlotIndex = i;
                            return i;
                            //Console.WriteLine(i);
                            //Console.WriteLine(i - 10);
                            //Console.WriteLine("==================");
                        }
                    }
                }
                else
                {
                    for (int i = 0; i <= 4; i++)
                    {
                        if (slotNGInfo[i].isExist == 0)
                        {
                            MovNGSlotIndex = i;
                            return i;
                            //Console.WriteLine(i);
                            //Console.WriteLine(i - 10);
                            //Console.WriteLine("==================");
                        }
                    }
                }

            }
            else
            {
                for (int i = 0; i <= 19; i++)
                {
                    if (slotNGInfo[i].isExist == 0)
                    {
                        MovNGSlotIndex = i;
                        return i;
                        //Console.WriteLine(i);
                        //Console.WriteLine(i - 10);
                        //Console.WriteLine("==================");
                    }
                }
            }

            return -1;
        }
        //计算可用空穴行列 返回第几行第几个
        public void CalSlotNGPos(int Num, out int Row, out int Col)
        {
            int quotient = (int)(Num / 5); // 商
            int remainder = (int)(Num % 5); // 余数

            Row = quotient + 1;
            Col = remainder;

            if (remainder == 0)
            {
                Row = Row - 1;
                Col = 5;
            }
        }
        //计算运动位置与可放料吸头
        public void CalMovPosAndEnableSuckerNG()
        {
            int Num = FindAvailableSlotNG();
            if (Num == -1)
            {
                //料盘已满
                OpDlg.GetInstance().m_var.SetDValue(14, -1);
                return;
            }
            MovNGSlotIndex = Num;
            int Row, Col;
            CalSlotNGPos(Num, out Row, out Col);

            //位置偏移
            int OffsetPos = -1;
            for (int i = 0; i < 2; i++)
            {
                string SN = OpDlg.GetInstance().m_var.GetCValue(i + 7);

                if (SN == "NG")
                {
                    OffsetPos = i;
                    break;
                }
            }
            if (OffsetPos == -1)
            {
                //无可放的料
                OpDlg.GetInstance().m_var.SetDValue(14, 0);
                return;
            }
            else
            {
                //可放料
                OpDlg.GetInstance().m_var.SetDValue(14, 1);
            }
            //料盘基准点
            double PosX;
            double PosY;
            double PosZ;

            NPoint pt = new NPoint();
            pt.index = 8;
            NMotionCtrl.MotionCtrl.GetInstance().GetPoint(0, ref pt);
            pt.index = 8;
            PosX = pt.pos[0];
            PosY = pt.pos[1];
            PosZ = pt.pos[2];

            //计算点位
            NPoint ptC = new NPoint();
            ptC.index = 9;
            NMotionCtrl.MotionCtrl.GetInstance().GetPoint(0, ref ptC);
            ptC.index = 9;
            ptC.pos[0] = PosX + (Col - 1 - OffsetPos) * OpDlg.GetInstance().m_var.GetDValue(10);
            ptC.pos[1] = PosY + (Row - 1) * OpDlg.GetInstance().m_var.GetDValue(11);
            ptC.pos[2] = PosZ;
            ptC.name = "NG放料点";
            NMotionCtrl.MotionCtrl.GetInstance().SetPoint(0, ptC);

        }

        //===========================NG料盘点位计算相关==================================================//

        //==========================Rest料盘点位计算相关==================================================//
        //保存Rest料盘信息

        //第一个可用的空穴位序号
        public int MovRestSlotIndex = 1;
        //定义穴位的信息
        public class RestSlot
        {
            public int isExist = 0;
        }
        public Dictionary<int, RestSlot> RestSlotInfo = new Dictionary<int, RestSlot>();

        //初始化料盘信息
        public void InitRestSlotInfo()
        {


            if (RestSlotInfo.Count != 36)
            {
                RestSlotInfo.Clear();
                for (int i = 0; i <= 35; i++)
                {
                    RestSlotInfo.Add(i, new RestSlot());
                }
            }
            else
            {
                for (int i = 0; i <= 24; i++)
                {
                    RestSlotInfo[i] = new RestSlot();
                }
            }

        }
        //更新穴位状态
        public void UpdateRestSlotNGInfo()
        {
            RestSlotInfo[MovRestSlotIndex].isExist = 1;
        }

        //清空穴位状态
        public void ClearRestSlotNGInfo()
        {
            RestSlotInfo[MovRestSlotIndex].isExist = 0;
        }

        //寻找缓冲台空穴位
        public int FindAvailableSlotRestS()
        {
            for (int i = 0; i <= 4; i++)
            {
                if (RestSlotInfo[i].isExist == 0)
                {
                    MovRestSlotIndex = i;
                    return i;

                }
            }
            return -1;
        }

        //寻找工装缓冲空穴位
        public int FindAvailableSlotRestM()
        {
            for (int i = 5; i <= 35; i++)
            {
                if (RestSlotInfo[i].isExist == 0)
                {
                    MovRestSlotIndex = i;
                    return i;

                }
            }
            return -1;
        }

        //寻找缓冲台有料穴位
        public int FindAvailableSlotRestExistS()
        {
            for (int i = 0; i <= 4; i++)
            {
                if (RestSlotInfo[i].isExist == 1)
                {
                    MovRestSlotIndex = i;
                    return i;

                }
            }
            return -1;
        }

        //寻找工装缓冲有料穴位
        public int FindAvailableSlotRestExistM()
        {
            for (int i = 5; i <= 35; i++)
            {
                if (RestSlotInfo[i].isExist == 1)
                {
                    MovRestSlotIndex = i;
                    return i;

                }
            }
            return -1;
        }
    }

    public class StationData
    {
        public int _total;
        public int _ok;
        public int _ng;
        public double _yeild;
        public double _ct;

        public StationData()
        {
            _total = 0;
            _ok = 0;
            _ng = 0;
            _yeild = 0;
            _ct = 0;
        }

        public void StDataReset(StationData stdata)
        {
            stdata._total = 0;
            stdata._ok = 0;
            stdata._ng = 0;
            stdata._yeild = 0;
            stdata._ct = 0;
        }
    }



}
