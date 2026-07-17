using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NsDemo.Comm_Ctrl
{
    public class BaseInfo : ICloneable
    {
        private List<BaseItem> _List_Info { get; set; }
        public string EntityPath;

        public object Clone()
        {
            return MemberwiseClone() as BaseInfo;
        }

        public List<BaseItem> List_Info
        {
            get
            {
                if (_List_Info == null)
                {
                    _List_Info = new List<BaseItem>();
                    ReadBaseInfo();
                }
                return _List_Info;
            }
        }

        public void ReadBaseInfo()
        {
            string strPath = Path.Combine(System.Windows.Forms.Application.StartupPath);
            string strPathachineStatus = strPath + "\\" + EntityPath + "\\BaseInfo.csv";
            _List_Info.Clear();
            try
            {
                FileStream fs = new FileStream(strPathachineStatus, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                List<string> lines = new List<string>();
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.IndexOf("Josn文件名称") >= 0)
                    {
                        continue;
                    }
                    analyzeUploadData(line);
                }

                sr.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Init  " + ex.Message);
            }

            return;
        }

        private void analyzeUploadData(string line)
        {
            string[] vec_Data = line.Split(new[] { "," }, StringSplitOptions.None);
            if (vec_Data.Count() < 6)
            {
                return;
            }
            BaseItem Current_Upload = new BaseItem();
            Current_Upload.strName = vec_Data[0];
            Current_Upload.strURL = vec_Data[1];
            Current_Upload.strLogPathName = vec_Data[2];
            Current_Upload.strJosnFileName = vec_Data[3];
            Current_Upload.strHttpCommitType = vec_Data[4];
            Current_Upload.strcsvPathName = vec_Data[5];
            Current_Upload.strJosnData = ReadJsonData(Current_Upload.strJosnFileName);
            _List_Info.Add(Current_Upload);
        }

        public string ReadJsonData(string Name) //读取发送的标本
        {
            string strPath = Path.Combine(System.Windows.Forms.Application.StartupPath);
            string strPathachineStatus = strPath + "\\" + EntityPath + "\\" + Name;
            string line = "";
            try
            {
                FileStream fs = new FileStream(strPathachineStatus, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
                List<string> lines = new List<string>();

                line = sr.ReadToEnd();
                sr.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ReadJsonData  " + ex.Message);
            }

            return line;
        }
    }

    public class BaseItem : ICloneable
    {
        public string strURL;
        public string strLogPathName;
        public string strJosnFileName;
        public string strJosnData;
        public string strName;
        public string strHttpCommitType;
        public string strcsvPathName;

        public object Clone()
        {
            return MemberwiseClone() as BaseItem;
        }
    }
}