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
    public partial class DataView : Form
    {
        DataUph m_uph;
        DataTossing m_tossing;
        public DataView()
        {
            InitializeComponent();
            m_uph = new DataUph();
            m_tossing = new DataTossing();
            UpdateUIView(SHOWUI.UPH_SEL);
        }

        private void buttonUPH_Click(object sender, EventArgs e)
        {
            UpdateUIView(SHOWUI.UPH_SEL);
            m_uph.UpdateUph();

        }

        private void buttonNG_Click(object sender, EventArgs e)
        {
            UpdateUIView(SHOWUI.NG_SEL);
            m_tossing.UpdateTossing();
        }

        private void UpdateUIView(SHOWUI id)
        {
            Form form = null;
            try
            {
                switch (id)
                {
                    case SHOWUI.UPH_SEL:
                        form = m_uph;
                        break;
                    case SHOWUI.NG_SEL:
                        form = m_tossing;
                        break;
                    default:
                        break;
                }

                if (null != form)
                {
                    if (panelDisp.Contains(form))
                    {
                        return;
                    }
                    panelDisp.Controls.Clear();
                    form.TopLevel = false;
                    form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    form.Dock = DockStyle.Fill;
                    form.Visible = true;
                    panelDisp.Controls.Add(form);
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public enum SHOWUI
    {
        NO_SEL = -1,
        UPH_SEL = 0,
        NG_SEL,
    }
}
