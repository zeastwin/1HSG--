//using NsDemo.Action_CoreWork;
using NUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NsDemo
{
    internal class NFCReader
    {

        private static NFCReader _Instance;
        public static NFCReader Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new NFCReader();
                    return _Instance;
                }
                else
                {
                    return _Instance;
                }
            }
        }


        public int ReadID(out string uid,out string errmsg)
        {
            uid = string.Empty;
            errmsg = string.Empty;

            int res = Init1();
            if(res == 0)
            {
                res = Connect1();
            }
            if (res == 0)
            {
                IntPtr puid = GetUID1();
                uid = Marshal.PtrToStringAnsi(puid);
            }
            if (res != 0)
            {
                IntPtr pErr = GetScardErrMsg1(res);
                errmsg = Marshal.PtrToStringAnsi(pErr); 
            }
            
            return res;     //成功返回0
        }






        [DllImport("NFCReader.dll")]
        public static extern int Init1();

        [DllImport("NFCReader.dll")]
        public static extern int Connect1();

        [DllImport("NFCReader.dll")]
        public static extern IntPtr GetUID1();

        [DllImport("NFCReader.dll")]
        public static extern IntPtr GetScardErrMsg1(int errcode);

    }
}
