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


    public class ColorOffsetItem
    {
        public double[,] _offset { get; set; }
        public string _describe { get; set; }
        public string _CCD_Cmd { get; set; }
        public ColorOffsetItem()
        {
            _offset = new double[4, 3];
            for (int i = 0; i < 4; i++)
            {
                _offset[i, 0] = 0;
                _offset[i, 1] = 0;
                _offset[i, 2] = 0;
            }
            _describe = "";
            _CCD_Cmd = "";
        }
    }

    
   

    public class AnalyzeJson
    {
        private static AnalyzeJson _Instance;
        public string EntityPath { get; set; }

        private Dictionary<string, ColorOffsetItem> _Dic_ColorOffset = null;
        public static AnalyzeJson Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new AnalyzeJson();
                    return _Instance;
                }
                else
                {
                    return _Instance;
                }
            }
        }


        public Dictionary<string, ColorOffsetItem> Dic_ColorOffset
        {
            get
            {
                if (_Dic_ColorOffset == null)
                {
                    _Dic_ColorOffset = SerializeToVariable();
                }
                return _Dic_ColorOffset;
            }

            set
            {
                if (value != null)
                {
                    if (_Dic_ColorOffset == null)
                    {
                        _Dic_ColorOffset = new Dictionary<string, ColorOffsetItem>();
                    }

                    _Dic_ColorOffset = value;
                }
            }

        }

         public Dictionary<string, ColorOffsetItem> SerializeToVariable()
         {
            try
            {
                var jsonAllText = File.ReadAllText(EntityPath + "AssemblyOffset.json");
                return JsonConvert.DeserializeObject<Dictionary<string, ColorOffsetItem>>(jsonAllText);
            }
            catch (Exception e)
            {
                return new Dictionary<string, ColorOffsetItem>();
            }
           
        }

        public bool SerializeToFile(Dictionary<string, ColorOffsetItem> obj)
        {
            try
            {
                using (var file = File.CreateText(EntityPath + "AssemblyOffset.json"))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, obj);
                }

                return true;
            }
            catch (Exception e)
            {
                //
                return false;
            }
        }
    }
}
