using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Newtonsoft.Json;

namespace NsDemo.Comm_Ctrl
{
    public class CSVInfo : ICloneable
    {
        public List<string> FileTitle { get; set; }
        public List<string> FileData { get; set; }
        public string FileName { get; set; }

        public object Clone()
        {
            return MemberwiseClone() as CSVInfo;
        }

        public string GetFileData()
        {
            string rtn = "";
            for (int i = 0; i < FileData.Count; i++)
            {
                if (FileData[i] == "回车换行")
                {
                    rtn += "\r\n";
                }
                else
                {
                    string strCurrentData = FileData[i].Replace("\r", "");
                    strCurrentData = strCurrentData.Replace("\n", "");
                    rtn += strCurrentData + ",";
                }
            }
            return rtn;
        }

        public string GetTitleData()
        {
            string rtn = "";
            for (int i = 0; i < FileTitle.Count; i++)
            {
                if (FileTitle[i] == "回车换行")
                {
                    rtn += "\r\n";
                }
                else
                {
                    string strCurrentData = FileTitle[i].Replace("\r", "");
                    strCurrentData = strCurrentData.Replace("\n", "");
                    rtn += strCurrentData + ",";
                }
            }
            return rtn;
        }
    }

    public class CSV_Fun
    {
        public CSVInfo _csvbaseInfo;
        public RegisterInfo _RegInfo = new RegisterInfo();
        public string _EntityPath;
        public string _RegPath;

        private CSVInfo SerializeToVariable()
        {
            try
            {
                var jsonAllText = File.ReadAllText(_EntityPath);
                return JsonConvert.DeserializeObject<CSVInfo>(jsonAllText);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private string ConvertData(string BaseData)
        {
            if (BaseData == null)
            {
                return "";
            }
            _RegInfo.UpDateData_Regiter(true);
            for (int i = 0; i < _RegInfo.List_Register.Count; i++)
            {
                BaseData = BaseData.Replace(_RegInfo.List_Register[i].Name, _RegInfo.List_Register[i].Data);
            }
            BaseData = BaseData.Replace("系统日期", DateTime.Now.ToString("yyyy-MM-dd"));
            BaseData = BaseData.Replace("系统时间", DateTime.Now.ToString("HH:mm:ss"));
            BaseData = BaseData.Replace("文件时间", DateTime.Now.ToString("HH-mm-ss"));
            return BaseData;
        }

        public bool SerializeToFile()
        {
            try
            {
                using (var file = File.CreateText(_EntityPath))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, _csvbaseInfo);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public CSV_Fun(string entityPath, string Reg_Path)
        {
            string strPath = Path.Combine(System.Windows.Forms.Application.StartupPath);
            _EntityPath = strPath + entityPath;
            _RegPath = Reg_Path;
            _csvbaseInfo = SerializeToVariable();
            _RegInfo.EntityPath = _RegPath;
        }

        public bool Save(string strData)
        {
            if (_csvbaseInfo == null)
            {
                return false;
            }

            strData += "\r\n";
            string strFilePath = ConvertData(_csvbaseInfo.FileName);
            try
            {
                if (!File.Exists(strFilePath))
                {
                    string Title = "";
                    Title = _csvbaseInfo.GetTitleData();
                    Title += "\r\n";
                    strData = Title + strData;
                }
                string strPath = strFilePath.Substring(0, strFilePath.LastIndexOf("\\"));
                Directory.CreateDirectory(strPath);
                StreamWriter sw = new StreamWriter(strFilePath, true, Encoding.UTF8);
                sw.Write(strData);
                sw.Flush();
                sw.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool Save()
        {
            if (_csvbaseInfo == null)
            {
                return false;
            }

            string strData = ConvertData(_csvbaseInfo.GetFileData());
            strData += "\r\n";
            string strFilePath = ConvertData(_csvbaseInfo.FileName);
            try
            {
                if (!File.Exists(strFilePath))
                {
                    string Title = "";
                    Title = _csvbaseInfo.GetTitleData();
                    Title += "\r\n";
                    strData = Title + strData;
                }
                string strPath = strFilePath.Substring(0, strFilePath.LastIndexOf("\\"));
                Directory.CreateDirectory(strPath);
                StreamWriter sw = new StreamWriter(strFilePath, true, Encoding.UTF8);
                sw.Write(strData);
                sw.Flush();
                sw.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}