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
    public partial class LogView : Form
    {
        public LogView()
        {
            InitializeComponent();
        }

        private void LogView_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
     
        private void LogView_Load(object sender, EventArgs e)
        {
          
        }

       
    }
}
