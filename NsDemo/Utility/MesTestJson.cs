using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using NsDemo.Utility;
using System.Drawing;
using System.IO;


namespace NsDemo.Utility
{


    public class MesTestJsonInfo
    {
        public int SeqNo { get; set; }
        public string TestName { get; set; }
        public double TestValue { get; set; }
        public string TestResult { get; set; }
        public double StandardValue { get; set; }
        public double UpperLimit { get; set; }
        public double LowerLimit { get; set; }
        public MesTestJsonInfo()
        {
            SeqNo = 0;
            TestName = "NULL";
            TestValue = 0;
            TestResult = "NULL";
            StandardValue = 0;
            UpperLimit = 0;
            LowerLimit = 0;
        }
    }


    public class MesBakeTestJsonInfo
    {
        public int SeqNo { get; set; }
        public string Bar2DCode { get; set; }
        public MesBakeTestJsonInfo()
        {
            SeqNo = 0;
            Bar2DCode = "";
        }
    }



    public class MesTestJson
    {
        private static MesTestJson _Instance;

        public static MesTestJson Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MesTestJson();
                    return _Instance;
                }
                else
                {
                    return _Instance;
                }
            }
        }


       
        public Dictionary<string, string> Deserialize(string strJsonData)
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(strJsonData);
            }
            catch (Exception e)
            {
                return new Dictionary<string, string>();
            }

        }

        public string SerializeToString(Dictionary<string, string> obj)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(obj);
                return jsonData;
            }
            catch (Exception e)
            {
                //
                return null;
            }
        }

 
    }
}
