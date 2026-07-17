using Newtonsoft.Json;
using NsDemo.Product;
using NVarConfig;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace NsDemo.MESSys
{

    public class bc_window_assy_mesinfo
    {
        public Data data { get; set; }
        public Serials serials { get; set; }

        public string Serialize2Json()
        {
            string json = JsonConvert.SerializeObject(this);

            string namepath = string.Format(@"D:\Log_{0}.txt", 1);

            try
            {
                using (StreamWriter writer = new StreamWriter(namepath))
                {
                    writer.WriteLine(json);
                }

                Console.WriteLine("文本文件写入成功。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入文本文件时发生错误：{ex.Message}");
            }


            return json;
        }
    }

    public class Data
    {
        public Insight insight { get; set; }
    }

    public class Insight
    {
        public Result[] results { get; set; }
        public Test_Attributes test_attributes { get; set; }
        public Test_Station_Attributes test_station_attributes { get; set; }
        public Uut_Attributes uut_attributes { get; set; }
    }

    public class Test_Attributes
    {
        public string test_result { get; set; }
        public string unit_serial_number { get; set; }
        public string uut_start { get; set; }
        public string uut_stop { get; set; }
    }

    public class Test_Station_Attributes
    {
        public string line_id { get; set; }
        public string software_name { get; set; }
        public string software_version { get; set; }
        public string station_id { get; set; }
    }

    public class Uut_Attributes
    {
        public string bc_window_assy_start_time { get; set; }
        public string bc_window_sn { get; set; }
        public string cavity_id { get; set; }
        public string fixture_id { get; set; }
        public string glue_dispense_start_time { get; set; }
        public string mesh_lot { get; set; }
        public string mesh_station_vendor { get; set; }
        public string oven_assy_time { get; set; }
        public string oven_start_time { get; set; }
        public string oven_stop_time { get; set; }
        public string plasma_start_time { get; set; }
        public string plasma_stop_time { get; set; }
        public string prima_expiry_date { get; set; }
        public string prima_lot { get; set; }
        public string prima_start_time { get; set; }
        public string prima_stop_time { get; set; }
        public string prima_vendor { get; set; }
        public string standing_start_time { get; set; }
        public string standing_stop_time { get; set; }
        public string station_vendor { get; set; }
    }

    public class Result
    {
        public string test { get; set; }
        public string units { get; set; }
        public string value { get; set; }
        public string result { get; set; }
        public string sub_test { get; set; }
        public string lower_limit { get; set; }
        public string upper_limit { get; set; }
    }

    public class Serials
    {
        public string fg { get; set; }
    }


}
