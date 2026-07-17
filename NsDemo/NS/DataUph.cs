using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NStructConfig;
using NVarConfig;
using System.IO;


namespace NsDemo
{
    public partial class DataUph : Form
    {
        private CSV m_csv;
        private Uph_Data[] m_stUphData = new Uph_Data[24];
        private StructManage m_structManer = StructManage.GetInstance();
        private VarInteface m_reg = VarInteface.GetInstance();
        private long m_total = 0;
        private long m_NGtotal = 0;

        public DataUph()
        {
			InitializeComponent();
			CoreWork.Instance.SendDatas += OnGetData;   //事件
            InitUph();
            for (int i = 0; i < m_stUphData.Count(); i++)
            {
                m_stUphData[i] = new Uph_Data();
            }
        }

        private void InitUph()
        {
            chartUph.Series[0].Points.AddXY(0, 0);   //添加点,使其显示出来
            chartPie.Series[0].Points.AddXY(0, 0);
            labelOK.Visible = false;
            labelNg.Visible = false;
        }

		private int OnGetData(string name, object obj)
		{
			if (name.Equals("DataUph.SaveYieldData"))
			{
				if (this.IsHandleCreated)
				{
					SaveYieldData();
				}
			}
            return 0;
		}

        public void UpdateUph()
        {
            string str, path;
            int total = 0, NGtotal = 0;
            SaveYieldData();
            double structID = m_reg.GetDValue("产量数据") > 0 ? m_reg.GetDValue("产量数据") : 0;
            str = string.Format("{0:yyyyMMdd}", dateTimePicker_Uph.Value);
            //str = "20220809";
            path = string.Format("D:\\Data\\Yield\\{0}.csv", str);
            if (File.Exists(path))
            {
                LoadYieldDate(str);
            }
            else
            {
                for (int i = 0; i < m_stUphData.Count(); i++)
                {
                    m_stUphData[i] = new Uph_Data();
                }
                MessageBox.Show("无记录!!!");

            }
            DrawChartUph(m_stUphData);

            for (int i = 0; i < 24; i++)
            {
                total += m_stUphData[i].total;
                NGtotal += m_stUphData[i].tossing;
            }
            m_total = total;
            m_NGtotal = NGtotal;
            DrawPieBingTu();
            labelProduction.Text = string.Format("{0} PCS", total + NGtotal);
            labelDayNG.Text = string.Format("{0} PCS", NGtotal);
        }

        private void DrawChartUph(Uph_Data[] chartData)//绘制柱状曲线图
        {
            int OktotalVal = 0, total = 0;
            double yield;

            for (int n = 0; n < chartUph.Series.Count(); n++)
            {
                chartUph.Series[n].Points.Clear();
            }

            for (int i = 0; i < chartData.Count(); i++)
            {
                OktotalVal = chartData[i].total;
                total = OktotalVal + chartData[i].tossing;
                chartUph.Series[0].Points.AddXY(i + 1, total); //产量总数

                yield = chartData[i].yield;
                chartUph.Series[1].Points.AddXY(i + 1, yield); //良率
                chartUph.Series[2].Points.AddXY(i + 1, OktotalVal); //ok总数

                chartUph.Series[0].Points[i].Label = string.Format("{0}", chartData[i].tossing);//显示NG标签,OK与总数叠加后剩余是NG部分
                chartUph.Series[1].Points[i].Label = string.Format("{0}%", chartUph.Series[1].Points[i].YValues[0]);//折线绘制百分比标签
                chartUph.Series[2].Points[i].Label = string.Format("{0}", total);//显示产量总数标签

                if (total == 0)
                {
                    chartUph.Series[1].Points[i].Label = " ";
                    chartUph.Series[0].Points[i].LabelForeColor = Color.Black;
                    chartUph.Series[2].Points[i].Label = " ";

                }
            }

        }

        private void DrawPieBingTu() //绘制饼图
        {
            List<string> xData = new List<string>() { "A", "B" };
            List<double> yData = new List<double>();
            double NGyield = (double)(m_NGtotal) / (m_total + m_NGtotal) * 100;
            double OKyield = (double)m_total / (m_total + m_NGtotal )* 100;
            if (chartPie.Series.Count == 0) return;
            chartPie.Series[0].Points.Clear();        //清除所有点
            chartPie.Series[0].Label = "#PERCENT{P2}";//设置内容为百分比显示，P2为精确位数为两位小数
            chartPie.Series[0]["PieLabelStyle"] = "Disabled";
            //chartPie.Series[0]["PieLabelStyle"] = "Outside";//将文字移到外侧
            //chartPie.Series[0]["PieLineColor"] = "Black";//绘制黑色的连线。

            //计算百分比
            if (double.IsNaN(OKyield))
            {//此处判断a为NaN
                OKyield = 0.0;
            }
            if (double.IsNaN(NGyield))
            {//此处判断a为NaN
                NGyield = 0.0;
            }
            yData.Add(OKyield);
            yData.Add(NGyield);
            chartPie.Series[0].Points.DataBindXY(xData, yData);   //序列数据点集合1绑定数据
            chartPie.Series[0].Points[0].Color = Color.GreenYellow; //设置各区域颜色
            chartPie.Series[0].Points[1].Color = Color.Red;

            if ((OKyield + NGyield) != 0)
            {
                labelOK.Visible = true;
                labelNg.Visible = true;
                labelOK.Text = "OK总数占比 -- " + string.Format("{0:F2}", OKyield) + "%";
                labelNg.Text = "NG总数占比 -- " + string.Format("{0:F2}", NGyield) + "%";
            }
            else
            {
                labelOK.Visible = false;
                labelNg.Visible = false;
            }

        }

        public void SaveYieldData()
        {
            string path, name;
            double structID = m_reg.GetDValue("产量数据") > 0 ? m_reg.GetDValue("产量数据") : 0;
            NStruct stData = m_structManer.StructList[(short)structID];
            if (25 != stData.ItemCount || stData == null)
                return;

            name = stData.ItemList[0].DataList[0].Cval;
            if (string.IsNullOrEmpty(name))
                name = DateTime.Now.ToString("yyyyMMdd");
            path = string.Format("D:\\Data\\Yield\\{0}.csv", name);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            try
            {
                if(File.Exists(path))
                    File.Delete(path);
                File.Create(path).Close();
                m_csv = new CSV(path);
                m_csv.AddTile(new string[] { "PASS", "FAIL" });

                for (int i = 1; i < stData.ItemCount; i++)
                {
                    m_csv.AddRow(i, stData.ItemList[i].DataList[0].Dval.ToString(), stData.ItemList[i].DataList[1].Dval.ToString());
                    //m_csv.SetData(i, 0, stData.ItemList[i].DataList[0].Dval.ToString());
                    //m_csv.SetData(i, 1, stData.ItemList[i].DataList[1].Dval.ToString());
                }
                m_csv.Save(path);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("读取结构体出错！");
            }

        }

        private void LoadYieldDate(string date)
        {
            string path = "";
            string[] AnalysisData;
            int num = 0;
            path = string.Format("D:\\Data\\Yield\\{0}.csv", date);

            m_csv = new CSV(path);
            if (m_csv == null)
            {
                return;
            }

            for (int i = 1; i < m_csv.Data.Count; i++)
            {//去掉第一行标题
                AnalysisData = m_csv.Data[i].ToArray();

                if (AnalysisData.Count() < 2) continue;
                if (int.TryParse(AnalysisData[0], out num))
                {
                    m_stUphData[i - 1].total = num;
                }
                if (int.TryParse(AnalysisData[1], out num))
                {
                    m_stUphData[i - 1].tossing = num;
                }
                if (0 == (m_stUphData[i - 1].total + m_stUphData[i - 1].tossing - m_stUphData[i - 1].tossing))
                {
                    m_stUphData[i - 1].yield = 0;
                }
                else
                {
                    m_stUphData[i - 1].yield = m_stUphData[i - 1].total * 100 / (m_stUphData[i - 1].total + m_stUphData[i - 1].tossing) ;
                }
            }

        }

        private void dateTimePicker_Uph_ValueChanged(object sender, EventArgs e)
        {
            UpdateUph();
        }
    }

    public class Uph_Data
    {
        public int total;     //总产量
        public double yield;  //良率
        public int tossing;   //抛料个数
        public int wNgTotal;  //周NG总数
        public int dNgTotal;  //日NG总数
        public Uph_Data()
        {
            total = 0;
            yield = 0;
            tossing = 0;
            wNgTotal = 0;
            dNgTotal = 0;
        }
    };

}
