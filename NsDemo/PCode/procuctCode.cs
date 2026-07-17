using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsDemo.PCode
{
    public class procuctCode
    {
        public int age { get; set; }
        public string project { get; set; }
        public string component { get; set; }
        public DateTime created { get; set; }
        public History[] history { get; set; }
        public Serials serials { get; set; }
        public Properties properties { get; set; }
    }

    public class Serials
    {
        public string fg { get; set; }
        public string wip_id { get; set; }
    }

    public class Properties
    {
        public string DOE { get; set; }
        public string Size { get; set; }
        public string Build { get; set; }
        public string Color { get; set; }
        public string Config1 { get; set; }
        public string Config2 { get; set; }
        public string Surface { get; set; }
        public string Project_Code { get; set; }
        public string Process_Config { get; set; }
        public string Raw_material_type { get; set; }
    }

    public class History
    {
        public string id { get; set; }
        public Data data { get; set; }
        public string _event { get; set; }
        public DateTime created { get; set; }
        public Serials1 serials { get; set; }
        public Defects defects { get; set; }
        public string agent_id { get; set; }
        public string process_id { get; set; }
        public string component_id { get; set; }
    }

    public class Data
    {
        public Insight insight { get; set; }
        public Properties1 properties { get; set; }
    }

    public class Insight
    {
        public Uut_Attributes uut_attributes { get; set; }
        public Test_Attributes test_attributes { get; set; }
        public Test_Station_Attributes test_station_attributes { get; set; }
    }

    public class Uut_Attributes
    {
        public string station_vendor { get; set; }
    }

    public class Test_Attributes
    {
        public string uut_stop { get; set; }
        public string uut_start { get; set; }
        public string test_result { get; set; }
    }

    public class Test_Station_Attributes
    {
        public string line_id { get; set; }
        public string station_id { get; set; }
        public string software_name { get; set; }
        public string software_version { get; set; }
    }

    public class Properties1
    {
        public string Config2 { get; set; }
    }

    public class Serials1
    {
        public string wip_id { get; set; }
    }

    public class Defects
    {
    }

}
