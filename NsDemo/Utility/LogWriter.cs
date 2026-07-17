using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NsDemo.Utility
{
    /// <summary>
    /// 文件记录点的类型
    /// </summary>
    public enum RollingFileType
    {
        /// <summary>
        /// 每天记录一个文件,时间节点为12点
        /// </summary>
        EVENT_DAY_ONCE,

        /// <summary>
        /// 每天记录两次文件,分别为当天的早08点和晚20点
        /// </summary>
        EVENT_DAY_TWICE,

        /// <summary>
        /// 暂时没有需求
        /// </summary>
        EVENT_DAY_THRICE
    }


    /// <summary>
    /// 按照时间节点记录生产中的数据日志
    /// </summary>
    public class LogWriter : IDisposable
    {
        public LogWriter()
        {
            IsEveryClose = true;
            BaseDirectory = @"d:\Data\";
            FileName = "sample";
            Encoder = Encoding.Default;
            Title = string.Empty;
            SubDirFormat = "yyyy-MM-dd";
            FileType = ".csv";
            RollingFileType = RollingFileType.EVENT_DAY_TWICE;
            IsDirectoryDate = true;
        }

        private DateTime _nextCheck;

        private StreamWriter _writer;

        private string _currentExFileName;

        public RollingFileType RollingFileType;

        /// <summary>
        /// 基目录,默认
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// 文件名,默认"sample"
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件编码,默认
        /// </summary>
        public Encoding Encoder { get; set; }

        /// <summary>
        /// 当文件为空时写入的内容,需换行请自行追加换行符
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 保存期限,大于0时将删除大于此天数的记录
        /// </summary>
        public int StorageLife { get; set; }

        /// <summary>
        /// 日期目录格式,默认"yyyy-MM-dd"
        /// </summary>
        public string SubDirFormat { get; set; }

        /// <summary>
        /// 简单标识一个文件类型,例如:csv,txt
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 是否执行每写一次数据关闭流一次
        /// </summary>
        public bool IsEveryClose { get; set; }

        public string FilePath { get; set; }

        public bool IsDirectoryDate { get; set; }
        /// <summary>
        /// 内部维护的写入流
        /// </summary>
        public StreamWriter Writer
        {
            get
            {
                if (_nextCheck <= DateTime.Now || _writer == null)
                {
                    if ((_nextCheck <= DateTime.Now || RollingFileType == RollingFileType.EVENT_DAY_ONCE) && IsDirectoryDate)
                    {
                        FileName = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                    }

                    if (_writer != null)
                        _writer.Close();
                    DeleteLog(StorageLife);
                    if (IsDirectoryDate)
                    {
                        FilePath = Path.Combine(BaseDirectory, DateTime.Now.Date.ToString(SubDirFormat));
                    }
                    else
                        FilePath = Path.Combine(BaseDirectory);
                    _nextCheck = NextCheckRolling(DateTime.Now, RollingFileType);
                    //var path = Path.Combine(BaseDirectory, DateTime.Now.Date.ToString(SubDirFormat));

                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }

                    var fileUri = Path.Combine(FilePath, FileName + _currentExFileName + FileType);
                    var isFileExist = File.Exists(fileUri);
                    _writer = new StreamWriter(fileUri, true, Encoder)
                    {
                        AutoFlush = true
                    };
                    if (!isFileExist)
                    {
                        _writer.Write(Title);
                    }
                }
                return _writer;
            }
        }

        /// <summary>
        /// 指定日期执行删除动作
        /// </summary>
        /// <param name="storageLife"></param>
        /// <returns></returns>
        public int DeleteLog(int storageLife)
        {
            return DeleteLog(BaseDirectory, SubDirFormat, storageLife);
        }

        /// <summary>
        /// 用于检查下一个时间节点的计算
        /// </summary>
        /// <param name="currentDateTime"></param>
        /// <param name="rollPoint"></param>
        /// <returns></returns>
        private DateTime NextCheckRolling(DateTime currentDateTime, RollingFileType rollPoint)
        {
            var current = currentDateTime;
            switch (rollPoint)
            {
                case RollingFileType.EVENT_DAY_ONCE:
                    current = current.AddDays(1);
                    break;

                case RollingFileType.EVENT_DAY_TWICE:
                    current = current.AddMilliseconds(-current.Millisecond);
                    current = current.AddSeconds(-current.Second);
                    current = current.AddMinutes(-current.Minute);

                    int currentHour = current.Hour;
                    //if (currentHour < 20)
                    if (currentHour >= 8 && currentHour < 20)
                    {
                        current = current.AddHours(20 - current.Hour);
                        _currentExFileName = "-白班";
                        //FilePath = Path.Combine(BaseDirectory, DateTime.Now.Date.ToString(SubDirFormat));
                    }
                    else
                    {
                        var pathDate = current.AddDays(-1);
                        if (currentHour >= 0 && currentHour < 8)
                        {
                           // FilePath = Path.Combine(BaseDirectory, pathDate.Date.ToString(SubDirFormat));
                            FileName = string.Format("{0:yyyy-MM-dd}", pathDate);
                        }
                        current = current.AddHours(8 - current.Hour);
                        //current = current.AddDays(1);
                        _currentExFileName = "-晚班";

                    }
                    break;

                case RollingFileType.EVENT_DAY_THRICE:
                    break;
            }

            return current;
        }

        /// <summary>
        /// 执行删除文件的操作
        /// </summary>
        /// <param name="baseDir"></param>
        /// <param name="dateFormat"></param>
        /// <param name="storageLife"></param>
        /// <returns></returns>
        private static int DeleteLog(string baseDir, string dateFormat, int storageLife)
        {
            if (storageLife <= 0 || !Directory.Exists(baseDir))
                return 0;
            int count = 0;
            var lastDay = DateTime.Now.Date.AddDays(-storageLife);
            foreach (var dir in Directory.GetDirectories(baseDir))
            {
                DateTime time;
                if (DateTime.TryParseExact(Path.GetFileName(dir), dateFormat, null,
                        System.Globalization.DateTimeStyles.None, out time) && time < lastDay)
                {
                    Directory.Delete(dir, true);
                    ++count;
                }
            }

            return count;
        }

        /// <summary>
        /// 写入到默认没有换行的数据到流中
        /// </summary>
        /// <param name="content"></param>
        public void Write(string content)
        {
            Writer.Write(content);
            if (IsEveryClose)
            {
                _writer.Close();
                _writer = null;
            }
        }

        /// <summary>
        /// 写入默认换行的数据到流中
        /// </summary>
        /// <param name="content"></param>
        public void WriteLine(string content)
        {
            Writer.WriteLine(content);
            if (IsEveryClose)
            {
                _writer.Close();
                _writer = null;
            }
        }

        /// <summary>
        /// 写入默认带逗号分隔的数组数据到流中
        /// </summary>
        /// <param name="data"></param>
        public void WriteCsvLine(params object[] data)
        {
            if (data == null || data.Length == 0)
            {
                Writer.WriteLine();
                if (IsEveryClose)
                {
                    _writer.Close();
                    _writer = null;
                }
                return;
            }

            var sb = new StringBuilder();
            foreach (var datum in data)
            {
                sb.Append(datum).Append(',');
            }

            Writer.WriteLine(sb.ToString());
            if (IsEveryClose)
            {
                _writer.Close();
                _writer = null;
            }
        }

        //读取文件内容
        //public string ReadFile()
        //{
        //    string content = "";

        //    return content;
        //}

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }
        }
    }
}

//1.IsEveryClose不同情况下，文件正在用程序写入，外部打开程序会出现什么情况