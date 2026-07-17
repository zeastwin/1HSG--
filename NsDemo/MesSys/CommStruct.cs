using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Policy;
using NsDemo.Product;
using NVarConfig;

namespace NsDemo.Comm_Ctrl
{
    public class CommStruct
    {
    }

    #region 寄存器的数据结构

    public class RegisterItem : ICloneable
    {
        public string Name { get; set; }
        public string Data { get; set; }

        public string note { get; set; }

        public object Clone()
        {
            return MemberwiseClone() as RegisterItem;
        }
    }

    #endregion 寄存器的数据结构

    #region 寄存器列表信息

    public class RegisterInfo : ICloneable
    {
        private VarInteface _varInteface = VarInteface.GetInstance();  //寄存器操作句柄
        private List<RegisterItem> _List_Register { get; set; }//数据结构列表信息

        private List<RegisterItem> _List_ProductValue { get; set; }//数据结构列表信息
        public string EntityPath { get; set; }//文件路径

        public List<RegisterItem> List_Register
        {
            get
            {
                if (_List_Register == null)
                {
                    _List_Register = new List<RegisterItem>();
                    Read_regInfo();
                }
                return _List_Register;
            }
        }

        //public List<RegisterItem> List_ProductValue
        //{
        //    get
        //    {
        //        if (_List_Register == null)
        //        {
        //            _List_Register = new List<RegisterItem>();
        //            Read_productClassInfo();
        //        }
        //        return _List_ProductValue;
        //    }
        //}


        //public void UpdateData_Product()
        //{

        //    if (_List_ProductValue == null)
        //    {
        //        _List_ProductValue = new List<RegisterItem>();
               
        //    }
        //    Read_productClassInfo();
        //}

        public bool UpDateData_Regiter(bool UpDateFlag)
        {
            if (_List_Register == null)
            {
                _List_Register = new List<RegisterItem>();
                Read_regInfo();

            }

            if (UpDateFlag)
            {
                for (int i = 0; i < _List_Register.Count; i++)
                {
                    string strData;
                    VarInfo info_v = _varInteface.GetVarInfo(_List_Register[i].Name);
                    if (info_v != null)
                    {
                        if (info_v.varType == 1)
                        {
                            strData = _varInteface.GetDValue(_List_Register[i].Name).ToString();
                        }
                        else
                        {
                            strData = _varInteface.GetCValue(_List_Register[i].Name).ToString();
                        }
                        _List_Register[i].Data = strData;
                        _List_Register[i].note = info_v.desc;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _List_Register.Count; i++)
                {
                    string strData;
                    VarInfo info_v = _varInteface.GetVarInfo(_List_Register[i].Name);
                    if (info_v != null)
                    {
                        if (info_v.varType == 1)
                        {
                            try
                            {
                                double ff = double.Parse(_List_Register[i].Data);
                                _varInteface.SetDValue(_List_Register[i].Name, ff);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        else
                        {
                            _varInteface.SetCValue(_List_Register[i].Name, _List_Register[i].Data);
                        }
                    }
                }
            }

            return true;
        }

        public object Clone()
        {
            return MemberwiseClone() as RegisterInfo;
        }

        public void Read_regInfo() //读取寄存器数据细信息
        {
            string strPath = Path.Combine(System.Windows.Forms.Application.StartupPath);
            string strPathachineStatus = strPath + EntityPath;
            _List_Register.Clear();
            try
            {
                FileStream fs = new FileStream(strPathachineStatus, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                List<string> lines = new List<string>();
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    analyzeData(line);
                }

                sr.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Read_regInfo  " + ex.Message);
            }

            return;
        }

        //0320新增
        public void Read_productClassInfo()
        {
            try
            {
                int slotIndex = (int)_varInteface.GetDValue("当前复检索引");
                _List_ProductValue.Clear();
                if (slotIndex > 3)
                {
                    return;
                }
                
                //1.码
                BcProductInfo bcProductInfo = new BcProductInfo();
                bcProductInfo = Products.Instance._productInfos[slotIndex];
                foreach (PropertyInfo info in bcProductInfo.GetType().GetProperties())
                {
                    object value = info.GetValue(bcProductInfo, null);
                    string name = info.Name;    //获取BcProductInfo类中所有的属性名
                    Type type = info.PropertyType;
                    if (type == typeof(string) || type == typeof(int) || type == typeof(double))
                    {
                        RegisterItem Current_Reg = new RegisterItem();
                        Current_Reg.Name = name;
                        Current_Reg.Data = value.ToString();
                        Current_Reg.note = "";
                        _List_ProductValue.Add(Current_Reg);
                        continue;
                    }
                    else if (type == typeof(double[]))
                    {
                        int index = 0;
                        foreach (var itor in (Array)value)
                        {
                            RegisterItem Current_Reg = new RegisterItem();
                            Current_Reg.Name = name + "[" + index + "]";
                            Current_Reg.Data = itor.ToString();
                            Current_Reg.note = "";
                            _List_ProductValue.Add(Current_Reg);
                            index++;
                        }
                        continue;
                    }
                    PropertyInfo[] properties = type.GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        Type type1 = property.PropertyType;
                        if (type1 == typeof(double[]))
                        {
                            int index = 0;
                            foreach (var itor in (Array)(property.GetValue(value, null)))
                            {
                                RegisterItem Current_Reg = new RegisterItem();
                                Current_Reg.Name = property.Name + "[" + index + "]";
                                Current_Reg.Data = itor.ToString();
                                Current_Reg.note = "";
                                _List_ProductValue.Add(Current_Reg);
                                index++;

                            }
                        }
                        else
                        {
                            RegisterItem Current_Reg = new RegisterItem();
                            Current_Reg.Name = property.Name;
                            Current_Reg.Data = property.GetValue(value, null).ToString();
                            Current_Reg.note = "";
                            _List_ProductValue.Add(Current_Reg);
                        }
                       
                    }

                }

            }
            catch
            {

            }


        }

        private void analyzeData(string line) //解析寄存器的数据类型
        {
            string[] vec_Data = line.Split(new[] { "," }, StringSplitOptions.None);
            if (vec_Data.Count() < 1)
            {
                return;
            }
            RegisterItem Current_Reg = new RegisterItem();
            Current_Reg.Name = vec_Data[0];
            Current_Reg.Data = "";
            Current_Reg.note = "";
            _List_Register.Add(Current_Reg);
        }
    }

    #endregion 寄存器列表信息
}