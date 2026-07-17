using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NCommunicationTool;
using System.IO;
using NsDemo.Utility;


namespace NsDemo.RFIDSys
{
    public partial class US_RFID : UserControl
    {
        public US_RFID()
        {
            InitializeComponent();
            Init();
        }

        public ComuManage _SockeManage;
        public List<RFConfigInfo> _ListRFConfig;
        public List<RFDataInfo> _ListRFData;

        public Dictionary<string,Ctrol> _DictionaryRFData = new Dictionary<string, Ctrol>();

        private void Init()
        {
            _SockeManage = ComuManage.GetInstance();
            _ListRFData = new List<RFDataInfo>();
            _ListRFConfig = new List<RFConfigInfo>();

            string strPath = Path.Combine(Application.StartupPath, "MachineConfig\\RF数据\\RF通讯配置.csv");
            LoadConfig(strPath);

            string strRFPath = Path.Combine(Application.StartupPath, "MachineConfig\\RF数据\\RF数据项配置.csv");
            LoadRFDataItem(strRFPath);


            //初始化RFID通信对象控件
            for(int i = 0; i < _ListRFConfig.Count; i++)
            {
                comboBox1.Items.Add(_ListRFConfig[i]._strName);
            }
            //问题，为什么这里加了之后，会导致DebugUI_Input界面加载时报错
            //comboBox1.SelectedIndex = 0;

            //绑定RF名和控件名
            _DictionaryRFData.Add("载具码", new Ctrol(textBox1,button6));

            _DictionaryRFData.Add("穴位1-BC码", new Ctrol(textBox2,button2));
            _DictionaryRFData.Add("穴位2-BC码", new Ctrol(textBox3,button3));
            _DictionaryRFData.Add("穴位3-BC码", new Ctrol(textBox4,button4));
            _DictionaryRFData.Add("穴位4-BC码", new Ctrol(textBox5,button5));
            /*
            _DictionaryRFData.Add("穴位1-WD码", textBox9);
            _DictionaryRFData.Add("穴位2-WD码", textBox8);
            _DictionaryRFData.Add("穴位3-WD码", textBox7);
            _DictionaryRFData.Add("穴位4-WD码", textBox6);
            */
            foreach(KeyValuePair<string,Ctrol> kvp in _DictionaryRFData)
            {
                _DictionaryRFData[kvp.Key]._writeButton.Click  += new System.EventHandler(this.button_Click);
            }

        }

        private void RF_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                TextBox Bt1 = sender as TextBox;
                foreach (KeyValuePair<string, Ctrol> kvp in _DictionaryRFData)
                {
                    //匹配控件对应的RF名字
                    if (Bt1 != _DictionaryRFData[kvp.Key]._textBox)
                    {
                        continue;
                       
                    }
                    for (int i = 0; i < _ListRFData.Count; i++)
                    {
                        //匹配RF名字对应的_ListRFData项
                        if (kvp.Key != _ListRFData[i].strName)
                        {
                            continue;
                        }
                        
                        //1.首先要判断输入的字符长度是否与配置文件中一致
                        if(Bt1.Text.Replace(" ","").Length != _ListRFData[i].DataActLength)
                        {
                            MessageBox.Show("数据长度不正确,现输入位数是" + Bt1.Text.Replace(" ", "").Length + "，请确保输入" + _ListRFData[i].DataLength + "位");
                            return;
                        }
                        _ListRFData[i].strData = Bt1.Text.Replace(" ", "");
                        //2.生成写入指令
                        RFTool.WriteRFCommand(_ListRFData[i].StartAddress.ToString(), _ListRFData[i].strData);
                        //3.写入RF芯片中
                        int RDCommID = comboBox1.SelectedIndex;
                        WriteData2RF(RDCommID, _ListRFData[i].strData);

                    }
                }
            }
            
            
        }

        private bool WriteData2RF(int SocketID, string strSendData)
        {
            string[] vec_Data = strSendData.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            _SockeManage.SendData(SocketID, strSendData, 0, vec_Data.Count(), true);
            System.Threading.Thread.Sleep(300);

            byte[] recvBuff = new byte[1024];
            string strRecvAllData = "";
            int RecvCount = _SockeManage.RecvData(SocketID, recvBuff, 1024);
            for (int j = 0; j < RecvCount; j++)
            {
                strRecvAllData += recvBuff[j].ToString("X2") + " ";
            }

            richTextBox1.Text = "发送\r\n" + strSendData + "\r\n接收" + strRecvAllData;
            return true;
        }


        private void LoadConfig(string strPath)
        {
            try
            {
                FileStream fs = new FileStream(strPath,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs,System.Text.Encoding.Default);
                List<string> lines = new List<string>();
                string line = "";
                while((line = sr.ReadLine()) != null)
                {
                    if(line.Contains("名称"))
                    {
                        continue;
                    }
                    analyzeData(line);
                }
                sr.Close();
                fs.Close();
            }
            
            catch (Exception ex)
            {

            }
        }

        private void analyzeData(string line)
        {
            try
            {
                string[] vec_data = line.Split(new[] {","},StringSplitOptions.None);
                if(vec_data.Count() < 2)
                {
                    return;
                }
                RFConfigInfo CurrentItem = new RFConfigInfo();
                CurrentItem._strName = vec_data[0];
                CurrentItem._nID = int.Parse(vec_data[1]);
                _ListRFConfig.Add(CurrentItem);
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

        private void button1_Click(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;
            if(index < 0 || index >= _ListRFConfig.Count)
            {
                return;
            }
            string strReadData = "";
            strReadData = ReadRFData(_ListRFConfig[index]._nID);
            if (GetRFData(strReadData))
            {
                RefreshUI();
            }
        }

        private void RefreshUI()
        {
            foreach(KeyValuePair<string,Ctrol> kvp in _DictionaryRFData)
            {
                for(int i = 0; i < _ListRFData.Count(); i++)
                {
                    if (kvp.Key ==_ListRFData[i].strName)
                    {
                        _DictionaryRFData[kvp.Key]._textBox.Text = _ListRFData[i].strData;
                    }
                }
                
            }
        }

        private string ReadRFData(int SocketID)
        {
            
            //读取“1---125”
            string str1 = "FF 08 11 00 01 00 00 01 7D D2 B3";
            //读取“126-250”
            string str2 = "FF 08 11 00 01 00 00 7E 7D E2 93";
            //读取“251-375”
            string str3 = "FF 08 11 00 01 00 00 FB 7D 72 F1";
            List<string> listComand = new List<string>() { str1,str2,str3};
            byte[] recvBuff = new byte[1024];
            List<Byte> _ListData = new List<Byte>();

            //三次读取接收到的数据和
            string strRecvAllData = "";
            string log = "";
            for(int index = 0; index < 3; index++)
            {
                //每次读取后接收到的数据
                string strRecvData = "";
                try
                {
                    //清空Comm缓冲区的数据
                    int kkk = _SockeManage.RecvData(SocketID, recvBuff, 1024);

                    //发送数据
                    bool ret = _SockeManage.SendData(SocketID, listComand[index], 0, 11, true);
                    System.Threading.Thread.Sleep(100);


                    //接收数据
                    for (int i = 0; i < 40; i++)
                    {
                        System.Threading.Thread.Sleep(50);
                        int RecvCount = _SockeManage.RecvData(SocketID, recvBuff, 1024);
                        for (int j = 0; j < RecvCount; j++)
                        {
                            _ListData.Add(recvBuff[j]);
                        }
                        if (_ListData.Count > 130)
                        {
                            for (int k = 0; k < _ListData.Count(); k++)
                            {
                                strRecvData += _ListData[k].ToString("X2") + " ";
                            }
                            break;
                        }

                    }
                }
                catch (System.Exception ex)
                {
                    strRecvData = ex.Message;
                }
                log += "发送\r\n" + listComand[index] + "\r\n接收\r\n" + strRecvData + "\r\n";
                richTextBox1.Text = log;

                strRecvAllData += strRecvData + ",";
            }
            

            return strRecvAllData;
        }

        public bool GetRFData(string strThreeData)
        {
            try
            {
                string[] strData = strThreeData.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                //存储三次接收的数据位组合
                string strAllData = "";
                for(int i = 0; i < strData.Length - 1; i++)
                {
                    string OKFlag = "11 80 01 00 00";
                    int index = strData[i].LastIndexOf(OKFlag);
                    if (index < 0)
                    {
                        return false;
                    }
                    //去掉前面的（head + 读取成功标志 + 最后的校验位[包括一个空格])
                    strData[i] = strData[i].Substring(index + OKFlag.Length, strData[i].Length - index - OKFlag.Length - 6);
                    strAllData += strData[i];
                }
                //读取是从1-375，所以要有375个数据
                string[] vec_RF_AllData = strAllData.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if(vec_RF_AllData.Length < 125)
                {
                    MessageBox.Show("读取失败，接收数据位数不对！！");
                    return false;
                }

                //解析到_ListRFData里面去
                for(int i = 0; i < _ListRFData.Count; i++)
                {
                    string strItem = "";
                    for (int j = 0; j < _ListRFData[i].DataLength; j++)
                    {
                       
                        //需要从哪里开始截取，如果起始地址是“1”，那么就从vec_RF_AllData的“0”位置开始截取(startAdress-1)
                       Byte c_RFData = Convert.ToByte(vec_RF_AllData[_ListRFData[i].StartAddress - 1 + j], 16);
                        if(c_RFData != 0)
                        {
                            char chardata = (char)c_RFData;
                            strItem += chardata;
                        }
                        
                    }
                    _ListRFData[i].strData = strItem;
                }
            }
            catch
            {

            }
            return true;
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button Bt1 = sender as Button;
            foreach (KeyValuePair<string, Ctrol> kvp in _DictionaryRFData)
            {
                //匹配控件对应的RF名字
                if (Bt1 != _DictionaryRFData[kvp.Key]._writeButton)
                {
                    continue;
                       
                }
                for (int i = 0; i < _ListRFData.Count; i++)
                {
                    //匹配RF名字对应的_ListRFData项
                    if (kvp.Key != _ListRFData[i].strName)
                    {
                        continue;
                    }
                        
                    //1.首先要判断输入的字符长度是否与配置文件中一致
                    if(_DictionaryRFData[kvp.Key]._textBox.Text.Replace(" ","").Length != _ListRFData[i].DataActLength)
                    {
                        MessageBox.Show("数据长度不正确,现输入位数是" + Bt1.Text.Replace(" ", "").Length + "，请确保输入" + _ListRFData[i].DataLength + "位");
                        return;
                    }
                    _ListRFData[i].strData = _DictionaryRFData[kvp.Key]._textBox.Text.Replace(" ", "");
                    //2.生成写入指令
                    RFTool.WriteRFCommand(_ListRFData[i].StartAddress.ToString(), _ListRFData[i].strData);
                    //3.写入RF芯片中
                    int RDCommID = comboBox1.SelectedIndex;
                    WriteData2RF(RDCommID, _ListRFData[i].strData);

                }
            }
        }
    }

    public class RFConfigInfo
    {
        public int _nID;
        public string _strName;
    }

    public class RFDataInfo
    {
        public string strName;
        public int StartAddress;
        public int DataLength;      //预留地址长度
        public string strData;
        public int DataActLength;   //实际长度
    }

    public class Ctrol
    {
        public TextBox _textBox;
        public Button _writeButton;
        public Ctrol(TextBox textBox,Button writeButton)
        {
            _textBox = textBox;
            _writeButton = writeButton;
        }
    }
}
