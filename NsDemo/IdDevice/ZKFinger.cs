using libzkfpcsharp;
using NUser;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace NsDemo
{
    internal class ZKFinger
    {
        Action<string, bool> m_Log;
        IntPtr mDevHandle = IntPtr.Zero;
        IntPtr mDBHandle = IntPtr.Zero;
        byte[] FPBuffer = null;
        int mfpWidth, mfpHeight;
        byte[][] RegTmps = new byte[3][];
        bool bStopEnroll = true;
        UserManager m_users = UserManager.GetInstance();
        int Fid = 5000; //注册ID初始号

        private static ZKFinger _Instance;
        public static ZKFinger Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ZKFinger();
                    _Instance.Init();
                    return _Instance;
                }
                else
                {
                    return _Instance;
                }
            }
        }


        public ZKFinger()
        {
            for (int i = 0; i < 3; i++)
            {
                RegTmps[i] = new byte[2048];
            }
        }

        void Log(string log, bool bSave = false)
        {
            if (m_Log != null)
                m_Log("ZKFinger:" + log, bSave);

        }

        public bool Init()
        {
            bStopEnroll = true;
            InitDev();
            LoadFinger();
            return true;
        }

        private bool InitDev()
        {
            int ret = zkfperrdef.ZKFP_ERR_OK;
            int DevCount = 0;

            zkfp2.Terminate();
            ret = zkfp2.Init();
            if (ret == zkfperrdef.ZKFP_ERR_OK)
                DevCount = zkfp2.GetDeviceCount();
            else
            {
                Log("Initialize fail, ret=" + ret + " !");
                return false;
            }

            if (DevCount <= 0)
            {
                zkfp2.Terminate();
                Log("No device connected!");
                return false;
            }
            /// 打开设备
            if (IntPtr.Zero == (mDevHandle = zkfp2.OpenDevice(0)))  //只使用设备0
            {
                Log("OpenDevice fail");
                return false;
            }
            if (IntPtr.Zero == (mDBHandle = zkfp2.DBInit()))
            {
                Log("Init DB fail");
                zkfp2.CloseDevice(mDevHandle);
                mDevHandle = IntPtr.Zero;
                return false;
            }

            byte[] paramValue = new byte[4];
            int size = 4;
            zkfp2.GetParameters(mDevHandle, 1, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpWidth);

            size = 4;
            zkfp2.GetParameters(mDevHandle, 2, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpHeight);

            FPBuffer = new byte[mfpWidth * mfpHeight];

            Log("Init Finish!");
            return true;
        }
        public int ReadID(out string uid, out string errmsg)
        {
            uid = "";
            errmsg = "";
            if(bStopEnroll == false)
            {
                return 2;
            }
            int id = Identify();
            if (id == 0)
            {
                return 1;
            }
            else if(id == -1)
            {
                uid = "-10000";
            }
                
            uid = $"ZK{id}";
            
            return 0;
        }
       


        public int Enroll(UserGroup group, string[] RightName)
        {
            int ret = zkfp.ZKFP_ERR_OK;
            int fid = 0, score = 0;
            int REGISTER_FINGER_COUNT = 3;
            bStopEnroll = false;

            //防止重复
            foreach (string user in m_users.UserList.Keys)
            {
                if (user.Contains("ZK") == false)
                    continue;
                string s = user.Replace("ZK", "");
                int num = 0;
                int.TryParse(s, out num);
                Fid = Fid > num ? Fid : num;
            }
            Fid++;


            for (int RegisterCount = 0; RegisterCount < REGISTER_FINGER_COUNT && bStopEnroll == false;)
            {
                int cbCapTmp = 2048;
                int cbRegTmp = 2048;
                byte[] CapTmp = new byte[cbCapTmp];
                byte[] RegTmp = new byte[cbRegTmp];
                ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                if (zkfp.ZKFP_ERR_OK != ret)
                    continue;

                ret = zkfp2.DBIdentify(mDBHandle, CapTmp, ref fid, ref score);
                if (zkfp.ZKFP_ERR_OK == ret)
                {
                    Log("This finger was already register by " + fid + "!");
                    bStopEnroll = true;
                    return 0; //指纹已经存在，不能添加
                }
                if (RegisterCount > 0 && zkfp2.DBMatch(mDBHandle, CapTmp, RegTmps[RegisterCount - 1]) <= 0)
                {
                    Log("Please press the same finger 3 times for the enrollment");
                    continue;
                }
                Array.Copy(CapTmp, RegTmps[RegisterCount], cbCapTmp);
                String strBase64 = zkfp2.BlobToBase64(CapTmp, cbCapTmp);
                byte[] blob = zkfp2.Base64ToBlob(strBase64);
                RegisterCount++;
                if (RegisterCount >= REGISTER_FINGER_COUNT)
                {
                    int ret1 = 0;
                    RegisterCount = 0;
                    if (zkfp.ZKFP_ERR_OK == (ret = zkfp2.DBMerge(mDBHandle, RegTmps[0], RegTmps[1], RegTmps[2], RegTmp, ref cbRegTmp)) &&
                           zkfp.ZKFP_ERR_OK == (ret1 = zkfp2.DBAdd(mDBHandle, Fid, RegTmp)))
                    {
                        Log($"enroll succ:{Fid}");

                        string uid = $"ZK{Fid}";
                        m_users.AddUser("system", uid, uid, (short)group);
                        foreach (string s in RightName)
                            m_users.AddRight("system", s, uid);
                        //保存文件
                        SaveFinger(RegTmp, cbRegTmp, Fid, group);


                        Log($"Add user success:{uid}");
                        bStopEnroll = true;
                        return Fid;
                    }
                    else
                    {
                        Log("enroll fail, error code=" + ret);
                        bStopEnroll = true;
                        return -1;//指纹添加失败
                    }
                }
                else
                {
                    Log("You need to press the " + (REGISTER_FINGER_COUNT - RegisterCount) + " times fingerprint");
                }
            }
            bStopEnroll = true;
            return -1;
        }

        //保存指纹模板
        private void SaveFinger(byte[] FingerReg, int length, int Fid, UserGroup group)
        {
            string path = System.Windows.Forms.Application.StartupPath;
            string PathFile = $"{path}\\MachineConfig\\ZKFinger\\config.csv";
            if (Directory.Exists(Path.GetDirectoryName(PathFile)) == false)
                Directory.CreateDirectory(Path.GetDirectoryName(PathFile));

            String strBase64 = zkfp2.BlobToBase64(FingerReg, length);
            string str = $"{Fid}\t{group}\t{strBase64}\r\n";
            File.AppendAllText(PathFile, str, Encoding.Unicode);
        }

        private void LoadFinger()
        {
            string[] limit = new string[] { "流程操作", "变量操作", "工站操作", "IO调试操作", "变量调试操作", "模拟量操作", "PLC操作", "通讯操作", "报警操作", "结构体操作" };
            string path = System.Windows.Forms.Application.StartupPath;
            string PathFile = $"{path}\\MachineConfig\\ZKFinger\\config.csv";
            if (File.Exists(PathFile) == false)
                return;

            string[] users = m_users.UserList.Keys.ToArray();
            foreach (string user in users)
            {
                if (user.Contains("ZF") == false)
                    continue;
                m_users.DelUser("system", user);
            }



            string[] lines = File.ReadAllLines(PathFile);
            foreach (string line in lines)
            {
                int idxFid = line.IndexOf("\t");
                string Fid = line.Substring(0, idxFid);
                int iFid = int.Parse(Fid);
                int idxGroup = line.IndexOf("\t", idxFid + 1);
                string StrGroup = line.Substring(idxFid + 1, idxGroup - idxFid - 1);
                string strBase64 = line.Substring(idxGroup + 1);
                UserGroup group = (UserGroup)Enum.Parse(typeof(UserGroup), StrGroup);
                if (group == UserGroup.AdminLimit || group == UserGroup.SystemLimit || group == UserGroup.RootLimit)
                    limit = m_users.UserRightList.Values.ToArray();

                byte[] blob = zkfp2.Base64ToBlob(strBase64);
                int ret1 = zkfp2.DBAdd(mDBHandle, iFid, blob);
                if (ret1 != zkfp.ZKFP_ERR_OK)
                    Log($"加载指纹失败:{Fid}");

                string uid = $"ZK{Fid}";
                m_users.AddUser("system", uid, uid, (short)group);
                foreach (string s in limit)
                    m_users.AddRight("system", s, uid);
            }




        }



        /// <summary>
        /// 指纹验证
        /// </summary>
        /// <returns>成功返回 ID值 ,失败返回 0</returns>
        private int Identify()
        {
            int ret = zkfp.ZKFP_ERR_OK;
            int fid = 0, score = 0;
            int cbCapTmp = 2048;
            byte[] CapTmp = new byte[cbCapTmp];

            //while (true)
            //{
            //    cbCapTmp = 2048;
            //    ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
            //    if (ret == zkfp.ZKFP_ERR_OK)
            //    {
            //        int A = 111;
            //    }
            //}

            ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
            if (zkfp.ZKFP_ERR_OK != ret)
                return 0;
            ret = zkfp2.DBIdentify(mDBHandle, CapTmp, ref fid, ref score);
            if (zkfp.ZKFP_ERR_OK != ret)
                return -1;


            Log("Identify succ, fid= " + fid + ",score=" + score + "!");
            return fid;
        }

        /// <summary>
        /// 登记指纹
        /// </summary>
        /// <param name="iFid">指定指纹ID</param>
        /// <returns>成功返回 ID值 ,失败返回 0</returns>
        private int Enroll(int iFid)
        {
            int ret = zkfp.ZKFP_ERR_OK;
            int fid = 0, score = 0;
            int REGISTER_FINGER_COUNT = 3;
            bStopEnroll = false;


            for (int RegisterCount = 0; RegisterCount < REGISTER_FINGER_COUNT && bStopEnroll == false;)
            {
                int cbCapTmp = 2048;
                int cbRegTmp = 2048;
                byte[] CapTmp = new byte[cbCapTmp];
                byte[] RegTmp = new byte[cbRegTmp];
                ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                if (zkfp.ZKFP_ERR_OK != ret)
                    continue;

                ret = zkfp2.DBIdentify(mDBHandle, CapTmp, ref fid, ref score);
                if (zkfp.ZKFP_ERR_OK == ret)
                {
                    Log("This finger was already register by " + fid + "!");
                    return fid;
                }
                if (RegisterCount > 0 && zkfp2.DBMatch(mDBHandle, CapTmp, RegTmps[RegisterCount - 1]) <= 0)
                {
                    Log("Please press the same finger 3 times for the enrollment");
                    continue;
                }
                Array.Copy(CapTmp, RegTmps[RegisterCount], cbCapTmp);
                String strBase64 = zkfp2.BlobToBase64(CapTmp, cbCapTmp);
                byte[] blob = zkfp2.Base64ToBlob(strBase64);
                RegisterCount++;
                if (RegisterCount >= REGISTER_FINGER_COUNT)
                {
                    RegisterCount = 0;
                    if (zkfp.ZKFP_ERR_OK == (ret = zkfp2.DBMerge(mDBHandle, RegTmps[0], RegTmps[1], RegTmps[2], RegTmp, ref cbRegTmp)) &&
                           zkfp.ZKFP_ERR_OK == (ret = zkfp2.DBAdd(mDBHandle, iFid, RegTmp)))
                    {
                        Log("enroll succ");
                        return iFid;
                    }
                    else
                    {
                        Log("enroll fail, error code=" + ret);
                        return 0;
                    }
                }
                else
                {
                    Log("You need to press the " + (REGISTER_FINGER_COUNT - RegisterCount) + " times fingerprint");
                }
            }
            return 0;
        }

        /// <summary>
        /// 中断指纹登记
        /// </summary>
        public void Stop_Enroll()
        {
            bStopEnroll = true;
        }


        /// <summary>
        /// 删除指纹
        /// </summary>
        /// <param name="iFid"></param>
        public void UnEnroll(int iFid)
        {
            int ret = zkfp2.DBDel(mDBHandle, iFid);
        }

        /// <summary>
        /// 删除所有指纹
        /// </summary>
        public void ClearAll()
        {
            int ret = zkfp2.DBClear(mDBHandle);
        }


    }
}
