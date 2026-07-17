using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NsDemo.Comm_Ctrl;
using NsDemo.Utility;
using System.Net.Http;
using System.Net;

namespace NsDemo.MESSys
{
    public class MES_Fun
    {
        #region 私有成员

        private static MES_Fun _Instance;
        private Object _Lock_UpdateData = new Object(); //数据更新锁
        private Object _WriteLock_Log = new Object(); //Log锁

        #endregion 私有成员

        public RegisterInfo _RegInfo = new RegisterInfo();
        public BaseInfo _BaseInfo = new BaseInfo();

        public static MES_Fun Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MES_Fun();
                    _Instance.Init();
                    return _Instance;
                }
                else
                {
                    return _Instance;
                }
            }
        }

        private void Init()
        {
            _RegInfo.EntityPath = "\\MachineConfig\\MESConfig\\Regiter.csv";
            _BaseInfo.EntityPath = "\\MachineConfig\\MESConfig";
        }

        private string LogInfo(string strBuff, string strPathName)  //记录Log信息
        {
            lock (_WriteLock_Log)
            {
                try
                {
                    string strPath = strPathName.Substring(0, strPathName.LastIndexOf("\\"));
                    Directory.CreateDirectory(strPath);
                    StreamWriter sw = new StreamWriter(strPathName, true, Encoding.UTF8);
                    sw.Write(strBuff);
                    sw.Flush();
                    sw.Close();
                    return "OK";
                }
                catch (System.Exception ex)
                {
                    return "NG->" + ex.Message;
                }
            }
        }

       

        public string ConvertSendData(string BaseData, bool UpDateData = true)
        {
            if (BaseData == null)
            {
                return "";
            }
            lock (_Lock_UpdateData)
            {
                if (UpDateData)
                {
                    _RegInfo.UpDateData_Regiter(UpDateData);
                   
                }
                for (int i = 0; i < _RegInfo.List_Register.Count; i++)
                {
                    BaseData = BaseData.Replace(_RegInfo.List_Register[i].Name, _RegInfo.List_Register[i].Data);
                    
                }


                BaseData = BaseData.Replace("系统日期", DateTime.Now.ToString("yyyy-MM-dd"));
                BaseData = BaseData.Replace("系统时间", DateTime.Now.ToString("HH:mm:ss"));
                BaseData = BaseData.Replace("文件时间", DateTime.Now.ToString("HH-mm-ss"));
            }
            return BaseData;
        }

        public string Http_Send(string url, string strData, string LogPath, string HttpCommitType, int Timeout = 3)
        {
            string strLog = "";

            try
            {
                var converData = new StringContent(strData, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    string strResult = "";
                    if (HttpCommitType == "GET")
                    {
                        /**********************************发送的Log*********************************/
                        strLog = "发送：\r\n" + System.DateTime.Now.ToString() + "\r\n ";
                        strLog += "URL: " + url + "\r\n";

                        /***********************************发送代码*********************************/
                        var jsonTokens = JObject.Parse(strData);
                        var content = new StringBuilder();
                        foreach (var jsonToken in jsonTokens)
                        {
                            content.Append(jsonToken.Key + "=" + jsonToken.Value + "&");
                        }
                        var getContent = content.ToString().TrimEnd('&');
                        var sendContent = string.Format("{0}?{1}", url, getContent);
       
                        var response = client.GetAsync(sendContent).Result;
                        strResult = response.Content.ReadAsStringAsync().Result;

                        /**********************************发送的Log*********************************/
                        strLog += sendContent + "\r\n";
                    }
                    else
                    {
                        /**********************************发送的Log*********************************/
                        strLog = "发送：\r\n" + System.DateTime.Now.ToString() + "\r\n ";
                        strLog += "URL: " + url + "\r\n";

                        /***********************************发送代码*********************************/
                        client.Timeout = TimeSpan.FromSeconds(Timeout);
                        var response = client.PostAsync(url, converData).Result;
                        strResult = response.Content.ReadAsStringAsync().Result;

                        /**********************************发送的Log*********************************/
                        strLog += converData + "\r\n";
                    }

                    /************************************接收的Log******************************/
                    strLog += "接收：\r\n" + System.DateTime.Now.ToString() + "\r\n ";
                    strLog += strResult + "\r\n ";

                    LogInfo(strLog, LogPath);
                    return strResult;
                }
            }
            catch (Exception ex)
            {
                strLog += ex.Message;
                LogInfo(strLog, LogPath);
                return "NG" + ex.Message;
            }
            //try
            //{
            //    var request = (HttpWebRequest)WebRequest.Create(url);
            //    var data = Encoding.UTF8.GetBytes(strData);


            //    request.Method = "POST"; 
            //    request.ContentType = "application/json";
            //    request.ContentLength = data.Length;
            //    request.KeepAlive = false;

            //    using (var stream = request.GetRequestStream())
            //    {
            //        stream.Write(data, 0, data.Length);
            //    }

            //    var response = (HttpWebResponse)request.GetResponse();
            //    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            //    return responseString;
            //}

            //catch (Exception ex)
            //{
            //    return "NG"+ex.Message;
            //}

        }

        #region 外部接口

        public string API_MES_Action(string ActionCode,string strData = "")
        {

            for (int i = 0; i < _BaseInfo.List_Info.Count(); i++)
            {
                if (_BaseInfo.List_Info[i].strName == ActionCode)
                {
                    
                    string strPath = ConvertSendData(_BaseInfo.List_Info[i].strLogPathName);

                    if (_BaseInfo.List_Info[i].strcsvPathName != "" && _BaseInfo.List_Info[i].strcsvPathName != "none")
                    {
                        CSV_Fun _csv_Fun = new CSV_Fun(_BaseInfo.List_Info[i].strcsvPathName, _RegInfo.EntityPath);
                        _csv_Fun.Save();
                    }
                    return Http_Send(_BaseInfo.List_Info[i].strURL, strData, strPath, _BaseInfo.List_Info[i].strHttpCommitType);
                }
            }
            return "NG ActionCode Error";
        }

        #endregion 外部接口
    }


    public class MesTestData
    {
        public string result { get; set; }
        public string test { get; set; }
        public string units { get; set; }
        public string value { get; set; }
        public string upper_limit { get; set; }
        public string lower_limit { get; set; }

        public MesTestData() 
        {
            result = "pass";
            test = "";
            units = "";
            value = "";
            upper_limit = "";
            lower_limit = "";
        }
    }

    public class JsonResults
    {
        static public Dictionary<string, MesTestData> SerializeToVariable(string strJsonData)
        {
            try
            {
                
                return JsonConvert.DeserializeObject<Dictionary<string, MesTestData>>(strJsonData);
            }
            catch (Exception e)
            {
                return new Dictionary<string, MesTestData>();
            }
        }

        static public string Deserialization(Dictionary<string,MesTestData> JsonData)
        {
            try
            {
                return JsonConvert.SerializeObject(JsonData);
            }
            catch (Exception e) 
            {
                return e.Message;
            }
        }

      
    }

}