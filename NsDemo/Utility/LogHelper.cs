using System;
using log4net;

namespace NsDemo.Utility
{
    public enum LogType
    {
        /// <summary>
        /// 一般输出
        /// </summary>
        Normal,

        /// <summary>
        /// 警告
        /// </summary>
        Warning,

        /// <summary>
        /// 错误
        /// </summary>
        Error
    }

    /// <summary>
    /// 日志帮助类
    ///
    /// </summary>
    public class LogHelper
    {
        private static readonly ILog logerror = log4net.LogManager.GetLogger("SysError");

        private static readonly ILog logdebug = log4net.LogManager.GetLogger("SysDebug");

        private static readonly ILog logingo = log4net.LogManager.GetLogger("SysInfo");

        public static void Error(string content)
        {
            logerror.Error(content);
        }

        public static void Error(string content, Exception ex)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(content, ex);
            }
        }

        public static void Debug(string content)
        {
            logdebug.Debug(content);
        }

        public static void Debug(string content, Exception ex)
        {
            if (logdebug.IsDebugEnabled)
            {
                logdebug.Debug(content, ex);
            }
        }

        public static void Info(string content)
        {
            logingo.Info(content);
        }

        public static void Info(string content, Exception ex)
        {
            if (logingo.IsInfoEnabled)
            {
                logingo.Info(content, ex);
            }
        }
    }
}