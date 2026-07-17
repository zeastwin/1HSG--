using System;
using System.Drawing;
using System.Threading;


using System.Text;
using static NsDemo.OpDlg;

namespace NsDemo.Utility
{
    
    public class GlobalEvent
    {
        public delegate void ShowMsgHandler(string msg, Level obj);
        public static event Action<string, string, Color> ShowLogEvent;

        public static event ShowMsgHandler ShowMsgH;//事件

        private static object LogLock = new object();

        private static LogWriter _Log = new LogWriter()
        {
            IsEveryClose = true,
            BaseDirectory = @"D:\\Debug\\Log\\",
            FileName = "sample",
            Encoder = Encoding.Default,
            Title = String.Empty,
            SubDirFormat = "yyyy-MM-dd",
            FileType = "DebugLog.txt",
            RollingFileType = RollingFileType.EVENT_DAY_ONCE,
            IsDirectoryDate = true,
        };

        public static void ShowMsg(string msg, Level obj)
        {
            ShowMsgH?.Invoke(msg, obj);
        }
        public static void ShowLog(string strTitle,string Info, Color foreColor)
        {
            lock(LogLock)
            {
                if (ShowLogEvent != null)
                {
                    ShowLogEvent.BeginInvoke(strTitle, Info, foreColor,null,null);
                    _Log.WriteLine(DateTime.Now.ToString() + " " + strTitle + " " + Info);
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        LogHelper.Debug(strTitle + " " + Info);
                    });
                }
            }
            
        }

        public static void ShowLog(string strTitle, string Info)
        {
            ShowLog(strTitle,Info, Color.Green);
        }

    }
}