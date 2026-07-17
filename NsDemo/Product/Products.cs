
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NStructConfig;
using NsDemo.Utility;
using NVarConfig;

namespace NsDemo.Product
{
    public class BcProductInfo
    {
        public string Result { get; set; }          //该产品NG的原因
        public string RFOKFlag { get; set; }
        public string CarrySn { get; set; }
        public string HSG { get; set; }
        public string SN { get; set; }
        public string InputTime { get; set; }
        public string OutputTime { get; set; }
        public string TimeSpan { get; set; }
        public string ScanTime { get; set; }
        public string WorkTime { get; set; }
        public int SlotIndex { get; set; }
        public double NGFlag { get; set; }
        public string BeforeStationRFIDFlag { get; set; }
        public string MESResult { get; set; }
    //    public string Mesh_lot { get; set; }
        public string BeforeStationNGFlag { get; set; }
        public string OvenTime { get; set; }
        public string AssyTime { get; set; }
        public BcProductInfo()
        {

        }
    }

    public class Products
    {
        public List<BcProductInfo> _productInfos = new List<BcProductInfo>();
        private StructManage m_struct = StructManage.GetInstance();
        private VarInteface _varsReg = VarInteface.GetInstance();
        private int _input;
        private int _output;
        private int _ngOut;
        public short m_structId = 2; //RFID信息结构体


        private static Products _Instance;

        public static Products Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Products();
                    return _Instance;
                }
                else
                {
                    return _Instance;
                }
            }
        }


        private readonly LogWriter _productsWriter = new LogWriter()
        {
            IsEveryClose = true,
            BaseDirectory = @"D:\\Data\\Product\\",
            FileName = "sample",
            Encoder = Encoding.Default,
            Title = string.Empty,
            SubDirFormat = "yyyy-MM-dd",
            FileType = ".csv",
            RollingFileType = RollingFileType.EVENT_DAY_TWICE,
            IsDirectoryDate = false,
        };
        // BcProductInfo productInfo = new BcProductInfo();
        public void SetProductInfo(int SuckIndex)
        {
            _input = 0;
            _output = 0;
            _ngOut = 0;
            BcProductInfo productInfoTemp = new BcProductInfo();
            string RFOKFlag = "";
            string CarrySn = "";
            string HSG = "";
            string SN = "";
            string InputTime = "";
            string OutputTime = "";
            string TimeSpan = "";
            string ScanTime = "";
            string WorkTime = "";
            string Result = "";         //该产品NG的原因
            double ResultFlag = -1;         //结果标记
            int SlotIndex = -1;
            string BeforeStationRFIDFlag = "";         //上个RFID工站是否成功写入RFID
            string mesResult = "";         //Mes上传结果
            string bydResult = "";         //钛大比亚迪防呆结果
            string BeforeStationNGFlag = "";         //前站抛NG结果

            m_struct.GetStructCval(m_structId, 0, 4, ref RFOKFlag);
            productInfoTemp.RFOKFlag = RFOKFlag;
            m_struct.GetStructCval(m_structId, 0, 5, ref CarrySn);
            productInfoTemp.CarrySn = CarrySn;

            double GetMaterialTime = OpDlg.GetInstance().m_var.GetDValue("取料次数");
            if (GetMaterialTime == 1)
            {
                if (SuckIndex == 0)
                {
                    m_struct.GetStructCval(m_structId, 0, 1, ref SN);
                    m_struct.GetStructCval(m_structId, 0, 7, ref HSG);
                    m_struct.GetStructCval(m_structId, 0, 18, ref mesResult);
                    m_struct.GetStructCval(m_structId, 0, 22, ref bydResult);
                    m_struct.GetStructCval(m_structId, 0, 26, ref BeforeStationNGFlag);
                    SlotIndex = 2;
                }
                else if (SuckIndex == 1)
                {
                    m_struct.GetStructCval(m_structId, 0, 0, ref SN);
                    m_struct.GetStructCval(m_structId, 0, 6, ref HSG);
                    m_struct.GetStructCval(m_structId, 0, 17, ref mesResult);
                    m_struct.GetStructCval(m_structId, 0, 21, ref bydResult);
                    m_struct.GetStructCval(m_structId, 0, 25, ref BeforeStationNGFlag);
                    SlotIndex = 1;

                }
                else
                {

                }

            }
            else if (GetMaterialTime == 2)
            {
                if (SuckIndex == 0)
                {
                    m_struct.GetStructCval(m_structId, 0, 3, ref SN);
                    m_struct.GetStructCval(m_structId, 0, 9, ref HSG);
                    m_struct.GetStructCval(m_structId, 0, 20, ref mesResult);
                    m_struct.GetStructCval(m_structId, 0, 24, ref bydResult);
                    m_struct.GetStructCval(m_structId, 0, 28, ref BeforeStationNGFlag);
                    SlotIndex = 4;
                }
                else if (SuckIndex == 1)
                {
                    m_struct.GetStructCval(m_structId, 0, 2, ref SN);
                    m_struct.GetStructCval(m_structId, 0, 8, ref HSG);
                    m_struct.GetStructCval(m_structId, 0, 19, ref mesResult);
                    m_struct.GetStructCval(m_structId, 0, 23, ref bydResult);
                    m_struct.GetStructCval(m_structId, 0, 27, ref BeforeStationNGFlag);
                    SlotIndex = 3;
                }
                else
                {

                }
            }

            productInfoTemp.SN = SN;
            productInfoTemp.HSG = HSG;
            double MESFlag = OpDlg.GetInstance().m_var.GetDValue("MES启用标记");
            if (MESFlag == 0)
            {
                mesResult = "未启用";
            }
            productInfoTemp.MESResult = mesResult;

            m_struct.GetStructCval(m_structId, 0, 10, ref InputTime);
            m_struct.GetStructCval(m_structId, 0, 11, ref OutputTime);
            m_struct.GetStructCval(m_structId, 0, 12, ref TimeSpan);
            m_struct.GetStructCval(m_structId, 0, 13, ref ScanTime);
            m_struct.GetStructCval(m_structId, 0, 14, ref WorkTime);
            m_struct.GetStructDval(m_structId, 0, 15, ref ResultFlag);
            m_struct.GetStructCval(m_structId, 0, 16, ref BeforeStationRFIDFlag);

            string assyTime = "";
            string ovenTime = "";
            ;
            m_struct.GetStructCval(m_structId, 0, 29, ref assyTime);
            m_struct.GetStructCval(m_structId, 0, 30, ref ovenTime);

            productInfoTemp.TimeSpan = TimeSpan;
            productInfoTemp.BeforeStationRFIDFlag = BeforeStationRFIDFlag;
            productInfoTemp.NGFlag = ResultFlag;

            productInfoTemp.OvenTime = ovenTime;
            productInfoTemp.AssyTime = assyTime;

            double BSNGFlag = OpDlg.GetInstance().m_var.GetDValue("点胶工站抛NG启用标记");
            if (BSNGFlag == 0)
            {
                BeforeStationNGFlag = "未启用";
            }

            productInfoTemp.BeforeStationNGFlag = BeforeStationNGFlag;
            try
            {
                //DateTime dtInput = DateTime.ParseExact(InputTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                //DateTime dtOutput = DateTime.ParseExact(OutputTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                productInfoTemp.InputTime = InputTime;
                productInfoTemp.OutputTime = OutputTime;
            }
            catch (Exception ex)
            {

            }
            try
            {
                //DateTime St = DateTime.ParseExact(ScanTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                //DateTime Wt = DateTime.ParseExact(WorkTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                productInfoTemp.ScanTime = ScanTime;
                productInfoTemp.WorkTime = WorkTime;
            }
            catch (Exception ex)
            {

            }

            if (bydResult == "NG")
            {
                Result = "钛大比亚迪防呆NG";

                _input += 1;
                _ngOut += 1;
            }
            else if (BeforeStationNGFlag.Contains("NG"))
            {
                Result = BeforeStationNGFlag;

                _input += 1;
                _ngOut += 1;
            }
            else
            {
                if (ResultFlag == 0)
                {
                    if (SN != "NG")
                    {
                        if (MESFlag == 1 && mesResult != "OK")
                        {
                            Result = "MES异常抛NG";
                            _input += 1;
                            _ngOut += 1;

                        }
                        else
                        {
                            //if (mesh_lot == "NG")
                            //{
                            //    Result = "防尘网抛NG";
                            //    _input += 1;
                            //    _ngOut += 1;
                            //}


                            Result = "OK";
                            _input += 1;
                            _output += 1;

                        }
                    }
                    else
                    {
                        Result = "玻璃扫码异常抛NG";
                        _input += 1;
                        _ngOut += 1;
                    }
                }
                else if (ResultFlag == 1)
                {
                    Result = "RFID-NG";
                    _input += 1;
                    _ngOut += 1;
                }
                else if (ResultFlag == 2)
                {
                    Result = "隧道炉时间异常抛NG";
                    _input += 1;
                    _ngOut += 1;
                }
                else if (ResultFlag == 3)
                {
                    Result = "点胶工站写RFID异常抛NG";
                    _input += 1;
                    _ngOut += 1;
                }
                else if (ResultFlag == 4)
                {
                    Result = "点胶-保压时间异常抛NG";
                    _input += 1;
                    _ngOut += 1;
                }
                else
                {

                }
            }

            productInfoTemp.Result = Result;
            productInfoTemp.SlotIndex = SlotIndex;

            _productInfos.Add(productInfoTemp);

            RecordUphToUI();
        }


        public void RecordingProductsData()
        {
            if (_productInfos == null || _productInfos.Count == 0)
            {
                //    GlobalEvent.ShowMsg("没有找到待写入信息：RecordingProductsDataError", OpDlg.Level.Normal);
                return;
            }
            for (int i = 0; i < _productInfos.Count; i++)
            {
                string title = "日期,时间,时间段,穴位号,RFOKFlag,CarrySn,HsgSN,玻璃SN,进烤炉时间,出烤炉时间,烤炉用时(分钟),扫码时间,收料时间,结果,ResultFlag,点胶工站RFFlag,MES结果,点胶工站结果,组装时间,压盖板时间\n";
                string content = "";
                DateTime now = DateTime.Now;

                //日期,时间,时间段
                content += now.ToString("yyyy-MM-dd") + "," + now.ToString("HH:mm:ss") + "," + now.Hour + ",";
                //SN+结果
                content += _productInfos[i].SlotIndex + "," + _productInfos[i].RFOKFlag + "," +
                           _productInfos[i].CarrySn + "," + _productInfos[i].HSG + "," + _productInfos[i].SN + "," + _productInfos[i].InputTime + "," + _productInfos[i].OutputTime + "," + _productInfos[i].TimeSpan + "," + _productInfos[i].ScanTime + "," + _productInfos[i].WorkTime + "," + _productInfos[i].Result + "," + _productInfos[i].NGFlag + "," + _productInfos[i].BeforeStationRFIDFlag + "," + _productInfos[i].MESResult + "," + _productInfos[i].BeforeStationNGFlag + "," + _productInfos[i].AssyTime + "," + _productInfos[i].OvenTime + ",";

                string strPath = @"D:\Data\Product\";
                string strDate = now.ToString("yyyy-MM-dd");
                double Model = _varsReg.GetDValue("模式选择Flg");
                string strModel = "";
                if (Model == 0)
                {
                    strModel = "调机模式";
                }
                else
                {
                    strModel = "生产模式";
                }

                if (_productsWriter.RollingFileType == RollingFileType.EVENT_DAY_TWICE && now.Hour >= 0 && now.Hour < 8)
                {
                    strDate = now.AddDays(-1).ToString("yyyy-MM-dd");
                }
                _productsWriter.BaseDirectory = strPath + strDate + "\\" + strModel + "\\";
                _productsWriter.Title = title;
                _productsWriter.FileName = now.ToString("yyyy-MM-dd");
                _productsWriter.WriteLine(content);
            }
        }

        public void RecordUphToUI()
        {
            double input = _varsReg.GetDValue("投入");
            input += _input;
            _varsReg.SetDValue("投入", input);

            double output = _varsReg.GetDValue("产出");
            output += _output;
            _varsReg.SetDValue("产出", output);

            double ngput = _varsReg.GetDValue("次品");
            ngput += _ngOut;
            _varsReg.SetDValue("次品", ngput);
        }

        public void ProductInfosClear()
        {
            _productInfos.Clear();
        }


    }
}
