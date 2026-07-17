using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NVarConfig;


namespace NsDemo
{
    public partial class AlamerView : Form
    {
        private string m_strFolderPath = "";
        private CSV m_csv;
        private VarInteface m_var = VarInteface.GetInstance();
        private string pathVarName = "WarmLogPath"; //可以用来重新设置warm的保存路径
        private string m_date;
        public AlamerView()
        {
            InitializeComponent();
			m_date = dateTimePickerAlamer.Value.ToString("yyyyMMdd");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = string.Empty;
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_strFolderPath = fbd.SelectedPath;
                textBoxPath.Text = m_strFolderPath;
            }
        }

        private void dateTimePickerAlamer_ValueChanged(object sender, EventArgs e)
        {
			m_date = dateTimePickerAlamer.Value.ToString("yyyyMMdd");
        }

        private void DisPlyChoiceDay(string path, string date)
        {
	        // TODO: 在此添加控件通知处理程序代码;
	        string FilePath;
	        string[] AnalysisData;

	        //FilePath = _T("D:\\Data\\WarmDisplyLog\\WarmDisplyLog") + str + _T(".csv");
            FilePath = path + "WarmDisplyLog\\WarmDisplyLog" + date + ".csv";

	        if (!File.Exists(FilePath))
	        {
		        MessageBox.Show("加载文件不存在！","警告",MessageBoxButtons.OK);
		        return;
	        }
            m_csv = new CSV(FilePath);
            if (m_csv == null)
            {
                MessageBox.Show("文件打开失败！", "警告", MessageBoxButtons.OK);
                return;
            }
            dataGridViewAlamer.Rows.Clear();
            for (int i = 1; i < m_csv.Data.Count; i++)
            {//去掉第一行标题
                AnalysisData = m_csv.Data[i].ToArray();
                dataGridViewAlamer.Rows.Add();
                for (int n = 0; n < AnalysisData.Count(); n++)
                {
                    dataGridViewAlamer.Rows[i-1].Cells[n].Value = AnalysisData[n];
                   // dataGridViewAlamer.Rows[i].Cells[n].ReadOnly = true;
                }
            }
          
        }

        private void buttonLogFlush_Click(object sender, EventArgs e)
        {
            string path = "D:\\Data\\";//默认路径
            DisPlyChoiceDay(path, string.Format("{0:yyyyMMdd}", DateTime.Now));
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (m_strFolderPath.Equals(""))
            {
                MessageBox.Show("请选择加载日志的路径！", "提示", MessageBoxButtons.OK);
                return;
            }
			DisPlyChoiceDay(m_strFolderPath, m_date);
        }

        private void buttonSavePath_Click(object sender, EventArgs e)
        {
            if (m_strFolderPath.Equals(""))
            {
                MessageBox.Show("修改前请选择报警日志重新保存的路径！", "提示", MessageBoxButtons.OK);
                return;
            }
            VarInfo varinfo = m_var.GetVarInfo(pathVarName);
            if (varinfo == null)
            {
                MessageBox.Show("报警日志路径变量不存在,请先设置变量名称为【WarmLogPath】的变量！", "提示", MessageBoxButtons.OK);
                return;
            }
            else
            {
                if (DialogResult.OK == MessageBox.Show("确定要修改报警日志保存路径？","提示", MessageBoxButtons.OKCancel))
                {
                    varinfo.varCval = m_strFolderPath;
                    m_var.SetVarInfo(varinfo, true);
                }
            }
            
        }

    }
}
