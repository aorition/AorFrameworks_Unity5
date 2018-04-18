using System;
using System.Collections.Generic;
using AorBaseUtility;

namespace Framework
{

    public class UnityLogger : ILoggerUtility
    {
        public void Debug(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public void Info(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Warning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
    }

    public class Log
    {

        public new static void Debug(params string[] message)
        {
            if(! AorBaseUtility.Log.isInit) AorBaseUtility.Log.Init(new UnityLogger());
            AorBaseUtility.Log.Debug(message);
        }

        /// <summary>
        /// info向下兼容debug
        /// </summary>
        /// <param name="message"></param>
        public new static void Info(params string[] message)
        {
            if (!AorBaseUtility.Log.isInit) AorBaseUtility.Log.Init(new UnityLogger());
            AorBaseUtility.Log.Info(message);
        }

        /// <summary>
        /// warning向下兼容info
        /// </summary>
        /// <param name="message"></param>
        public new static void Warning(params string[] message)
        {
            if (!AorBaseUtility.Log.isInit) AorBaseUtility.Log.Init(new UnityLogger());
            AorBaseUtility.Log.Info(message);
        }

        /// <summary>
        /// warning向下兼容info
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public new static void Warning(Exception ex, params string[] message)
        {
            if (!AorBaseUtility.Log.isInit) AorBaseUtility.Log.Init(new UnityLogger());
            AorBaseUtility.Log.Warning(message);
        }

        /// <summary>
        /// error向下兼容warning
        /// </summary>
        /// <param name="message"></param>
        public new static void Error(params string[] message)
        {
            if (!AorBaseUtility.Log.isInit) AorBaseUtility.Log.Init(new UnityLogger());
            AorBaseUtility.Log.Error(message);
        }

        /// <summary>
        /// error向下兼容warning
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public new static void Error(Exception ex, params string[] message)
        {
            if (!AorBaseUtility.Log.isInit) AorBaseUtility.Log.Init(new UnityLogger());
            AorBaseUtility.Log.Error(ex, message);
        }

    }
}
