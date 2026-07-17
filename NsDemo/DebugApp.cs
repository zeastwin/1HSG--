using Newtonsoft.Json;
using NsDemo.IdDevice;
using NsDemo.MESSys;
using NsDemo.PCode;
using NsDemo.Utility;
using NsDemo.Virtual_IO;
using NUser;
using NVarConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NsDemo
{
    public partial class DebugApp : Form
    {
        public UserManager m_users;
        public DebugApp()
        {
            InitializeComponent();
            m_users = UserManager.GetInstance();
        }
        private UI_FingerPrint m_FingerPrint = new UI_FingerPrint();
        private void MainView_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void MainView_Load(object sender, EventArgs e)
        {
        }

        private void btnFrmFinger_Click(object sender, EventArgs e)
        {
            m_FingerPrint.StartPosition = FormStartPosition.CenterScreen;
            m_FingerPrint.Show();
            m_FingerPrint.BringToFront();
            m_FingerPrint.WindowState = FormWindowState.Normal;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            string strRecive = MES_Fun.Instance.API_MES_Action("查询", SendBox.Text.Trim());
            ReceiveBox.Clear();
            ReceiveBox.Text = strRecive;
        }

        private void btnUpLoad_Click(object sender, EventArgs e)
        {
            string strUpLoadRecive = MES_Fun.Instance.API_MES_Action("过站", SendBox.Text.Trim());
            ReceiveBox.Clear();
            ReceiveBox.Text = strUpLoadRecive;
        }
        private void btnRePOST_Click(object sender, EventArgs e)
        {
            try
            {
                short level = m_users.GetGurUserLevel();
                if (level == 2)
                {
                    // 文件夹路径
                    string folderPath = "";
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                        openFileDialog.Multiselect = false;

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            folderPath = openFileDialog.FileName;
                        }
                        else
                        {
                            return;
                        }
                    }
                    // 读取原始CSV文件内容
                    List<string> lines = File.ReadAllLines(folderPath, Encoding.Default).ToList();
                    // 处理每一行
                    GlobalEvent.ShowMsg($"=========MES补传流程启动=========", OpDlg.Level.Normal);
                    for (int i = 1; i < lines.Count; i++)
                    {
                        string line = lines[i];
                        string[] columns = line.Split(',');

                        if (columns.Length > 0)
                        {
                            string strHsgSN = columns[6];  //HSG
                            string strWDSN = columns[7];  //WD
                            string InputTime = columns[8];  //进烤炉时间
                            string OutputTime = columns[9];  //出烤炉时间
                                                             //  string oventime = columns[10];  //烤炉用时
                            string ResultFlag = columns[14];  //ResultFlag
                            string BeForeStationResultFlag = columns[15];  //点胶工站RFFlag
                            string MESFlag = columns[16];  //MES结果
                            string NGFlag = columns[17];  //点胶工站结果

                            //if (BeForeStationResultFlag != "5")
                            //    continue;
                            if (NGFlag.Contains("NG"))
                            {
                                GlobalEvent.ShowMsg($"{strHsgSN}" + "该物料点胶工站NG，禁止上传", OpDlg.Level.Normal);
                                continue;
                            }
                            if (MESFlag == "OK")
                            {
                                GlobalEvent.ShowMsg($"{strHsgSN}" + "该物料MES上传OK，禁止重复上传", OpDlg.Level.Normal);
                                continue;
                            }

                            DateTime timeTemp = DateTime.Now;
                            if (strHsgSN != "" && strHsgSN != "NA" && strHsgSN != "NG")
                            {
                                if (strWDSN != "" && strWDSN != "NA" && strWDSN != "NG" && strWDSN != "NULL")
                                {
                                    procuctCode procuctInfo = CheckProductInfo(strHsgSN);
                                    if (procuctInfo != null)
                                    {
                                        try
                                        {
                                            bool isReUpload = false;
                                            for (int j = procuctInfo.history.Count() - 1; j >= 0; j--)
                                            {
                                                try
                                                {
                                                    if (procuctInfo.history[j].data.insight.test_station_attributes.station_id.Contains("DEVELOPMENT13"))
                                                    {
                                                        WriteLog4ReUpload($"{strHsgSN}" + "该物料电阻测试机已过站，禁止重复上传" + "\r\n");
                                                        GlobalEvent.ShowMsg($"{strHsgSN}" + "该物料电阻测试机已过站，禁止重复上传", OpDlg.Level.Normal);
                                                        isReUpload = true;
                                                        break;
                                                    }
                                                    if (procuctInfo.history[j].data.insight.test_station_attributes.software_name == "HSG上料机")
                                                    {
                                                        if (procuctInfo.history[j].data.insight.test_attributes.test_result == "pass")
                                                        {
                                                            WriteLog4ReUpload($"{strHsgSN}" + "该物料结果已OK，禁止重复上传" + "\r\n");
                                                            GlobalEvent.ShowMsg($"{strHsgSN}" + "该物料结果已OK，禁止重复上传", OpDlg.Level.Normal);
                                                            isReUpload = true;
                                                            break;
                                                        }
                                                    }

                                                }
                                                catch (Exception EX)
                                                {
                                                    continue;
                                                }

                                            }
                                            if (isReUpload)
                                                continue;
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                    }
                                    string strJson = "{\"cBarCode\":\"" + strHsgSN + "\"}";
                                    WriteLog4ReUpload("查询" + strJson + "\r\n");
                                    string strRecive = MES_Fun.Instance.API_MES_Action("查询", strJson);
                                    WriteLog4ReUpload("查询结果" + strRecive + "\r\n");
                                    bc_window_assy_mesinfo obj = SerializeToVariable(strRecive);
                                    try
                                    {
                                        if (obj != null)
                                        {
                                            if (obj.data != null && obj.data.insight != null && obj.data.insight.uut_attributes != null)
                                            {
                                                if (obj.data.insight.results != null)
                                                {
                                                    if (obj.data.insight.results.Length < 3)
                                                    {
                                                        GlobalEvent.ShowMsg($"{i + 1}号穴物料:{strHsgSN}点胶测试项不足，取消上传", OpDlg.Level.Normal);
                                                        WriteLog4ReUpload($"物料:{strHsgSN}点胶测试项不足，取消上传");
                                                        continue;
                                                    }
                                                }
                                                // obj = CoreWork.Instance.SetLimitParam(obj);
                                                //if (obj == null)
                                                //    continue;
                                                try
                                                {
                                                    DateTime dtInput = DateTime.ParseExact(obj.data.insight.test_attributes.uut_start, "MM-dd-yyyy HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                                                    obj.data.insight.test_attributes.uut_start = dtInput.ToString("yyyy-MM-dd HH:mm:ss");
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            }
                                            else
                                            {
                                                double ReUpLoadAutoCreate = OpDlg.GetInstance().m_var.GetDValue("补传自动生成");
                                                if (ReUpLoadAutoCreate == 1)
                                                {
                                                    obj = CoreWork.Instance.bcBaseInfo;
                                                    obj.data.insight.test_attributes.unit_serial_number = strHsgSN;
                                                    obj.data.insight.test_attributes.uut_start = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                                    obj.data.insight.test_attributes.uut_stop = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                                    obj.serials.fg = strHsgSN;

                                                    obj.data.insight.uut_attributes.bc_window_assy_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");
                                                    obj.data.insight.uut_attributes.glue_dispense_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");

                                                    obj.data.insight.uut_attributes.fixture_id = "C-SM-00078";

                                                    obj.data.insight.uut_attributes.prima_expiry_date = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");

                                                    obj.data.insight.uut_attributes.prima_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");

                                                    obj.data.insight.uut_attributes.prima_stop_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");

                                                    obj.data.insight.uut_attributes.standing_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");

                                                    obj.data.insight.uut_attributes.standing_stop_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");

                                                    obj.data.insight.uut_attributes.bc_window_assy_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");

                                                    obj.data.insight.uut_attributes.plasma_start_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");

                                                    obj.data.insight.uut_attributes.plasma_stop_time = timeTemp.ToString("yyyy-MM-dd HH:mm:ss");

                                                    obj.data.insight.uut_attributes.prima_lot = "3090510";

                                                    obj.data.insight.uut_attributes.prima_vendor = "EW";
                                                }
                                                else
                                                {
                                                    GlobalEvent.ShowMsg($"{strHsgSN}" + "该物料MES查询信息为空，请检查上料机点胶机上传情况", OpDlg.Level.Normal);
                                                    continue;
                                                }
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

                                    //更新数据
                                    //obj.data.insight.test_station_attributes.line_id = VarInteface.GetInstance().GetCValue("线体ID");
                                    //obj.data.insight.test_station_attributes.software_name = VarInteface.GetInstance().GetCValue("设备软件名");
                                    //obj.data.insight.test_station_attributes.software_version = VarInteface.GetInstance().GetCValue("设备软件版本");
                                    //obj.data.insight.test_station_attributes.station_id = VarInteface.GetInstance().GetCValue("工站ID");


                                    try
                                    {
                                        //DateTime timeTemp = DateTime.ParseExact("20000101010101", "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);


                                        obj.data.insight.uut_attributes.bc_window_sn = strWDSN;
                                        DateTime dtInput;
                                        DateTime dtOutput;
                                        try
                                        {
                                            dtInput = DateTime.ParseExact(InputTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                                            dtOutput = DateTime.ParseExact(OutputTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                                        }
                                        catch
                                        {
                                            dtInput = timeTemp;
                                            dtOutput = timeTemp;
                                        }


                                        obj.data.insight.uut_attributes.oven_assy_time = dtInput.ToString("yyyy-MM-dd HH:mm:ss");
                                        obj.data.insight.uut_attributes.oven_start_time = dtInput.ToString("yyyy-MM-dd HH:mm:ss");
                                        obj.data.insight.uut_attributes.oven_stop_time = dtInput.ToString("yyyy-MM-dd HH:mm:ss");
                                        obj.data.insight.test_attributes.uut_stop = dtOutput.ToString("yyyy-MM-dd HH:mm:ss");
                                        obj.data.insight.test_station_attributes.station_id = VarInteface.GetInstance().GetCValue("工站ID");

                                        double MESAutoCreate = OpDlg.GetInstance().m_var.GetDValue("MES自动填充");
                                        if (MESAutoCreate == 1)
                                        {

                                            if (obj.data.insight.uut_attributes.fixture_id == "")
                                                obj.data.insight.uut_attributes.fixture_id = "C-SM-00078";
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
                                                obj.data.insight.uut_attributes.prima_lot = "3090510";
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

                                        double ForceOk = OpDlg.GetInstance().m_var.GetDValue("补传强制OK");
                                        if (ForceOk == 1)
                                        {
                                            obj.data.insight.test_attributes.test_result = "pass";
                                        }
                                        else
                                        {
                                            if (ResultFlag == "0")
                                            {
                                                obj.data.insight.test_attributes.test_result = "pass";
                                            }
                                            else
                                            {
                                                obj.data.insight.test_attributes.test_result = "scrap";
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        obj.data.insight.test_attributes.test_result = "scrap";
                                    }



                                    //查询物料材质并生成url
                                    string procuctCode = CheckProductCode(strHsgSN);
                                    // string procuctCode = "N217-TI";
                                    WriteLog4ReUpload("查询材质结果" + procuctCode + "\r\n");
                                    if (procuctCode == "NG")
                                    {
                                        GlobalEvent.ShowMsg($"{strHsgSN}" + $"该物料查询材质失败", OpDlg.Level.Normal);
                                        continue;
                                    }



                                    //string str = "{\"data\":{\"insight\":{\"results\":[{\"result\":\"pass\",\"test\":\"glue_open_time\",\"units\":\"s\",\"value\":\"40\"}],\"test_attributes\":{\"test_result\":\"pass\",\"unit_serial_number\":\"FP2041300PBP2M66629\",\"uut_start\":\"2018 - 09 - 18 20:41:33\",\"uut_stop\":\"2018 - 09 - 18 20:41:33\"},\"test_station_attributes\":{\"line_id\":\"IPGL_C09 - 3FA\",\"software_name\":\"DEVELOPMENT1\",\"software_version\":\"V1.111\",\"station_id\":\"Site_LineID_MachineID_StationName\"},\"uut_attributes\":{\"bc_window_assy_start_time\":\"yyyy - mm - dd hh: mm: ss\",\"bc_window_sn\":\"\",\"cavity_id\":\"TBD\",\"fixture_id\":\"ABC0001\",\"glue_dispense_start_time\":\"yyyy - mm - dd hh: mm: ss\",\"oven_start_time\":\"\",\"oven_stop_time\":\"\",\"plasma_start_time\":\"yyyy - mm - dd hh: mm: ss\",\"plasma_stop_time\":\"yyyy - mm - dd hh: mm: ss\",\"prima_expiry_date\":\"yyyy - mm - dd hh: mm: ss\",\"prima_lot\":\"ABC0001\",\"prima_open_time\":\"yyyy - mm - dd hh: mm: ss\",\"prima_vendor\":\"JQS\",\"standing_start_time\":\"yyyy - mm - dd hh: mm: ss\",\"standing_stop_time\":\"yyyy - mm - dd hh: mm: ss\",\"station_vendor\":\"JQS\"}}},\"serials\":{\"fg\":\"FP2041300PBP2M66629\"}}";

                                    //上传
                                    string strUpLoadRecive = "";
                                    try
                                    {
                                        string objJson = obj.Serialize2Json();
                                        WriteLog4ReUpload("上传" + objJson + "\r\n");
                                        // string strUpLoadRecive = MES_Fun.Instance.API_MES_Action("过站", objJson);
                                        string PDCAurl = OpDlg.GetInstance().m_var.GetCValue("PDCAurl");
                                        PDCAurl = PDCAurl + procuctCode;
                                        strUpLoadRecive = SendToPDCA(PDCAurl, objJson);
                                        WriteLog4ReUpload("上传结果" + strUpLoadRecive + "\r\n");
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteLog4ReUpload("上传结果" + strUpLoadRecive + "\r\n");
                                        GlobalEvent.ShowMsg($"{strHsgSN}" + $"该物料上传异常{ex.Message}", OpDlg.Level.Normal);
                                        continue;
                                    }


                                    try
                                    {
                                        //MesResultJson mesJson = new MesResultJson();
                                        //mesJson = mesJson.SerializeToVariable(strUpLoadRecive);
                                        bool isSucceed = CheckIsUploaded(strUpLoadRecive);
                                        if (isSucceed)
                                        {
                                            // 更新特定的单元格
                                            columns[16] = "OK";
                                            lines[i] = string.Join(",", columns);



                                        }
                                        else
                                        {

                                            continue;
                                        }
                                    }
                                    catch
                                    {

                                        continue;
                                    }
                                }
                                else
                                {

                                }
                            }
                            else
                            {

                            }


                        }
                        // 写入更新后的内容到文件中
                        File.WriteAllLines(folderPath, lines, Encoding.Default);
                    }
                    GlobalEvent.ShowMsg($"=========MES补传流程结束=========", OpDlg.Level.Normal);
                    MessageBox.Show("MES补传流程结束");
                }
                else
                {
                    MessageBox.Show("请登录管理员账号后进行补传");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public procuctCode CheckProductInfo(string HSG)
        {
            string sUrl = "http://17.80.224.220/api/v2/parts?serial_type=fg&serial=";
            sUrl = sUrl + $"{HSG.Trim().ToUpper()}";

            string sMESname = OpDlg.GetInstance().m_var.GetCValue("MES_username");
            string spassword = OpDlg.GetInstance().m_var.GetCValue("MES_password");
            //  WriteLog4MesReEDIT("查询Product" + sUrl + HSG + "\r\n");
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(sUrl) as HttpWebRequest;
                request.Credentials = new NetworkCredential(sMESname, spassword);
                request.Method = "GET";
                request.Timeout = 1000;
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //         WriteLog4MesReEDIT("查询结果" + responseString + "\r\n");
                //解析出procuctCode
                procuctCode procuctC = new procuctCode();
                procuctC = SerializeToProcuctCode(responseString);
                if (procuctC != null && procuctC.history != null && procuctC.history.Count() != 0)
                {
                    return procuctC;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
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
        public void WriteLog4ReUpload(string str)
        {
            // 设置特定文件夹的路径
            string folderPath = "D:\\TestLog\\MesReUpLoadlog\\";


            // 检查文件夹是否存在
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            //查询是否有文件存在，如果不存在则创建
            string path = "D:\\TestLog\\MesReUpLoadlog\\" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                // 检查文件夹是否存在
                if (!Directory.Exists("D:\\TestLog\\MesReUpLoadlog\\"))
                {
                    Directory.CreateDirectory("D:\\TestLog\\MesReUpLoadlog\\");
                }
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
        public string CheckProductCode(string HSG)
        {
            string sUrl = "http://17.80.224.220/api/v2/parts?serial_type=fg&serial=";
            sUrl = sUrl + $"{HSG.Trim().ToUpper()}" + "&last_log=true";

            string sMESname = OpDlg.GetInstance().m_var.GetCValue("MES_username");
            string spassword = OpDlg.GetInstance().m_var.GetCValue("MES_password");
            string procuctCodestr = "";
            WriteLog4ReUpload("查询ProductCode" + sUrl + "\r\n");
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
                        return procuctC.properties.Project_Code.ToLower();

                    }
                    else
                    {
                        return "NG";
                    }
                }
                else
                {
                    return "NG";
                }
                return procuctCodestr;
            }
            catch (Exception ex)
            {
                return "NG";
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

        private void btnOutPut_Click(object sender, EventArgs e)
        {
            string strModel = "";
            if (DialogResult.OK == MessageBox.Show($"是否导出生产模式数据，取消则导出调机模式", "提示", MessageBoxButtons.OKCancel))
            {
                strModel = "生产模式";
            }
            else
            {
                strModel = "调机模式";
            }

            DateTime strDate = DateTime.Now;

            Task.Run(async () => await OpDlg.GetInstance().m_DebugUI_UPH.ExportAsync(strDate, strModel));
        }

    }
}
