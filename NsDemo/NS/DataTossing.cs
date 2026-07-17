using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NsDemo
{
    public partial class DataTossing : Form
    {
        private List<int> m_datalist = new List<int>();
        public DataTossing()
        {
            InitializeComponent();
            chartTossing.Series[0].Points.AddXY(0, 0);   //添加点,使其显示出来
        }

        public void UpdateTossing()
        {
            for (int i = 0; i < 7; i++)//test
            {
                m_datalist.Add(20*i);
            }
           
            DrawChartTossing(m_datalist);
        }

        private void DrawChartTossing(List<int> data)
        {
            chartTossing.Series[0].Points.Clear();
            for (int i = 0; i < 7; i++)
            {
                chartTossing.Series[0].Points.AddXY(dateTimePicker_Tossing.Value.Date.AddDays(+i), data[i]);
            }
        }

        private int GetDays(int year,int month)
        {
	        int nRet;
	        switch(month)
	        {
		        case 4:
		        case 6:
		        case 9:
		        case 11:
			        nRet = 30;
		        break;
		        case 2:
			        if ((year%4 == 0 && year%100 !=0) || year%400 == 0)
			        {
				        nRet = 29;
			        }
			        else
				        nRet = 28;
		        break;
		        default:
			        nRet = 31;
			        break;
	        }
	        return nRet;
        }

        private void dateTimePicker_Tossing_ValueChanged(object sender, EventArgs e)
        {
            UpdateTossing();
        }
    }
}
