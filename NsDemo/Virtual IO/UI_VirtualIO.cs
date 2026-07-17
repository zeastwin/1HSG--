using System;
using System.Drawing;
using System.Windows.Forms;

namespace NsDemo.Virtual_IO
{
    public partial class UI_VirtualIO : Form
    {
        private Label[,] _Input_Ctrl = new Label[4, 16];
        private Button[] _Output_button = new Button[16];
        private PictureBox[] _PictureBox_Sensor = new PictureBox[16];

        public UI_VirtualIO()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            for (int i = 0; i < _Output_button.Length; i++)
            {
                if (_Output_button[i].Name == button.Name)
                {
                    if (Virtual_IO_TCP.Instance.GetOutput(i) == 1)
                    {
                        Virtual_IO_TCP.Instance.SetOutput(i, 0);
                    }
                    else
                    {
                        Virtual_IO_TCP.Instance.SetOutput(i, 1);
                    }
                }
            }
        }

        private void UI_VirtualIO_Load(object sender, EventArgs e)
        {
            _Input_Ctrl[0, 0] = label25;
            _Input_Ctrl[0, 1] = label26;
            _Input_Ctrl[0, 2] = label27;
            _Input_Ctrl[0, 3] = label28;
            _Input_Ctrl[0, 4] = label29;
            _Input_Ctrl[0, 5] = label30;
            _Input_Ctrl[0, 6] = label31;
            _Input_Ctrl[0, 7] = label32;
            _Input_Ctrl[0, 8] = label33;
            _Input_Ctrl[0, 9] = label34;
            _Input_Ctrl[0, 10] = label35;
            _Input_Ctrl[0, 11] = label36;
            _Input_Ctrl[0, 12] = label37;
            _Input_Ctrl[0, 13] = label38;
            _Input_Ctrl[0, 14] = label39;
            _Input_Ctrl[0, 15] = label40;

            _Input_Ctrl[1, 0] = label79;
            _Input_Ctrl[1, 1] = label80;
            _Input_Ctrl[1, 2] = label81;
            _Input_Ctrl[1, 3] = label82;
            _Input_Ctrl[1, 4] = label83;
            _Input_Ctrl[1, 5] = label84;
            _Input_Ctrl[1, 6] = label85;
            _Input_Ctrl[1, 7] = label86;
            _Input_Ctrl[1, 8] = label87;
            _Input_Ctrl[1, 9] = label88;
            _Input_Ctrl[1, 10] = label89;
            _Input_Ctrl[1, 11] = label90;
            _Input_Ctrl[1, 12] = label91;
            _Input_Ctrl[1, 13] = label92;
            _Input_Ctrl[1, 14] = label93;
            _Input_Ctrl[1, 15] = label94;

            _Input_Ctrl[2, 0] = label95;
            _Input_Ctrl[2, 1] = label96;
            _Input_Ctrl[2, 2] = label97;
            _Input_Ctrl[2, 3] = label98;
            _Input_Ctrl[2, 4] = label99;
            _Input_Ctrl[2, 5] = label100;
            _Input_Ctrl[2, 6] = label101;
            _Input_Ctrl[2, 7] = label102;
            _Input_Ctrl[2, 8] = label103;
            _Input_Ctrl[2, 9] = label104;
            _Input_Ctrl[2, 10] = label105;
            _Input_Ctrl[2, 11] = label106;
            _Input_Ctrl[2, 12] = label107;
            _Input_Ctrl[2, 13] = label108;
            _Input_Ctrl[2, 14] = label109;
            _Input_Ctrl[2, 15] = label110;

            _Input_Ctrl[3, 0] = label63;
            _Input_Ctrl[3, 1] = label64;
            _Input_Ctrl[3, 2] = label65;
            _Input_Ctrl[3, 3] = label66;
            _Input_Ctrl[3, 4] = label67;
            _Input_Ctrl[3, 5] = label68;
            _Input_Ctrl[3, 6] = label69;
            _Input_Ctrl[3, 7] = label70;
            _Input_Ctrl[3, 8] = label71;
            _Input_Ctrl[3, 9] = label72;
            _Input_Ctrl[3, 10] = label73;
            _Input_Ctrl[3, 11] = label74;
            _Input_Ctrl[3, 12] = label75;
            _Input_Ctrl[3, 13] = label76;
            _Input_Ctrl[3, 14] = label77;
            _Input_Ctrl[3, 15] = label78;

            _Output_button[0] = button1;
            _Output_button[1] = button2;
            _Output_button[2] = button3;
            _Output_button[3] = button4;
            _Output_button[4] = button5;
            _Output_button[5] = button6;
            _Output_button[6] = button7;
            _Output_button[7] = button8;
            _Output_button[8] = button9;
            _Output_button[9] = button10;
            _Output_button[10] = button11;
            _Output_button[11] = button12;
            _Output_button[12] = button13;
            _Output_button[13] = button14;
            _Output_button[14] = button15;
            _Output_button[15] = button16;

            _PictureBox_Sensor[0] = pictureBox1;
            _PictureBox_Sensor[1] = pictureBox2;
            _PictureBox_Sensor[2] = pictureBox3;
            _PictureBox_Sensor[3] = pictureBox4;
            _PictureBox_Sensor[4] = pictureBox5;
            _PictureBox_Sensor[5] = pictureBox6;
            _PictureBox_Sensor[6] = pictureBox7;
            _PictureBox_Sensor[7] = pictureBox8;
            _PictureBox_Sensor[8] = pictureBox9;
            _PictureBox_Sensor[9] = pictureBox10;
            _PictureBox_Sensor[10] = pictureBox11;
            _PictureBox_Sensor[11] = pictureBox12;
            _PictureBox_Sensor[12] = pictureBox13;
            _PictureBox_Sensor[13] = pictureBox14;
            _PictureBox_Sensor[14] = pictureBox15;
            _PictureBox_Sensor[15] = pictureBox16;

            for (int i = 0; i < Virtual_IO_TCP.Instance._IO_List._array_input.Length; i++)
            {
                _Input_Ctrl[0, i].Text = Virtual_IO_TCP.Instance._IO_List._array_input[i].IO_Name;
                _Input_Ctrl[1, i].Text = "";
                _Input_Ctrl[2, i].Text = "";
                for (int j = 0; j < Virtual_IO_TCP.Instance._IO_List._array_matching_model.Count; j++)
                {
                    if (Virtual_IO_TCP.Instance._IO_List._array_matching_model[j].Input_Index == i)
                    {
                        _Input_Ctrl[1, i].Text = Virtual_IO_TCP.Instance._IO_List._array_matching_model[j].CardID;
                        _Input_Ctrl[2, i].Text = Virtual_IO_TCP.Instance._IO_List._array_matching_model[j].Output_Index.ToString();
                    }
                }
            }

            for (int i = 0; i < Virtual_IO_TCP.Instance._IO_List._array_output.Length; i++)
            {
                _Output_button[i].Text = Virtual_IO_TCP.Instance._IO_List._array_output[i].IO_Name;
                // _Input_Ctrl[3, i].Text = Virtual_IO_TCP.Instance._IO_List._array_input[i].IO_Status.ToString();
            }

            label2.Text = Virtual_IO_TCP.Instance._IO_List.CardID;
            label44.Text = "服务器:" + Virtual_IO_TCP.Instance.virtualIO_IP_Config.serverIP.IP + "\r端口号:" + Virtual_IO_TCP.Instance.virtualIO_IP_Config.serverIP.Port.ToString();
        }

        public void Start()
        {
            timer1.Enabled = true;
            timer1.Start();
        }

        private void Update_IO()
        {
            //更新输入
            for (int i = 0; i < Virtual_IO_TCP.Instance._IO_List._array_input.Length; i++)
            {
                if (i < _PictureBox_Sensor.Length)
                {
                    if (Virtual_IO_TCP.Instance._IO_List._array_input[i].IO_Status == 0)
                    {
                        _PictureBox_Sensor[i].BackColor = Color.Gray;
                    }
                    else if (Virtual_IO_TCP.Instance._IO_List._array_input[i].IO_Status == 1)
                    {
                        _PictureBox_Sensor[i].BackColor = Color.Green;
                    }
                    else
                    {
                        _PictureBox_Sensor[i].BackColor = Color.Red;
                    }
                }
            }

            //更新输出
            for (int i = 0; i < Virtual_IO_TCP.Instance._IO_List._array_output.Length; i++)
            {
                if (i < _Output_button.Length)
                {
                    if (Virtual_IO_TCP.Instance._IO_List._array_output[i].IO_Status == 0)
                    {
                        _Output_button[i].BackColor = Color.DarkGray;
                    }
                    else if (Virtual_IO_TCP.Instance._IO_List._array_output[i].IO_Status == 1)
                    {
                        _Output_button[i].BackColor = Color.GreenYellow;
                    }
                    else
                    {
                        _Output_button[i].BackColor = Color.Red;
                    }
                }
            }

            if (Virtual_IO_TCP.Instance.GetServerStatus() != null)
            {
                if (Virtual_IO_TCP.Instance.GetServerStatus() != label46.Text)
                {
                    label46.Text = Virtual_IO_TCP.Instance.GetServerStatus();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Update_IO();
        }

        private void UI_VirtualIO_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            timer1.Stop();
            Hide();
            e.Cancel = true;
        }
    }
}