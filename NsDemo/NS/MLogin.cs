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
    public partial class MLogin : Form
    {
        public MLogin()
        {
            InitializeComponent();         
        }

		public void ShowWin(bool bfocus = false)
		{
			Show();
			loginManager1.Focus(bfocus);
		}

        private void loginManager1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button  == MouseButtons.Left)
            {
                Point mouseSet = new Point(e.X, e.Y );
                Location = mouseSet;
                this.Invalidate();
            }
        }

        private void MLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void close_Click(object sender, EventArgs e)
        {
            Close();
        }       
    }
}
