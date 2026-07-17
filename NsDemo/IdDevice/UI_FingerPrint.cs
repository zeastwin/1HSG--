using NsDemo.Comm_Ctrl;
using NUser;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace NsDemo.IdDevice
{
    public partial class UI_FingerPrint : Form
    {
        Thread _Thread;


        public UI_FingerPrint()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if(_Thread == null || _Thread.ThreadState == ThreadState.Stopped || _Thread.ThreadState == ThreadState.Aborted || _Thread.ThreadState == ThreadState.Unstarted)
            {
                _Thread = new Thread(Erroll_Thread);
                _Thread.Start(this);
            }
            
        }

        private void UI_FingerPrint_Load(object sender, EventArgs e)
        {
            EnableWindown(false);
            
        }
        public static void Erroll_Thread(object lparam)
        {
            UI_FingerPrint Instance = (UI_FingerPrint)lparam;
            string[] limit = new string[] { "流程操作", "变量操作", "工站操作", "IO调试操作", "变量调试操作", "模拟量操作", "PLC操作", "通讯操作", "报警操作", "结构体操作" };
            int aa = -1;
            Instance.button1.BackColor = Color.LightGreen;
            if (Instance.radioButton1.Checked == true)
            {
                aa = ZKFinger.Instance.Enroll(UserGroup.OpLimit, limit);
            }
            else if (Instance.radioButton2.Checked == true)
            {
                aa = ZKFinger.Instance.Enroll(UserGroup.AdminLimit, limit);
            }
            else if (Instance.radioButton3.Checked == true)
            {
                aa = ZKFinger.Instance.Enroll(UserGroup.SystemLimit, limit);
            }
            else
            {
                Translation_Message.Instance.MessageShow("请选择权限");
                Instance.button1.BackColor = Color.Gray;
                return;
            }
            if (aa == 0)
            {
                Translation_Message.Instance.MessageShow("指纹已经存在，不能添加");
            }
            else if (aa == -1)
            {
                Translation_Message.Instance.MessageShow("添加指纹失败");
            }
            else
            {
                Translation_Message.Instance.MessageShow("添加指纹成功");
            }
            Instance.button1.BackColor = Color.LightGray;
        }

        private void EnableWindown(bool Falg)
        {
            button1.Enabled = Falg;
            radioButton1.Enabled = Falg;
            radioButton2.Enabled = Falg;
            radioButton3.Enabled = Falg;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "system" && textBox2.Text == "software_123")
            {
                EnableWindown(true);
                button1.BackColor = Color.LightYellow;
            }
            EnableWindown(true);
            button1.BackColor = Color.LightYellow;
        }

        private void UI_FingerPrint_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
