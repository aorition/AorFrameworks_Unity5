using System.Collections;
using System;
using System.Diagnostics;
using System.Text;

namespace AorBaseUtility
{
    
    public class Log
    {
        public enum LogLevel : uint
        {
            All = 0xFFFFFFFF,

            Debug = LEVEL_MASK_DEBUG | LEVEL_MASK_INFO | LEVEL_MASK_WARNING | LEVEL_MASK_ERROR,
            Info = LEVEL_MASK_INFO | LEVEL_MASK_WARNING | LEVEL_MASK_ERROR,
            Warning = LEVEL_MASK_WARNING | LEVEL_MASK_ERROR,
            Error = LEVEL_MASK_ERROR,

            Off = 0x0,
        }

        private const uint LEVEL_MASK_DEBUG = 0x1;
        private const uint LEVEL_MASK_INFO = 0x2;
        private const uint LEVEL_MASK_WARNING = 0x4;
        private const uint LEVEL_MASK_ERROR = 0x8;

        private static ILoggerUtility _logger;
        public static LogLevel Level = LogLevel.Off;

        public static void Init(ILoggerUtility logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static StringBuilder HandleMessage(params string[] message)
        {
            StringBuilder builder = new StringBuilder();
            //            builder.AppendFormat("{0}ms ", Time.time*1000);

            for (int i = 0; i < message.Length; i++)
            {
                builder.Append(message[i]);
            }
            builder.Append("\n");

            StackTrace trace = new StackTrace(true);
            for (int j = 0; j < trace.FrameCount; j++)
            {
                StackFrame sf = trace.GetFrame(j);
                int fileLine = sf.GetFileLineNumber();
                string fileName = sf.GetMethod().Name;
                string filterdName = "HandleMessage,Debug,Info,Warning,Error,";
                if (fileLine.Equals(0)) continue;
                if (filterdName.Contains(fileName)) continue;
                builder.AppendFormat("at {0}.{1}", sf.GetMethod().DeclaringType.FullName, fileName);
                builder.AppendFormat("( in {0}:{1})", sf.GetFileName(), sf.GetFileLineNumber());
                builder.Append("\n");
            }

            return builder;
        }

        /// <summary>
        /// 信息+堆栈信息+异常信息
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static StringBuilder HandleMessage(Exception ex, params string[] message)
        {
            StringBuilder builder = new StringBuilder();
            //            builder.AppendFormat("{0} ", Time.time*1000);

            for (int i = 0; i < message.Length; i++)
            {
                builder.Append(message[i]);
            }
            builder.Append("\n");

            builder.AppendLine(ex.Message);
            builder.AppendLine(ex.StackTrace);

            return builder;
        }

        public static void Debug(params string[] message)
        {
            if (((uint)Level & LEVEL_MASK_DEBUG) != 0)
            {
                if (_logger != null)
                    _logger.Debug(HandleMessage(message).Insert(0, "Debug:").ToString());
            }
        }

        /// <summary>
        /// info向下兼容debug
        /// </summary>
        /// <param name="message"></param>
        public static void Info(params string[] message)
        {
            if (((uint)Level & LEVEL_MASK_INFO) != 0)
            {
                if (_logger != null)
                    _logger.Info(HandleMessage(message).Insert(0, "Info:").ToString());
            }
        }

        /// <summary>
        /// warning向下兼容info
        /// </summary>
        /// <param name="message"></param>
        public static void Warning(params string[] message)
        {
            if (((uint)Level & LEVEL_MASK_WARNING) != 0)
            {
                if (_logger != null)
                    _logger.Warning(HandleMessage(message).Insert(0, "Waining:").ToString());

            }
        }

        /// <summary>
        /// warning向下兼容info
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public static void Warning(Exception ex, params string[] message)
        {
            if (((uint)Level & LEVEL_MASK_WARNING) != 0)
            {
                if (_logger != null)
                    _logger.Warning(HandleMessage(ex, message).Insert(0, "Waining:").ToString());
            }
        }

        /// <summary>
        /// error向下兼容warning
        /// </summary>
        /// <param name="message"></param>
        public static void Error(params string[] message)
        {
            if (((uint)Level & LEVEL_MASK_ERROR) != 0)
            {
                if (_logger != null)
                    _logger.Error(HandleMessage(message).Insert(0, "Error:").ToString());
            }
        }

        /// <summary>
        /// error向下兼容warning
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public static void Error(Exception ex, params string[] message)
        {
            if (((uint)Level & LEVEL_MASK_ERROR) != 0)
            {
                if (_logger != null)
                    _logger.Error(HandleMessage(ex, message).Insert(0, "Error:").ToString());
            }
        }

        public static void Assert(bool b, object info = null)
        {
            #if DEBUG
                if (!b)
                {
                    throw new ApplicationException(info == null ? "" : info.ToString());
                }
            #endif
        }
    }
}
