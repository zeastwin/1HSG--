using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;

namespace NsDemo.Comm_Ctrl
{
    public class Translation_Message
    {
        Dictionary<string, List<string>> _Message_Page;
        private static Translation_Message _Instance;
        private int[] Language_Select;
        public static Translation_Message Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Translation_Message();
                    _Instance.Loadinfo();
                    return _Instance;
                }
                else
                {
                    return _Instance;
                }
            }
        }

        void Loadinfo()
        {
            string strPath = Path.Combine(System.Windows.Forms.Application.StartupPath);
            strPath += "\\MachineConfig\\MessageBox\\Message.csv";
            _Message_Page = new Dictionary<string, List<string>>();
            _Message_Page.Clear();
            try
            {
                FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                List<string> lines = new List<string>();
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    
                    try
                    {
                        if (line.IndexOf("基础信息记录") >= 0)
                        {
                            string[] vec_Data = line.Split(new[] { "\t" }, StringSplitOptions.None);
                            string[] Select_Index = vec_Data[0].Split(new[] { "<" , ">" , "&" }, StringSplitOptions.RemoveEmptyEntries);
                            if (Select_Index.Length > 1)
                            {
                                Language_Select = new int[Select_Index.Length - 1];
                                for (int i = 1; i < Select_Index.Length; i++)
                                {
                                    Language_Select[i - 1] = Convert.ToInt32(Select_Index[i]);
                                }
                            }
                            continue;
                        }
                        else
                        {
                            string[] vec_Data = line.Split(new[] { "\t" }, StringSplitOptions.None);
                            if(vec_Data.Length > 1)
                            {
                                if (!_Message_Page.ContainsKey(vec_Data[0]))
                                {
                                    List<string> list = new List<string>();
                                    for(int i = 1;i< vec_Data.Length;i++)
                                    {
                                        list.Add(vec_Data[i]);
                                    }
                                    _Message_Page.Add(vec_Data[0], list);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("" + ex.Message);
                    }

                }

                sr.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("" + ex.Message);
            }
        }


        public string Format(string strMsg, params object[] obj)
        {
            string str_Base = "";
            try
            {
                str_Base = string.Format(strMsg, obj);
                if (_Message_Page.ContainsKey(strMsg))
                {
                    for (int i = 0; i < Language_Select.Length; i++)
                    {
                        if (Language_Select[i] < _Message_Page[strMsg].Count && Language_Select[i] >= 0)
                        {
                            string strItem = string.Format(_Message_Page[strMsg][Language_Select[i]], obj);
                            str_Base += "\r\n" + strItem;
                        }
                    }
                }
                else
                {
                    str_Base += "\r\n" + "Message.csv 表中无此翻译";
                }
            }
            catch(Exception ex)
            { 
                return ex.Message;
            }
            return str_Base;
        }

        public void MessageShow(string strMsg, params object[] obj)
        {
            string str = Format(strMsg, obj);
            MessageBox.Show(str);
        }
    }
}
