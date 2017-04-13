namespace AorBaseUtility
{
    public interface ILoggerUtility
    {
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
    }
}
