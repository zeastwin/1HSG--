using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsDemo.MesSys
{
    public class mesh_lot
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
        public Result[] results { get; set; }
    }

    public class Uut_Attributes
    {
        public string station_vendor { get; set; }
        public string fly_bar_id { get; set; }
        public string fly_bar_locX { get; set; }
        public string fly_bar_locY { get; set; }
        public string fly_bar_locZ { get; set; }
        public string cavity_id { get; set; }
        public string resin_lot { get; set; }
        public string tooling_no { get; set; }
        public string epoxy_lot { get; set; }
        public string fixture_id { get; set; }
        public string epoxy_pressure1 { get; set; }
        public string epoxy_pressure2 { get; set; }
        public string degas_machine_id { get; set; }
        public string fixture_using_times { get; set; }
        public string cnc1_day { get; set; }
        public string cnc3_day { get; set; }
        public string cnc4_day { get; set; }
        public string cnc1_time { get; set; }
        public string cnc1_year { get; set; }
        public string cnc1_month { get; set; }
        public string cnc3_month { get; set; }
        public string cnc1_machine { get; set; }
        public string cnc3_machine { get; set; }
        public string cnc4_machine { get; set; }
        public string interposer_lot { get; set; }
        public string temperature { get; set; }
        public string mesh_lot { get; set; }
        public string fg_sn { get; set; }
        public string bc_window_sn { get; set; }
        public string mesh_station_vendor { get; set; }
        public string glue_dispense_start_time { get; set; }
        public string bc_window_assy_start_time { get; set; }
        public string size { get; set; }
        public string config { get; set; }
        public string material { get; set; }
        public string BC_HSG_DCR_Bin { get; set; }
        public string box_sn { get; set; }
        public string fatp_name { get; set; }
    }

    public class Test_Attributes
    {
        public string uut_stop { get; set; }
        public string uut_start { get; set; }
        public string test_result { get; set; }
        public string unit_serial_number { get; set; }
    }

    public class Test_Station_Attributes
    {
        public string line_id { get; set; }
        public string station_id { get; set; }
        public string software_name { get; set; }
        public string software_version { get; set; }
    }

    public class Result
    {
        public string test { get; set; }
        public string units { get; set; }
        public string value { get; set; }
        public string result { get; set; }
        public string lower_limit { get; set; }
        public string upper_limit { get; set; }
    }

    public class Properties1
    {
        public string DOE { get; set; }
        public string Size { get; set; }
        public string Build { get; set; }
        public string Project_Code { get; set; }
        public string Raw_material_type { get; set; }
        public string Process_Config { get; set; }
        public string Config1 { get; set; }
        public string Color { get; set; }
        public string Surface { get; set; }
        public string Config2 { get; set; }
    }

    public class Serials1
    {
        public string wip_id { get; set; }
        public string fg { get; set; }
    }

    public class Defects
    {
    }

}
