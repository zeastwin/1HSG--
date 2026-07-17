using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using NLog;
using NInterface;
using NsDemo.Virtual_IO;

namespace NsDemo
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                using (System.Threading.Mutex mx = new System.Threading.Mutex(true, "e2d6cdc5-5b55-412c-9e88-f82a7ace810e"))
                {
                    if (!mx.WaitOne(TimeSpan.Zero, true))
                    {
						if (MessageBox.Show("软件只能打开一个, 如果需要多开, 请点否 ", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            return;
                        }
						NInterface.RTConfig.IsFrameworkNoInit = MessageBox.Show("是否初始化底层?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes;
					}
					try
					{
						Application.EnableVisualStyles();
						Application.SetCompatibleTextRenderingDefault(false);
            			var frm = new FormStart();
						Application.Run(frm);
                        Virtual_IO_TCP.Instance.Dispose();
                    }
					catch (Exception ex)
					{
						MessageBox.Show("窗体发生不可逆异常: " + ex.ToString() + ex.Message + "\n即将退出..", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						Log.WriteLine("奔溃异常:位置{0}{1}", Log.Error, ex.ToString(), ex.Message);
					}

                }        
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Program 异常.");
            }
        }
			
    }
}
