using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCommunicationTool;
using System.IO;
using NsDemo.Utility;
using System.Windows.Forms;
using NVarConfig;
using log4net.Core;

/*********************************************************************************************************/
/*2023-04-11
修改RF读取写入成功标志，新增重连断开Log*/
/*********************************************************************************************************/

namespace NsDemo.RFIDSys
{

    class RFTool
    {
        public ComuManage _SocketManage;
        public List<RFDataInfo> _ListRFData;
        public VarInteface _varsReg;

        private readonly LogWriter _RFLog = new LogWriter()
        {
            IsEveryClose = true,
            BaseDirectory = @"d:\Data\RF收发记录",
            Encoder = Encoding.Default,
            //SubDirFormat = "yyyy-MM-dd",
            FileType = ".txt",
            //Title = "时间,\n",
            RollingFileType = RollingFileType.EVENT_DAY_ONCE
        };

        Dictionary<int, int> _CommIndex;
        int TimeOutCount;

        public string GetErrorCode(int ComID, string strData)
        {
            if (strData.IndexOf("80 01 80") >= 0)
            {
                DisConnectComm(ComID);
                return "指令执行失败：一般出现的原因是无标签处于可读区域 或者标签读取失败";
            }
            else if (strData.IndexOf("80 01 82") >= 0)
            {
                DisConnectComm(ComID);
                return "参数错误：请检查读写起始地址 读写长度是否配置正确";
            }
            else if (strData.IndexOf("80 01 83") >= 0)
            {
                DisConnectComm(ComID);
                return "读写器发生未知错误";
            }
            else if (strData.IndexOf("80 01 90") >= 0)
            {
                DisConnectComm(ComID);
                return "无标签在可读区域：此错误码将在指令即时有效的情况下 无标签在可读区域时返回";
            }
            else if (strData.IndexOf("80 01 91") >= 0)
            {
                DisConnectComm(ComID);
                return "射频数据传输错误：数据传输时发生格式错误，可能原因射频受到干扰 标签处理临界区域 或者有多个标签在可读区域";
            }
            else if (strData.IndexOf("80 01 92") >= 0)
            {
                DisConnectComm(ComID);
                return "写数据地址错误：标签没有请求的数据地址";
            }
            else if (strData.IndexOf("80 01 93") >= 0)
            {
                DisConnectComm(ComID);
                return "写数据块被锁 所请求的数据地址被锁 不可再执行写操作";
            }
            else if (strData.IndexOf("80 01 94") >= 0)
            {
                DisConnectComm(ComID);
                return "写数据块失败：所请求的数据地址编程失败 可能是此标签已达到规写次数";
            }
            else if (strData.IndexOf("80 01 95") >= 0)
            {
                DisConnectComm(ComID);
                return "读取标签失败 读取一半标签离开标签：移动过快或标签所在的区域读写器工作不稳定，读写器无法完成全部的读取数据操作";
            }
            else if (strData.IndexOf("80 01 96") >= 0)
            {
                DisConnectComm(ComID);
                return "写标签失败 写标签一部分数据后标签离开：标签移动过快或标签所在的区域读写器工作不稳定，读写器无法完成全部的写数据操作";
            }
            else if (strData.IndexOf("80 01 97") >= 0)
            {
                DisConnectComm(ComID);
                return "写写标签失败 写标签一部分后射频数据传输错误：读写器写了一部分数据后受到干扰导致数据传输异常";
            }
            else if (strData.IndexOf("80 01 98") >= 0)
            {
                DisConnectComm(ComID);
                return "数据校验错误：仅在启用了读写校验时可能出现";
            }
            else if (strData.IndexOf("80 01 99") >= 0)
            {
                DisConnectComm(ComID);
                return "标签型号解析错误：读取的标签型号暂不能兼容";
            }
            else if (strData.IndexOf("80 01 A1") >= 0)
            {
                DisConnectComm(ComID);
                return "射频模块异常：射频模块有异常现象";
            }
            else if (strData.IndexOf("80 01 B0") >= 0)
            {
                DisConnectComm(ComID);
                return "参数有误：读写指令的数据格式不对 或操作的数据大于限定的长度";
            }
            return "OK";
        }
        public int Init()
        {
            _SocketManage = ComuManage.GetInstance();
            string strRFPath = Path.Combine(Application.StartupPath, "MachineConfig\\RF数据\\RF数据项配置.csv");
            LoadRFDataItem(strRFPath);
            _varsReg = VarInteface.GetInstance();

            _CommIndex = new Dictionary<int, int>();

            try
            {
                TimeOutCount = int.Parse(_varsReg.GetCValue("RF超时次数"));
                if (TimeOutCount == 0)
                {
                    TimeOutCount = 20;
                }
            }
            catch (System.Exception ex)
            {
                TimeOutCount = 20;
            }

            return 0;
        }


        /**********************************************************************/
        /* 串口读取错误大于10次重连*/

        public void DisConnectComm(int ComID)
        {
            try
            {
                if (_CommIndex.ContainsKey(ComID) != true)
                {
                    _CommIndex.Add(ComID, 0);
                    _CommIndex[ComID]++;
                }
                else
                {
                    if (_CommIndex[ComID] > TimeOutCount)
                    {
                        DisConnect(ComID);
                        _CommIndex[ComID] = 0;
                    }
                    else
                    {
                        _CommIndex[ComID]++;
                    }

                }
            }
            catch (System.Exception ex)
            {

            }


        }
        /*****************************************************************************/

        public string ReadRFData(string strMsg)
        {
            string strrtnData = "";
            string errorCode = "";
            //读取RF数据(60002,1#RF)
            string[] vec_Data = strMsg.Split(new[] { "(", ",", ")" }, StringSplitOptions.RemoveEmptyEntries);
            if (vec_Data.Count() < 5)
            {
                return "Error";
            }
            string strRegName = vec_Data[4];
            int CommID = Convert.ToInt32(vec_Data[1]);
            int ReadTime125 = 2;     //读取多少个125个长度 （由于RFID硬件不允许一次性不能读取太长的数据）,只读取一次 
            int nTimeout = Convert.ToInt32(vec_Data[2]);
            int TryAgain = Convert.ToInt32(vec_Data[3]);
            string strData = "";
            for (int i = 0; i < TryAgain; i++)
            {
                strData = ReadRFBuff(CommID, ReadTime125, nTimeout);
                if (strData.IndexOf("Error") >= 0)
                {
                    errorCode = strData;
                    continue;
                }
                bool rtnFlag = false;
                //得到了部分配置文件里面的数据(RF数据名，数据)
                List<string> List_AllRFBuff = GetRFData(strData, ReadTime125, ref rtnFlag);
                //if (List_AllRFBuff.Count() != _ListRFData.Count())
                //{
                //    continue;
                //}


                if (rtnFlag == true)
                {
                    for (int j = 5; j < vec_Data.Count(); j++)
                    {
                        for (int k = 0; k < List_AllRFBuff.Count(); k++)
                        {
                            string[] AllRFBuff = List_AllRFBuff[k].Split(new[] { "," }, StringSplitOptions.None);
                            string RFName = AllRFBuff[0];
                            string RFLength = AllRFBuff[1];
                            string RFData = AllRFBuff[2];
                            if (vec_Data[j] == RFName)
                            {
                                //如果读取到的数据，去掉空格后和配置文件中不一致，则报错   要排除NA和NG的情况
                                //if (RFData.Replace(" ", "").Length != int.Parse(RFLength) && RFData.Replace(" ", "").Length != 2)
                                //{
                                //    errorCode = "NG(数据长度不对等)";
                                //    break;
                                //}
                                strrtnData = strrtnData + "," + RFData;
                                break;
                            }
                            //如果匹配到最后都没有在List_AllRFBuff中找到要读取的数据,那证明读取的位数不够
                            else if (k == List_AllRFBuff.Count)
                            {
                                errorCode = "NG(读取位数不够，没有读取到所需的数据)";
                                break;
                            }
                            else
                            {
                                //strrtnData = strrtnData + "," + RFData;
                            }
                        }
                    }
                    if (errorCode.IndexOf("NG") >= 0)
                    {
                        _varsReg.SetCValue(strRegName, errorCode + strrtnData);
                        //GlobalEvent.ShowLog("RFID读取失败 ",errorCode + strrtnData,System.Drawing.Color.Red);
                        return "NG" + strrtnData;
                    }
                    else
                    {
                        strrtnData = strrtnData.Replace(" ","");
                        _varsReg.SetCValue(strRegName, "OK" + strrtnData);
                        //GlobalEvent.ShowLog("RFID读取成功 ","OK" + strrtnData);
                        return "OK" + strrtnData;
                    }
                }
            }
            _varsReg.SetCValue(strRegName, "NG" + errorCode);
            //GlobalEvent.ShowLog("RFID读取失败 ","NG" + errorCode,System.Drawing.Color.Red);
            return "TimeOut " + errorCode;
        }


        private void analyzeRFItem(string line)
        {
            try
            {
                string[] vec_Data = line.Split(new[] { "," }, StringSplitOptions.None);
                if (vec_Data.Count() < 3)
                {
                    return;
                }

                RFDataInfo CurrentItem = new RFDataInfo();
                CurrentItem.strName = vec_Data[0];
                CurrentItem.StartAddress = int.Parse(vec_Data[1]);
                CurrentItem.DataLength = int.Parse(vec_Data[2]);
                CurrentItem.strData = vec_Data[3];
                CurrentItem.DataActLength = int.Parse(vec_Data[4]);
                _ListRFData.Add(CurrentItem);
            }
            catch
            {
                int kk = 0;
            }

        }

        private void LoadRFDataItem(string strPath)
        {
            try
            {
                _ListRFData = new List<RFDataInfo>();
                FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                List<string> lines = new List<string>();
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("名称"))
                    {
                        continue;
                    }

                    analyzeRFItem(line);
                }

                sr.Close();
                fs.Close();
            }
            catch
            {
            }
        }

        private bool DisConnect(int COMID)
        {
            _SocketManage.UninitObject(COMID);
            DisConnectLog(COMID);
            return true;
        }

        private bool Connect(int COMID, int Timeout = 3000)
        {
            _SocketManage.UninitObject(COMID);
            System.Threading.Thread.Sleep(300);

            _SocketManage.InitObject(COMID);

            DateTime StartTime = DateTime.Now;
            while (true)
            {
                int CurrentSocketStatus = _SocketManage.GetObjectStatus(COMID);
                if (CurrentSocketStatus == 2)
                {
                    ConnectLog(COMID, "成功");
                    return true;
                }
                System.Threading.Thread.Sleep(30);
                TimeSpan Diff = DateTime.Now - StartTime;
                if (Diff.TotalMilliseconds > Timeout)
                {
                    ConnectLog(COMID, "失败");
                    return false;
                }
            }
        }

        private string ReadRFBuff(int ComID, int ReadTime125, int nTimeout)
        {

            //读取“1---125”
            string str1 = "FF 08 11 00 01 00 00 01 7D D2 B3";
            //读取“126-250”
            string str2 = "FF 08 11 00 01 00 00 7E 7D E2 93";
            //读取“251-375”
            string str3 = "FF 08 11 00 01 00 00 FB 7D 72 F1";
            List<string> listComand = new List<string>() { str1, str2, str3 };
            byte[] recvBuff = new byte[1024];
            List<Byte> _ListData = new List<Byte>();
            string strRecvAllData = "";
            DateTime StartTime = DateTime.Now;
            if (ReadTime125 > 3)
            {
                return "Error,读取125个地址的次数不能大于3次";
            }
            try
            {
                for (int i = 0; i < ReadTime125; i++)
                {
                    string strRecvData = ""; //每读取一次接收的数据
                    //清空Comm缓冲区的数据
                    int kkk = _SocketManage.RecvData(ComID, recvBuff, 1024);

                    //发送数据
                    bool sendFlag = _SocketManage.SendData(ComID, listComand[i], 0, 11, true);
                    //LOG
                    SendDataLog(ComID, listComand[i]);
                    if (sendFlag == false)
                    {
                        _SocketManage.InitObject(ComID);
                        Connect(ComID);
                        _SocketManage.RecvData(ComID, recvBuff, 1024);
                        sendFlag = _SocketManage.SendData(ComID, listComand[i], 0, 11, true);
                        //LOG
                        SendDataLog(ComID, listComand[i]);
                    }
                    System.Threading.Thread.Sleep(10);

                    StartTime = DateTime.Now;

                    //接收数据
                    while (true)
                    {
                        _ListData.Clear();
                        System.Threading.Thread.Sleep(10);
                        int RecvCount = _SocketManage.RecvData(ComID, recvBuff, 1024);
                        for (int j = 0; j < RecvCount; j++)
                        {
                            _ListData.Add(recvBuff[j]);
                        }
                        if (_ListData.Count >= 130)     //如果大于130，代表每一次读取接收成功
                        {
                            strRecvData = "";
                            for (int k = 0; k < _ListData.Count(); k++)
                            {
                                strRecvData += _ListData[k].ToString("X2") + " ";
                            }
                            //LOG
                            RecvDataLog(ComID, null, strRecvData);
                            string strMsg = strRecvData + ",";
                            strRecvAllData += strMsg;   //如果每次都读取成功，则把每次的数据累加起来
                            break;
                        }
                        else
                        {
                            string strbuff = "";
                            for (int k = 0; k < _ListData.Count(); k++)
                            {
                                strbuff += _ListData[k].ToString("X2") + " ";
                            }
                            string strErrorCode = GetErrorCode(ComID, strbuff);
                            //LOG
                            RecvDataLog(ComID, null, strbuff);
                            if (strErrorCode != "OK")
                            {
                                return "(Error" + strErrorCode + ")";
                            }
                        }
                        TimeSpan Diff = DateTime.Now - StartTime;
                        if (Diff.TotalMilliseconds > nTimeout)
                        {
                            DisConnect(ComID);
                            return "Error Timeout";
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                strRecvAllData = "Error" + ex.Message;
            }



            return strRecvAllData;
        }

        public string RF_WriteData(string strMsg)
        {
            string strrtnData = "OK";

            //读取RF数据(60002,1#RF)
            string[] vec_Data = strMsg.Split(new[] { "(", ",", ")" }, StringSplitOptions.RemoveEmptyEntries);
            if (vec_Data.Count() < 5)
            {
                return "Error";
            }


            int CommID = Convert.ToInt32(vec_Data[1]);
            int DataLength = -1; ;
            int nTimeout = Convert.ToInt32(vec_Data[2]);
            int TryAgain = Convert.ToInt32(vec_Data[3]);
            string strResultReg = vec_Data[4];
            int TryCount = 0;

            for (int j = 6; j < vec_Data.Count(); j = j + 2)
            {
                string strItemName = vec_Data[j - 1];
                string strRegName = vec_Data[j];

                int RFAddress = -1;
                //获取地址
                for (int i = 0; i < _ListRFData.Count(); i++)
                {
                    if (strItemName == _ListRFData[i].strName)
                    {
                        RFAddress = _ListRFData[i].StartAddress;
                        DataLength = _ListRFData[i].DataLength;
                    }
                }



                //判断写入的数据长度
                string strData = _varsReg.GetCValue(strRegName);
                strData = strData.Replace(" ", "");
                //if (strData.Length != DataLength)
                //{
                //    _varsReg.SetCValue(strResultReg, "NG数据长度不对等");
                //    return "NG数据长度不对等";
                //}


                //将数据转换成16进制的字符
                byte[] ba = Encoding.Default.GetBytes(strData);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in ba)
                {
                    sb.Append(b.ToString("x"));
                }
                string newData = sb.ToString();

                for (int i = ba.Count(); i < DataLength; i++)
                {
                    newData += "00";
                }

                //生成RF指令
                string strRFComm = WriteRFCommand(RFAddress.ToString(), newData);
                string result = "";
                //写入数据
                for (int k = TryCount; k < TryAgain; k++)
                {
                    result = WriteData2RF(CommID, strRFComm, nTimeout);
                    if (result == "OK")
                    {
                        break;
                    }
                }
                if (result != "OK")
                {
                    strrtnData = "NG " + result;
                    break;
                }


            }
            _varsReg.SetCValue(strResultReg, strrtnData);
            //GlobalEvent.ShowLog("RFID写入",strrtnData);
            return strrtnData;
        }

        public string RF_Clear(string strMsg)
        {

            //读取RF数据(60002,1#RF)
            string[] vec_Data = strMsg.Split(new[] { "(", ",", ")" }, StringSplitOptions.RemoveEmptyEntries);
            if (vec_Data.Count() < 5)
            {
                return "Error";
            }


            int CommID = Convert.ToInt32(vec_Data[1]);
            int nTimeout = Convert.ToInt32(vec_Data[2]);
            int TryAgain = Convert.ToInt32(vec_Data[3]);
            int StartAddress = Convert.ToInt32(vec_Data[4]);
            int Length = Convert.ToInt32(vec_Data[5]);
            string strResultReg = vec_Data[6];

            if (Length > 120)
            {
                _varsReg.SetCValue(strResultReg, "NG 数据长度大于120");
                return "NG 数据长度大于120";
            }

            string newData = "";
            for (int i = 0; i < Length; i++)
            {
                newData += "00";
            }

            //生成RF指令
            string strRFComm = WriteRFCommand(StartAddress.ToString(), newData);
            string result = "";
            //写入数据
            for (int k = 0; k < TryAgain; k++)
            {
                result = WriteData2RF(CommID, strRFComm, nTimeout);
                if (result == "OK")
                {
                    _varsReg.SetCValue(strResultReg, "OK");
                    return "OK";
                }
            }
            _varsReg.SetCValue(strResultReg, "NG " + result);
            return "NG " + result;
        }



        private string WriteData2RF(int ComID, string strSendData, int TimeOut)
        {
            try
            {
                byte[] recvBuff = new byte[1024];
                string[] vec_Data = strSendData.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                //发送数据
                int RecvCount = _SocketManage.RecvData(ComID, recvBuff, 1024);
                bool SendFlag = _SocketManage.SendData(ComID, strSendData, 0, vec_Data.Count(), true);
                //LOG
                SendDataLog(ComID, strSendData);
                if (SendFlag == false)
                {
                    _SocketManage.InitObject(ComID);
                    Connect(ComID);
                    RecvCount = _SocketManage.RecvData(ComID, recvBuff, 1024);
                    _SocketManage.SendData(ComID, strSendData, 0, vec_Data.Count(), true);
                    //LOG
                    SendDataLog(ComID, strSendData);
                }

                //延时等待
                System.Threading.Thread.Sleep(20);
                DateTime StartTime = DateTime.Now;
                string strRecvAllData = "";
                while (true)
                {
                    RecvCount = _SocketManage.RecvData(ComID, recvBuff, 1024);
                    for (int j = 0; j < RecvCount; j++)
                    {
                        strRecvAllData += recvBuff[j].ToString("X2") + " ";
                    }
                    //LOG
                    RecvDataLog(ComID, null, strRecvAllData);
                    //if (strRecvAllData.IndexOf("80 01 00 00") >= 0)
                    if (strRecvAllData.IndexOf("FF 06 12 80 01 00 00 6A D4") >= 0)
                    {
                        return "OK";
                    }
                    else
                    {
                        string strErrorCode;
                        strErrorCode = GetErrorCode(ComID, strRecvAllData);
                        if (strErrorCode != "OK")
                        {
                            return "NG " + strErrorCode;
                        }
                    }

                    TimeSpan Diff = DateTime.Now - StartTime;
                    if (Diff.TotalMilliseconds > TimeOut)
                    {
                        DisConnect(ComID);
                        return "NG 返回数据超时";
                    }
                }
            }
            catch (System.Exception ex)
            {
                return "NG " + ex.Message;
            }


        }



        public List<string> GetRFData(string strTimeData, int ReadTime125, ref bool Flag)
        {
            List<string> rtn_List = new List<string>();
            Flag = true;
            try
            {
                string[] strData = strTimeData.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                //存储三次接收的数据位组合
                string strAllData = "";
                for (int i = 0; i < strData.Length; i++)
                {
                    string OKFlag = "11 80 01 00 00 ";
                    int index = strData[i].LastIndexOf(OKFlag);
                    if (index < 0 || index > 100)
                    {
                        Flag = false;
                        return rtn_List;
                    }
                    //去掉前面的（head + 读取成功标志 + 最后的校验位[包括一个空格])
                    strData[i] = strData[i].Substring(index + OKFlag.Length, strData[i].Length - index - OKFlag.Length - 6);
                    strAllData += strData[i];
                }
                //读取是从1-375，所以要有375个数据
                string[] vec_RF_AllData = strAllData.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (vec_RF_AllData.Length < 125 * ReadTime125)
                {
                    Flag = false;
                    return rtn_List;
                }

                //解析到_ListRFData里面去
                for (int i = 0; i < _ListRFData.Count; i++)
                {
                    string strItem = "";
                    //在此之前，还需要判断，读取的数据是不是足够解析配置文件里的数据
                    if (_ListRFData[i].StartAddress + _ListRFData[i].DataLength > vec_RF_AllData.Length)
                    {
                        //如果不够后面的数据，就不继续往下面解析了
                        break;
                    }
                    for (int j = 0; j < _ListRFData[i].DataLength; j++)
                    {


                        //需要从哪里开始截取，如果起始地址是“1”，那么就从vec_RF_AllData的“0”位置开始截取(startAdress-1)
                        Byte c_RFData = Convert.ToByte(vec_RF_AllData[_ListRFData[i].StartAddress - 1 + j], 16);
                        if (c_RFData != 0)
                        {
                            char chardata = (char)c_RFData;
                            strItem += chardata;
                        }

                    }
                    _ListRFData[i].strData = strItem;
                    string nameAndData = _ListRFData[i].strName + "," + _ListRFData[i].DataLength + "," + _ListRFData[i].strData;
                    rtn_List.Add(nameAndData);
                }
            }
            catch
            {
                Flag = false;
            }
            return rtn_List;
        }





        //生成RF写入指令
        public static string WriteRFCommand(string address, string data)
        {
            //获取数据byte长度
            data = data.Replace(" ", "");
            int length = data.Length / 2;
            string strLength = length.ToString();

            //生成RF写入指令
            byte ParaLen = 0;
            byte[] Para = new byte[256];
            ushort Writeaddr = ushort.Parse(address);
            ushort ctrlflg = ushort.Parse("0001");
            byte id = byte.Parse("00");
            byte TotalRespLen = byte.Parse("100");

            Para[0] = (byte)((Writeaddr >> 8) & 0xff);
            Para[1] = (byte)((Writeaddr >> 0) & 0xff);
            Para[2] = byte.Parse(strLength);
            ParaLen = (byte)(3 + Para[2]);

            byte[] bdata = Tool.HexStringToByte(data.Trim(), 0);
            if (bdata.Length != Para[2])
            {
                MessageBox.Show("数据长度不正确");
                return "";
            }
            Array.Copy(bdata, 0, Para, 3, Para[2]);

            byte[] WByte = PackData(0x12, ctrlflg, id, TotalRespLen, ParaLen, Para);
            string RFCommand = Tool.ByteToHexString(WByte, 0, WByte.Length, " ");
            return RFCommand;
        }
        private static byte[] PackData(byte cmd, ushort ctrl, byte rid, byte TotalRespLen, byte paralen, byte[] para)
        {
            int i = 0; int cnt = 0;
            byte ctrl_h = (byte)(ctrl >> 8 & 0xff);
            byte ctrl_l = (byte)(ctrl >> 0 & 0xff);
            int ArrayLen = 5 + paralen;
            byte[] packdata = new byte[ArrayLen + 2];
            packdata[i++] = 0xff;
            packdata[i++] = (byte)(ArrayLen - 1);
            packdata[i++] = cmd;
            packdata[i++] = ctrl_h;
            packdata[i++] = ctrl_l;
            if ((ctrl_l & 0x01) == 0x01)
            {
                cnt++;
                packdata[i++] = rid;
            }
            if ((ctrl_l & 0x04) == 0x04)
            {
                cnt++;
                packdata[i++] = TotalRespLen;
            }
            ArrayLen += cnt;
            if (paralen > 0)
            {
                Array.Copy(para, 0, packdata, i, paralen);
            }

            packdata[1] = (byte)(ArrayLen - 1);
            ushort CRC = GetCRC16(packdata, 0, ArrayLen);
            byte[] ReturnData = new byte[ArrayLen + 2];
            Array.Copy(packdata, 0, ReturnData, 0, ArrayLen);
            ReturnData[ArrayLen] = (byte)(CRC >> 8 & 0xff);
            ReturnData[ArrayLen + 1] = (byte)(CRC >> 0 & 0xff);
            return ReturnData;
        }
        public static UInt16 GetCRC16(byte[] data, int offset, int len)
        {
            UInt16 wCRC = 0xFFFF;
            int i = 0;
            byte chChar = 0x00;

            for (i = offset; i < len + offset; i++)
            {
                chChar = data[i];
                wCRC = (ushort)(wCRCTalbeAbs[(chChar ^ wCRC) & 15] ^ (wCRC >> 4));
                wCRC = (ushort)(wCRCTalbeAbs[((chChar >> 4) ^ wCRC) & 15] ^ (wCRC >> 4));
            }

            return wCRC;
        }
        static UInt16[] wCRCTalbeAbs = new UInt16[]{
            0x0000, 0xCC01, 0xD801, 0x1400,
            0xF001, 0x3C00, 0x2800, 0xE401,
            0xA001, 0x6C00, 0x7800, 0xB401,
            0x5000, 0x9C01, 0x8801, 0x4400,
        };


        private void SendDataLog(int CommID, string sendBuff)
        {
            try
            {
                _RFLog.FileName = DateTime.Now.ToString("yyyy-MM-dd") + "_" + CommID.ToString();
                string content = "";
                /*时间：
                 * 发送数据：
                 */
                content = "时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _RFLog.WriteLine(content);
                content = CommID.ToString() + "发送数据：";
                _RFLog.WriteLine(content);
                _RFLog.WriteLine(sendBuff);
            }
            catch (System.Exception ex)
            {

            }


        }

        private void RecvDataLog(int CommID, List<Byte> ListData = null, string recvBuff = "")
        {
            try
            {
                _RFLog.FileName = DateTime.Now.ToString("yyyy-MM-dd") + "_" + CommID.ToString();
                string content = "";
                /*时间：
                 * 发送数据：
                 */
                content = "时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _RFLog.WriteLine(content);
                content = CommID.ToString() + "接收数据：";
                _RFLog.WriteLine(content);
                content = "";
                if (ListData != null)
                {
                    for (int i = 0; i < ListData.Count; i++)
                    {
                        content += ListData[i].ToString();
                    }
                }
                if (recvBuff != "")
                {
                    content = recvBuff;
                }
                _RFLog.WriteLine(content);
                _RFLog.WriteLine(" ");
            }
            catch (System.Exception ex)
            {

            }


        }

        private void DisConnectLog(int ComID)
        {
            try
            {
                _RFLog.FileName = DateTime.Now.ToString("yyyy-MM-dd") + "_" + ComID.ToString();
                string content = "时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
                _RFLog.WriteLine(content);
                _RFLog.Write("断开连接");
                _RFLog.Write(" ");
            }
            catch (System.Exception ex)
            {

            }

        }
        private void ConnectLog(int ComID, string IsOk)
        {
            try
            {
                _RFLog.FileName = DateTime.Now.ToString("yyyy-MM-dd") + "_" + ComID.ToString();
                string content = "时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + IsOk + "\n";
                _RFLog.WriteLine(content);
                _RFLog.Write("重连" + content);
                _RFLog.Write(" ");
            }
            catch
            {

            }
        }

    }
}
