using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;

namespace NsDemo.Virtual_IO
{
    #region 自定义事件

    public delegate void EventDelegate_IOSTATUS(string sender, IO_EventArgs e);

    // 自定义事件参数类，用于传递返回值
    public class IO_EventArgs : EventArgs
    {
        public IO_Info[] _list_input { get; set; }
        public IO_Info[] _list_output { get; set; }
        public string _CardID { get; set; }
    }

    #endregion 自定义事件

    public class Virtual_IO_TCP : IDisposable
    {
        #region 变量

        public list_ioinfo _IO_List;  //IO列表
        public VirtualIO_IP_Config virtualIO_IP_Config;  //IP配置文件
        private TCP_Server tCP_Server; //服务端
        private List<TCP_Client> _array_tcp_Client;//客户端
        private List<Thread> listenThread;
        private bool is_SendFlag;

        public static event Action<string, string> EVENT_VIRTUREIO_ADDLOG;

        #endregion 变量

        #region 单例

        private Virtual_IO_TCP()
        {
        }

        private static Virtual_IO_TCP _instance;

        //public static Virtual_IO_TCP Instance => _instance ?? (_instance = new Virtual_IO_TCP());
        public static Virtual_IO_TCP Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Virtual_IO_TCP();
                    _instance.InitVirtual_IO();
                    return _instance;
                }
                else
                {
                    return _instance;
                }
            }
        }

        #endregion 单例

        #region 私有函数

        private void InitVirtual_IO()
        {
            string filepath = System.Windows.Forms.Application.StartupPath;
            _IO_List = list_ioinfo.Serialize2Obj(filepath + @"\MachineConfig\VirtualIO\Virtual.json");
            virtualIO_IP_Config = VirtualIO_IP_Config.Serialize2Obj(filepath + @"\MachineConfig\VirtualIO\VirtualIO_IP.json");
            StopPro_Server(virtualIO_IP_Config.serverIP.IP, virtualIO_IP_Config.serverIP.Port);
            Init(virtualIO_IP_Config.serverIP.IP, virtualIO_IP_Config.serverIP.Port);
            _array_tcp_Client = new List<TCP_Client>();
            listenThread = new List<Thread>();
            is_SendFlag = true;
            for (int i = 0; i < virtualIO_IP_Config.client_IP.Count; i++)
            {
                Thread thread = new Thread(Thread_Client_Send);
                thread.Start(virtualIO_IP_Config.client_IP[i]);
                listenThread.Add(thread);
            }
        }

        private void Init(string ipAddress, int port)
        {
            UpDate_Status("初始化服务器", ipAddress + "   " + port.ToString());
            tCP_Server = new TCP_Server(ipAddress, port);
            tCP_Server.Start();
            tCP_Server.Event_GET_IO_STATUS += Get_IO_Status;
            TCP_Server.Event_UPDATE_IOSTATUS += UpDateIO;
        }

        private void Get_IO_Status(string strType, IO_EventArgs e)
        {
            e._list_input = _IO_List._array_input;
            e._list_output = _IO_List._array_output;
            e._CardID = _IO_List.CardID;
        }

        private string Get_CurrentIOStatus()
        {
            string strID = _IO_List.CardID;
            string strinput = "";
            string strOutput = "";
            for (int i = 0; i < _IO_List._array_input.Length; i++)
            {
                strinput += _IO_List._array_input[i].IO_Status.ToString() + ",";
            }

            for (int i = 0; i < _IO_List._array_output.Length; i++)
            {
                strOutput += _IO_List._array_output[i].IO_Status.ToString() + ",";
            }
            strID = strID + ";" + strinput + ";" + strOutput;
            return strID;
        }

        private void UpDateIO(string strStatus)
        {
            string[] vec_Data = strStatus.Split(';');
            if (vec_Data.Length < 4) { return; }
            string[] vec_output = vec_Data[3].Split(',');
            for (int i = 0; i < _IO_List._array_matching_model.Count; i++)
            {
                if (_IO_List._array_matching_model[i].CardID == vec_Data[1])
                {
                    if (_IO_List._array_matching_model[i].Output_Index < vec_output.Count() &&
                        _IO_List._array_matching_model[i].Output_Index >= 0 &&
                        _IO_List._array_matching_model[i].Input_Index >= 0 &&
                        _IO_List._array_matching_model[i].Input_Index < _IO_List._array_input.Length)
                    {
                        _IO_List._array_input[_IO_List._array_matching_model[i].Input_Index].IO_Status = int.Parse(vec_output[_IO_List._array_matching_model[i].Output_Index]);
                    }
                }
            }
        }

        private bool StopPro_Server(string ipAddressString, int port)
        {
            string ipAddress = ipAddressString;
            string portNumber = port.ToString();

            // 构造命令行指令
            string command = $"netstat -ano | findstr {ipAddress}:{portNumber}";

            // 创建进程对象并设置属性
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C {command}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // 启动进程并等待执行完成
            process.Start();
            process.WaitForExit();

            // 读取命令输出结果

            string output = process.StandardOutput.ReadToEnd();

            // 显示输出结果
            Console.WriteLine(output);

            // 获取进程 ID
            string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] tokens = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length >= 5)
                {
                    if (tokens[1] == ipAddressString + ":" + port.ToString())
                    {
                        string processId = tokens[4];
                        kill_ProID(int.Parse(processId));
                    }
                }
            }

            return true;
        }

        private bool kill_ProID(int processId)
        {
            // 创建进程对象并设置属性
            Process process = new Process();
            process.StartInfo.FileName = "taskkill.exe";
            process.StartInfo.Arguments = $"/F /PID {processId}"; // 使用 /F 强制终止进程
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // 启动进程并等待执行完成
            process.Start();
            process.WaitForExit();

            // 获取命令输出结果
            string output = process.StandardOutput.ReadToEnd();

            // 显示输出结果
            Console.WriteLine(output);
            UpDate_Status("终止流程", processId.ToString());
            if (process.ExitCode == 0)
            {
                Console.WriteLine("进程终止成功");
            }
            else
            {
                Console.WriteLine("进程终止失败");
            }
            return true;
        }

        private void UpDate_Status(string StrIP, string strinfo)
        {
            if (EVENT_VIRTUREIO_ADDLOG != null)
            {
                EVENT_VIRTUREIO_ADDLOG.BeginInvoke(StrIP, strinfo, null, null);
            }
        }

        #endregion 私有函数

        #region 公有函数

        public void Dispose()
        {
            if (tCP_Server != null)
            {
                tCP_Server.Dispose();
            }
            is_SendFlag = false;
            Thread.Sleep(1000);
            for (int i = 0; i < listenThread.Count; i++)
            {
                try
                {
                    listenThread[i].Abort();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public bool SetOutput(int Index, int Status)
        {
            if (Index >= 0 && Index < _IO_List._array_output.Length)
            {
                _IO_List._array_output[Index].IO_Status = Status;
                return true;
            }
            return false;
        }

        public int GetOutput(int Index)
        {
            if (Index >= 0 && Index < _IO_List._array_output.Length)
            {
                return _IO_List._array_output[Index].IO_Status;
            }
            return -1;
        }

        public int GetInput(int Index)
        {
            if (Index >= 0 && Index < _IO_List._array_input.Length)
            {
                return _IO_List._array_input[Index].IO_Status;
            }
            return -1;
        }

        public string GetServerStatus()
        {
            return tCP_Server.Link_Status;
        }

        #endregion 公有函数

        private static void Thread_Client_Send(object parameter)
        {
            IP_Config tCP_ClientConfig = parameter as IP_Config;
            if (tCP_ClientConfig == null) return;
            _instance.UpDate_Status("客户端连接服务器", tCP_ClientConfig.IP + "   " + tCP_ClientConfig.Port.ToString());
            string str_history_IOStatus = _instance.Get_CurrentIOStatus();
            DateTime SendDateTime = DateTime.Now;
            while (_instance.is_SendFlag)
            {
                //获取当前IO状态
                string str_IOStatus = _instance.Get_CurrentIOStatus();

                //发送数据
                if (str_history_IOStatus != str_IOStatus)
                {
                    TCP_Client tCP_Client = new TCP_Client(tCP_ClientConfig.IP, tCP_ClientConfig.Port);
                    tCP_Client.Connect();
                    byte[] SendBuffData = Encoding.UTF8.GetBytes("UPDATE;" + str_IOStatus);
                    if (tCP_Client.SendData(SendBuffData))
                    {
                        Thread.Sleep(50);
                        byte[] ReceveBuffData = tCP_Client.ReceiveData(3000);
                        string str = Encoding.UTF8.GetString(ReceveBuffData);
                        if (str.Contains("OK"))
                        {
                            str_history_IOStatus = str_IOStatus;
                            SendDateTime = DateTime.Now;
                            _instance.UpDate_Status("客户端", "更新输出状态成功");
                        }
                    }
                    tCP_Client.Close();
                }

                TimeSpan timeSpan = DateTime.Now - SendDateTime;
                if (timeSpan.TotalSeconds > 2)
                {
                    TCP_Client tCP_Client = new TCP_Client(tCP_ClientConfig.IP, tCP_ClientConfig.Port);
                    tCP_Client.Connect();
                    byte[] SendBuffData = Encoding.UTF8.GetBytes("GETIO");
                    if (tCP_Client.SendData(SendBuffData))
                    {
                        Thread.Sleep(50);
                        byte[] ReceveBuffData = tCP_Client.ReceiveData(1000);
                        string str = Encoding.UTF8.GetString(ReceveBuffData);
                        _instance.UpDateIO(str);
                        SendDateTime = DateTime.Now;
                    }
                    else
                    {
                        string strError = "GETIO;" + tCP_ClientConfig.Name + ";-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,;-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,";
                        _instance.UpDateIO(strError);
                        SendDateTime = DateTime.Now;
                    }

                    tCP_Client.Close();
                }

                Thread.Sleep(20);
            }
        }
    }

    #region IO信息定义

    public class IO_Info
    {
        public int IO_Status;  //IO状态
        public string IO_Name; //IO名称
        public int IO_Index;   //索引
    }

    public class input_matching_model
    {
        public string CardID; //IO名称
        public int Output_Index;   //输出索引
        public int Input_Index;    //输入索引
    }

    public class list_ioinfo
    {
        public string CardID;
        public IO_Info[] _array_input;
        public IO_Info[] _array_output;
        public List<input_matching_model> _array_matching_model;

        public string SerializeToString(list_ioinfo obj)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(obj);

                string filePath = @"D:\Virtual.json"; // 替换为实际的文件路径

                try
                {
                    // 打开文件以进行写入
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.Write(jsonData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("发生异常： " + ex.Message);
                }

                return jsonData;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public list_ioinfo()
        {
            _array_input = new IO_Info[16];
            _array_output = new IO_Info[16];
            _array_matching_model = new List<input_matching_model>();
            //CardID = "1#";

            //for (int i = 0; i < 16; i++)
            //{
            //    IO_Info iO_Info = new IO_Info();
            //    iO_Info.IO_Status = 0;
            //    iO_Info.IO_Name = "输入" + i.ToString("00");
            //    iO_Info.IO_Index = i;
            //    _array_input[i] = iO_Info;
            //}

            //for (int i = 0; i < 16; i++)
            //{
            //    IO_Info iO_Info = new IO_Info();
            //    iO_Info.IO_Status = 0;
            //    iO_Info.IO_Name = "输出" + i.ToString("00");
            //    iO_Info.IO_Index = i;
            //    _array_output[i] = iO_Info;
            //}

            //for (int i = 0; i < 3; i++)
            //{
            //    input_matching_model input_matching = new input_matching_model();
            //    input_matching.Output_Index = i;
            //    input_matching.Input_Index = i;
            //    input_matching.CardID = "1#";
            //    _array_matching_model.Add(input_matching);
            //}

            //SerializeToString(this);
        }

        public static list_ioinfo Serialize2Obj(string filePath)
        {
            try
            {
                // 打开文件以进行读取
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string content = reader.ReadToEnd();
                    list_ioinfo list_Ioinfo = JsonConvert.DeserializeObject<list_ioinfo>(content);
                    return list_Ioinfo;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }

    #endregion IO信息定义

    #region 服务端

    public class TCP_Server : IDisposable
    {
        private TcpListener listener;
        private Thread listenThread;
        private bool is_Listening;
        public string Link_Status;

        #region 事件

        public event EventDelegate_IOSTATUS Event_GET_IO_STATUS;

        public static event Action<string> Event_UPDATE_IOSTATUS;

        #endregion 事件

        public void Dispose()
        {
            is_Listening = false;
            try
            {
                Thread.Sleep(1000);
                listenThread.Abort();
                listener.Stop();
            }
            catch
            {
            }
        }

        private string Get_IO_Status()
        {
            if (Event_GET_IO_STATUS != null)
            {
                IO_EventArgs iO_EventArgs = new IO_EventArgs();
                Event_GET_IO_STATUS.Invoke("", iO_EventArgs);
                string strType = "GETIO";
                string strID = iO_EventArgs._CardID;
                string strinput = "";
                string strOutput = "";
                for (int i = 0; i < iO_EventArgs._list_input.Length; i++)
                {
                    strinput += iO_EventArgs._list_input[i].IO_Status.ToString() + ",";
                }

                for (int i = 0; i < iO_EventArgs._list_output.Length; i++)
                {
                    strOutput += iO_EventArgs._list_output[i].IO_Status.ToString() + ",";
                }
                strID = strType + ";" + strID + ";" + strinput + ";" + strOutput;
                return strID;
            }
            return "Error;Get IO Status Error";
        }

        public TCP_Server(string ipAddress, int port)
        {
            // 创建TCP监听器
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            listenThread = new Thread(new ThreadStart(ListenForClients));
        }

        public void Start()
        {
            // 启动监听线程
            is_Listening = true;
            listenThread.Start();
            Console.WriteLine("服务器已启动，等待连接...");
        }

        private void ListenForClients()
        {
            try
            {
                // 开始监听
                listener.Start();

                while (is_Listening)
                {
                    try
                    {
                        Thread.Sleep(20);
                        // 检查是否有传入的连接
                        if (listener.Pending())
                        {
                            // 接受连接
                            Link_Status = "接收数据";
                            TcpClient client = listener.AcceptTcpClient();
                            HandleClient(client);
                        }
                        else
                        {
                            Link_Status = "等待接收";
                        }
                    }
                    catch (SocketException ex)
                    {
                        Link_Status = ex.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HandleClient(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;

            // 获取网络流
            NetworkStream stream = client.GetStream();

            // 读取客户端发送的数据
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            if (dataReceived.Contains("GETIO"))
            {
                string responseMessage;
                responseMessage = Get_IO_Status();
                byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);
                stream.Write(responseData, 0, responseData.Length);
            }
            else if (dataReceived.Contains("UPDATE"))
            {
                byte[] responseData = Encoding.UTF8.GetBytes("OK");

                stream.Write(responseData, 0, responseData.Length);
                if (Event_UPDATE_IOSTATUS != null)
                {
                    Event_UPDATE_IOSTATUS.Invoke(dataReceived);
                }
            }
            client.Close();
        }
    }

    #endregion 服务端

    #region 客户端

    public class IP_Config
    {
        public string IP;
        public int Port;
        public string Name;
    }

    public class VirtualIO_IP_Config
    {
        public IP_Config serverIP;
        public List<IP_Config> client_IP;

        public static VirtualIO_IP_Config Serialize2Obj(string filePath)
        {
            try
            {
                // 打开文件以进行读取
                using (StreamReader reader = new StreamReader(filePath, Encoding.Default))
                {
                    string content = reader.ReadToEnd();
                    VirtualIO_IP_Config VirtualIO_Config = JsonConvert.DeserializeObject<VirtualIO_IP_Config>(content);
                    return VirtualIO_Config;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public string SerializeToString(VirtualIO_IP_Config obj)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(obj);

                string filePath = @"D:\VirtualIO_IP.json"; // 替换为实际的文件路径

                try
                {
                    // 打开文件以进行写入
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.Write(jsonData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("发生异常： " + ex.Message);
                }

                return jsonData;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public VirtualIO_IP_Config()
        {
            //serverIP = new IP_Config();
            ////serverIP.IP = "127.0.0.1";
            ////serverIP.Port = 6001;

            //client_IP = new List<IP_Config>();
            //IP_Config clientIP1 = new IP_Config();
            //clientIP1.IP = "127.0.0.1";
            //clientIP1.Port = 6002;

            //IP_Config clientIP2 = new IP_Config();
            //clientIP2.IP = "127.0.0.1";
            //clientIP2.Port = 6002;

            //IP_Config clientIP3 = new IP_Config();
            //clientIP3.IP = "127.0.0.1";
            //clientIP3.Port = 6002;

            //client_IP.Add(clientIP1);
            //client_IP.Add(clientIP2);
            //client_IP.Add(clientIP3);

            //SerializeToString(this);
        }
    }

    public class TCP_Client
    {
        private Socket clientSocket;
        private string serverHost;
        private int serverPort;

        public TCP_Client(string host, int port)
        {
            serverHost = host;
            serverPort = port;
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public string Connect()
        {
            try
            {
                clientSocket.Connect(serverHost, serverPort);
                return "OK";
            }
            catch (Exception e)
            {
                return "Error:" + e.Message;
            }
        }

        public bool SendData(byte[] data)
        {
            try
            {
                clientSocket.Send(data);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public byte[] ReceiveData()
        {
            byte[] buffer = new byte[1024]; // 适当的缓冲区大小
            int bytesRead = clientSocket.Receive(buffer);
            byte[] receivedData = new byte[bytesRead];
            Array.Copy(buffer, receivedData, bytesRead);
            return receivedData;
        }

        public byte[] ReceiveData(int timeoutMilliseconds)
        {
            byte[] buffer = new byte[1024];
            clientSocket.ReceiveTimeout = timeoutMilliseconds;

            try
            {
                int bytesRead = clientSocket.Receive(buffer);
                byte[] receivedData = new byte[bytesRead];
                Array.Copy(buffer, receivedData, bytesRead);
                return receivedData;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    // 处理超时异常
                    return Encoding.Default.GetBytes("Error Timeout");
                }
                else
                {
                    return Encoding.Default.GetBytes("Error other");
                }
            }
        }

        public void Close()
        {
            clientSocket.Close();
        }
    }

    #endregion 客户端
}