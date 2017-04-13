using System;

namespace AorBaseUtility
{
    /// <summary>
    /// 控制台打印信息
    /// </summary>
    public class ConsoleLogger : ILoggerUtility
    {
        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Warning(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            Console.WriteLine(message);
        }
    }
}
