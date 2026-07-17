using NCommon;
using NDataBase;
using Newtonsoft.Json;
using NsDemo.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shapes;

namespace NsDemo.MachineDebug
{
    public partial class DebugUI_UPH : Form
    {
        private readonly LogWriter _products = new LogWriter()
        {
            IsEveryClose = true,
            BaseDirectory = @"D:\Data\Product\导出数据\",
            FileName = "allData",
            Encoder = Encoding.Default,
            Title = string.Empty,
            SubDirFormat = "yyyy-MM-dd",
            FileType = ".csv",
            RollingFileType = RollingFileType.EVENT_DAY_ONCE,
            IsDirectoryDate = false,
        };
        public async Task ExportAsync(DateTime strDate, string strModel)
        {

            string strPath;
            strPath = @"D:\Data\Product\";

            bool isTitle = true;
            //导出目标路径
            //D:\Data\Product\生产模式\2024-07-08-allData.csv

            _products.BaseDirectory = "D:\\Data\\Product\\导出数据\\" + strModel + "-" + strDate.ToString("yyyyMMdd") + "\\";


            for (int i = 0; i < 30; i++)
            {
                int day = -i;
                string newDate = strDate.AddDays(day).ToString("yyyy-MM-dd");
                string shift = "白班";
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        shift = "白班";
                    }
                    else
                    {
                        shift = "晚班";
                    }
                    string newStrPath = strPath + newDate + "\\" + strModel + "\\" + newDate + "-" + shift + ".csv";

                    try
                    {
                        // 打开文件以进行读取
                        using (StreamReader reader = new StreamReader(newStrPath, Encoding.Default))
                        {

                            string line = "";
                            int lineId = 0;

                            while ((line = reader.ReadLine()) != null)
                            {
                                if (isTitle)
                                {
                                    _products.Title = line + "\n";
                                }
                                else
                                {
                                    if (lineId != 0)
                                        _products.WriteLine(line);
                                }
                                isTitle = false;
                                lineId = 1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // MessageBox.Show(ex.Message);
                    }
                }

            }

            MessageBox.Show("导出完成:路径：D:\\Data\\Product\\导出数据\\");
        }

        public DebugUI_UPH()
        {
            InitializeComponent();
            Init_Header();
        }
        UPH_DataHeader uPH_DataHeader;
        public int _Day_Night;

        private CSV m_csv;
        private void Init_Header()
        {
            string path = System.Windows.Forms.Application.StartupPath;
            uPH_DataHeader = UPH_DataHeader.Serialize2Obj(path + @"\MachineConfig\Product\Tossing.json");
            _Day_Night = 0;


        }

        public void LoadProductDataList()
        {

            m_listUph.Clear();
            string strPath, strShift, strModel, strDate;
            strPath = @"D:\Data\Product\";
            strDate = dateTimePicker_Uph.Value.ToString("yyyy-MM-dd");
            strShift = comboBox1.Text;
            strModel = comboBox2.Text;
            strPath = strPath + strDate + "\\" + strModel + "\\" + strDate + "-" + strShift + ".csv";

            string[] AnalysisData;
            int num = 0;
            if (!File.Exists(strPath))
            {
                MessageBox.Show("无记录!");
                return;
            }

            m_csv = new CSV(strPath);
            if (m_csv == null)
            {
                return;
            }

            m_listUph.Clear();
            for (int i = 0; i < m_csv.Data.Count; i++)
            {
                AnalysisData = m_csv.Data[i].ToArray();
                if (i == 0)
                {
                    m_listUph.Columns.Add("索引", 100, HorizontalAlignment.Center);
                    for (int j = 0; j < AnalysisData.Length; j++)
                    {
                        m_listUph.Columns.Add(AnalysisData[j].ToString(), 100, HorizontalAlignment.Center);
                    }
                }
                else
                {
                    ListViewItem lviItem = new ListViewItem(i.ToString());
                    for (int j = 0; j < AnalysisData.Length; j++)
                    {
                        lviItem.SubItems.Add(AnalysisData[j].ToString());
                    }
                    m_listUph.Items.Add(lviItem);
                }
            }
        }


        public void LoadUphDataList()
        {

            m_listUph.Clear();
            ImageList iList = new ImageList();
            iList.ImageSize = new Size(1, 32);
            this.m_listUph.SmallImageList = iList;
            int TotalWidth = 0;
            int TotalIndex = 0;

            for (int i = 0; i < uPH_DataHeader._List_Dataheader.Count(); i++)
            {
                if (uPH_DataHeader._List_Dataheader[i].UI_Subitem_Width < 0)
                {
                    TotalIndex++;
                }
                else
                {
                    TotalWidth += uPH_DataHeader._List_Dataheader[i].UI_Subitem_Width;
                }
            }

            int average_width;
            if (TotalIndex <= 0)
            {
                average_width = 0;
            }
            else
            {
                average_width = (int)((this.Width * 1.0 - TotalWidth - 20) / TotalIndex);
            }

            for (int i = 0; i < uPH_DataHeader._List_Dataheader.Count(); i++)
            {

                if (uPH_DataHeader._List_Dataheader[i].UI_Subitem_Width < 0)
                {
                    m_listUph.Columns.Add(uPH_DataHeader._List_Dataheader[i].UI_Subitem_Description, average_width, HorizontalAlignment.Center);
                }
                else
                {
                    m_listUph.Columns.Add(uPH_DataHeader._List_Dataheader[i].UI_Subitem_Description, uPH_DataHeader._List_Dataheader[i].UI_Subitem_Width, HorizontalAlignment.Center);
                }
            }


            for (int i = 0; i < 13; i++)
            {
                string strShift_Day_Night;
                if (_Day_Night == 0)
                {
                    strShift_Day_Night = string.Format("{0:D2}:00:00 -- {1:D2}:00:00", (i + 8) % 24, (i + 9) % 24);
                }
                else
                {
                    strShift_Day_Night = string.Format("{0:D2}:00:00 -- {1:D2}:00:00", (i + 20) % 24, (i + 21) % 24);
                }
                if (i == 12)
                {
                    strShift_Day_Night = "统计";
                }

                ListViewItem item1 = new ListViewItem(strShift_Day_Night);
                if (i % 2 == 0)
                {
                    item1.BackColor = Color.LightGreen;
                }
                else
                {
                    item1.BackColor = Color.GreenYellow;
                }

                for (int j = 0; j < uPH_DataHeader._List_Dataheader.Count(); j++)
                {
                    if (uPH_DataHeader._List_Dataheader[j].Match == "时间")
                    {
                        continue;
                    }
                    else if (uPH_DataHeader._List_Dataheader[j].Match == "良率")
                    {
                        double f_yield = uPH_DataHeader.Yield_Statistics(i);
                        if (f_yield < 0)
                        {
                            item1.SubItems.Add("");
                        }
                        else if (f_yield < 20)
                        {
                            item1.SubItems.Add(f_yield.ToString("0.00") + "%");
                            item1.ForeColor = Color.Red;
                        }
                        else
                        {
                            item1.SubItems.Add(f_yield.ToString("0.00") + "%");
                        }
                    }
                    else
                    {
                        string strtxt = "";
                        if (uPH_DataHeader._List_Dataheader[j].Total[i] > 0)
                        {
                            strtxt = uPH_DataHeader._List_Dataheader[j].Total[i].ToString();
                        }
                        item1.SubItems.Add(strtxt);
                    }
                }
                m_listUph.Items.Add(item1);
            }


            //不良率统计
            ListViewItem item2 = new ListViewItem("不良率统计");
            item2.BackColor = Color.GreenYellow;
            item2.ForeColor = Color.Red;
            for (int j = 0; j < uPH_DataHeader._List_Dataheader.Count(); j++)
            {
                if (uPH_DataHeader._List_Dataheader[j].Match == "时间")
                {
                    continue;
                }
                else if (uPH_DataHeader._List_Dataheader[j].Match == "良率")
                {
                    continue;
                }
                else
                {
                    string strtxt = "";
                    double f_NG_Yield = uPH_DataHeader.Yield_NGType(uPH_DataHeader._List_Dataheader[j].Match);
                    if (f_NG_Yield > 0)
                    {

                        strtxt = f_NG_Yield.ToString("0.00") + "%";
                    }
                    else
                    {
                        strtxt = "";
                    }
                    item2.SubItems.Add(strtxt);
                }
            }
            m_listUph.Items.Add(item2);
        }

        private void DebugUI_UPH_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            LoadUphDataList();
        }

        private void DrawChartUph()//绘制柱状曲线图
        {
            int OktotalVal = 0, total = 0;
            double yield;

            for (int n = 0; n < chartUph.Series.Count(); n++)
            {
                chartUph.Series[n].Points.Clear();
            }

            for (int i = 0; i < 12; i++)
            {
                OktotalVal = uPH_DataHeader._List_Dataheader[2].Total[i];
                total = uPH_DataHeader._List_Dataheader[1].Total[i];
                chartUph.Series[0].Points.AddXY(i + 1, total); //产量总数

                yield = uPH_DataHeader.Yield_Statistics(i);
                chartUph.Series[1].Points.AddXY(i + 1, yield); //良率
                chartUph.Series[2].Points.AddXY(i + 1, OktotalVal); //ok总数

                chartUph.Series[0].Points[i].Label = string.Format("{0}", uPH_DataHeader._List_Dataheader[3].Total[i]);//显示NG标签,OK与总数叠加后剩余是NG部分
                chartUph.Series[1].Points[i].Label = string.Format("{0:F2}%", yield);//折线绘制百分比标签
                chartUph.Series[2].Points[i].Label = string.Format("{0}", total);//显示产量总数标签

                if (total == 0)
                {
                    chartUph.Series[1].Points[i].Label = " ";
                    chartUph.Series[0].Points[i].LabelForeColor = Color.Black;
                    chartUph.Series[2].Points[i].Label = " ";

                }
            }

        }

        private void dateTimePicker_Uph_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strPath, strShift, strModel, strDate;
            strPath = @"D:\Data\Product\";
            strDate = dateTimePicker_Uph.Value.ToString("yyyy-MM-dd");
            strShift = comboBox1.Text;
            strModel = comboBox2.Text;
            if (strShift == "白班")
            {
                _Day_Night = 0;
            }
            else
            {
                _Day_Night = 1;
            }
            strPath = strPath + strDate + "\\" + strModel + "\\" + strDate + "-" + strShift + ".csv";
            uPH_DataHeader.LoadData(strPath);
            LoadUphDataList();
            DrawChartUph();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadProductDataList();
        }

        private void DebugUI_UPH_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }


    public class UPH_Tossing_info
    {

        public UPH_Tossing_info(string strErrorCode)
        {
            _List_UPH_Code = new List<string>();
            string[] list_errorcode = strErrorCode.Split(',');
            key_Tossing.Add("Tossing", 0);
            key_Tossing.Add("OK", 0);
            key_Tossing.Add("NG", 0);
            _List_UPH_Code.Add("Tossing");
            _List_UPH_Code.Add("OK");
            _List_UPH_Code.Add("NG");

            for (int i = 0; i < list_errorcode.Length; i++)
            {
                if (!key_Tossing.ContainsKey(list_errorcode[i]))
                {
                    key_Tossing.Add(list_errorcode[i], 0);
                    _List_UPH_Code.Add(list_errorcode[i]);
                }
            }

            if (!key_Tossing.ContainsKey("Other_Error"))
            {
                key_Tossing.Add("Other_Error", 0);
                _List_UPH_Code.Add("Other_Error");
            }
        }
        List<string> str_DataBuff = new List<string>();
        Dictionary<string, int> key_Tossing = new Dictionary<string, int>();
        List<string> _List_UPH_Code;
        public List<string> Statistics_by_hours(int hourIndex)
        {
            List<string> rtn = new List<string>();
            for (int i = 0; i < str_DataBuff.Count; i++)
            {
                string[] list_Data = str_DataBuff[i].Split(',');
                if (list_Data.Length > 10)
                {
                    continue;
                }
                int Index = int.Parse(list_Data[2]);
                if (hourIndex != Index)
                {
                    continue;
                }

                if (list_Data[4] != null && list_Data[4] != "NA" && list_Data[4] != "")
                {
                    key_Tossing["投入"]++; //投入++
                    if (list_Data[6].Contains("OK"))
                    {
                        key_Tossing["产出"]++; //OK++
                    }
                    else
                    {
                        if (key_Tossing.ContainsKey(list_Data[7]))
                        {
                            key_Tossing[list_Data[7]]++; //对应错误代码的++
                            key_Tossing["次品"]++;
                        }
                    }
                }
            }


            for (int i = 0; i < _List_UPH_Code.Count; i++)
            {
                if (key_Tossing.ContainsKey(_List_UPH_Code[i]))
                {
                    rtn.Add(key_Tossing[_List_UPH_Code[i]].ToString());
                }
            }
            return rtn;
        }


        public string LoadInfo(string PathName)
        {
            if (!Directory.Exists(PathName))
            {
                return "";
            }

            str_DataBuff.Clear();

            FileStream fs = new FileStream(PathName, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            List<string> lines = new List<string>();
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                str_DataBuff.Add(line);
            }
            sr.Close();

            return "";


        }
       

    }


    public class UPH_Dataheader_subitem
    {
        public string Match;//匹配数据
        public string UI_Subitem_Description;//描述
        public int[] Total;//统计数量
        public int UI_Subitem_Width;//列表宽度
        public UPH_Dataheader_subitem(string strDescription)
        {
            Match = strDescription;
            UI_Subitem_Description = strDescription;
            Total = new int[13] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            UI_Subitem_Width = -1;
        }
    }
    public class UPH_DataHeader
    {
        private int indexResullStatus = 13;
        private int indexResult = 13;
        public List<UPH_Dataheader_subitem> _List_Dataheader;

        public double Yield_Statistics(int Index)
        {
            double f_Yield = 0;
            if (Index < 0 || Index >= 13)
            {
                return 0;
            }
            int Pass_Count = 0;
            int All_Count = 0;
            int Fail_Count = 0;
            for (int i = 0; i < _List_Dataheader.Count; i++)
            {
                if (_List_Dataheader[i].Match.ToUpper() == "ALL")
                {
                    All_Count = _List_Dataheader[i].Total[Index];
                }
                else if (_List_Dataheader[i].Match.ToUpper() == "OK")
                {
                    Pass_Count = _List_Dataheader[i].Total[Index];
                }
                else if (_List_Dataheader[i].Match.ToUpper() == "NG")
                {
                    Fail_Count = _List_Dataheader[i].Total[Index];
                }
            }
            if (All_Count > 0)
            {
                f_Yield = Pass_Count * 100.00 / All_Count;
            }
            else
            {
                f_Yield = -1;
            }

            return f_Yield;

        }
        public static UPH_DataHeader Serialize2Obj(string filePath)
        {
            try
            {
                // 打开文件以进行读取
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string content = reader.ReadToEnd();
                    UPH_DataHeader list_UPH_DataHeader = JsonConvert.DeserializeObject<UPH_DataHeader>(content);
                    return list_UPH_DataHeader;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }


        public UPH_DataHeader()
        {
            //_List_Dataheader = new List<UPH_Dataheader_subitem>();
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem1  = new UPH_Dataheader_subitem("时间");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem2  = new UPH_Dataheader_subitem("投入");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem3  = new UPH_Dataheader_subitem("良品");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem4  = new UPH_Dataheader_subitem("次品");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem5  = new UPH_Dataheader_subitem("扫码NG");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem6  = new UPH_Dataheader_subitem("MES查询NG");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem7  = new UPH_Dataheader_subitem("Primer超时");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem8  = new UPH_Dataheader_subitem("Plasma超时");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem9  = new UPH_Dataheader_subitem("点胶定位NG");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem10 = new UPH_Dataheader_subitem("断胶");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem11 = new UPH_Dataheader_subitem("同心度NG");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem12 = new UPH_Dataheader_subitem("胶宽NG");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem13 = new UPH_Dataheader_subitem("OpenTime 超时");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem14 = new UPH_Dataheader_subitem("组装角度NG");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem15 = new UPH_Dataheader_subitem("FI2NG");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem16 = new UPH_Dataheader_subitem("FI6NG");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem17 = new UPH_Dataheader_subitem("MES上传NG");
            //UPH_Dataheader_subitem uPH_Dataheader_Subitem18 = new UPH_Dataheader_subitem("良率");
            //_List_Dataheader.Add(uPH_Dataheader_Subitem1 );
            //_List_Dataheader.Add(uPH_Dataheader_Subitem2 );
            //_List_Dataheader.Add(uPH_Dataheader_Subitem3 );
            //_List_Dataheader.Add(uPH_Dataheader_Subitem4 );
            //_List_Dataheader.Add(uPH_Dataheader_Subitem5 );
            //_List_Dataheader.Add(uPH_Dataheader_Subitem6 );
            //_List_Dataheader.Add(uPH_Dataheader_Subitem7 );
            //_List_Dataheader.Add(uPH_Dataheader_Subitem8 );
            //_List_Dataheader.Add(uPH_Dataheader_Subitem9 );
            //_List_Dataheader.Add(uPH_Dataheader_Subitem10);
            //_List_Dataheader.Add(uPH_Dataheader_Subitem11);
            //_List_Dataheader.Add(uPH_Dataheader_Subitem12);
            //_List_Dataheader.Add(uPH_Dataheader_Subitem13);
            //_List_Dataheader.Add(uPH_Dataheader_Subitem14);
            //_List_Dataheader.Add(uPH_Dataheader_Subitem15);
            //_List_Dataheader.Add(uPH_Dataheader_Subitem16);
            //_List_Dataheader.Add(uPH_Dataheader_Subitem17);
            //_List_Dataheader.Add(uPH_Dataheader_Subitem18);

            //SerializeToString(this);

        }


        public string SerializeToString(UPH_DataHeader obj)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(obj);

                string filePath = @"D:\file.json"; // 替换为实际的文件路径

                try
                {
                    // 打开文件以进行写入
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.Write(jsonData);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("发生异常： " + ex.Message);
                }


                return jsonData;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void ClearItem()
        {
            if (_List_Dataheader == null)
            {
                return;
            }
            for (int i = 0; i < _List_Dataheader.Count; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    _List_Dataheader[i].Total[j] = 0;
                }
            }
        }

        public void LoadData(string filePath)
        {
            try
            {
                // 打开文件以进行读取
                using (StreamReader reader = new StreamReader(filePath, Encoding.Default))
                {
                    ClearItem();
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {

                        analyzeData(line);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void analyzeData(string line)
        {
            try
            {
                string[] vec_Data = line.Split(new[] { "," }, StringSplitOptions.None);
                if (vec_Data.Count() < 4)
                {
                    return;
                }


                int Index = int.Parse(vec_Data[2]);
                Index = (Index - 8 + 24) % 12;

                for (int i = 0; i < _List_Dataheader.Count(); i++)
                {
                    if (vec_Data[indexResullStatus] == "OK")
                    {
                        if (_List_Dataheader[i].Match.ToUpper() == "OK")
                        {
                            _List_Dataheader[i].Total[Index]++;
                            _List_Dataheader[i].Total[12]++;
                        }
                        else if (_List_Dataheader[i].Match.ToUpper() == "ALL")
                        {
                            _List_Dataheader[i].Total[Index]++;
                            _List_Dataheader[i].Total[12]++;
                        }
                    }
                    else if (vec_Data[indexResullStatus].Contains("NG"))
                    {
                        if (_List_Dataheader[i].Match.ToUpper() == "NG")
                        {
                            _List_Dataheader[i].Total[Index]++;
                            _List_Dataheader[i].Total[12]++;
                        }
                        else if (_List_Dataheader[i].Match.ToUpper() == "ALL")
                        {
                            _List_Dataheader[i].Total[Index]++;
                            _List_Dataheader[i].Total[12]++;
                        }
                        else if (_List_Dataheader[i].Match == vec_Data[indexResult])
                        {
                            _List_Dataheader[i].Total[Index]++;
                            _List_Dataheader[i].Total[12]++;
                        }

                    }
                }
            }
            catch
            {

            }



        }

        public double Yield_NGType(string NGType)
        {
            double f_Yield;
            int All_Count = 0;
            int Fail_Count = 0;
            if (NGType.ToUpper() == "ALL" || NGType.ToUpper() == "OK" || NGType.ToUpper() == "NG")
            {
                return -2;
            }
            for (int i = 0; i < _List_Dataheader.Count; i++)
            {
                if (_List_Dataheader[i].Match.ToUpper() == "ALL")
                {
                    All_Count = _List_Dataheader[i].Total[12];
                }
                else if (_List_Dataheader[i].Match == NGType)
                {
                    Fail_Count = _List_Dataheader[i].Total[12];
                }

            }
            if (All_Count > 0)
            {
                f_Yield = Fail_Count * 100.00 / All_Count;
            }
            else
            {
                f_Yield = -1;
            }

            return f_Yield;
        }
       
       

    }
   

}
