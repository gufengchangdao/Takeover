using System;
using System.Diagnostics;
using YooAsset;

namespace GameFramework.AOT
{

    public static class Log
    {
        public static void Debug(object message)
        {
            UnityEngine.Debug.Log(string.Format("<color=#888888>{0}</color>", message));
        }

        public static void Debug(string format, object arg0)
        {
            Debug(string.Format(format, arg0));
        }

        public static void Debug(string format, object arg0, object arg1)
        {
            Debug(string.Format(format, arg0, arg1));
        }

        public static void Debug(string format, object arg0, object arg1, object arg2)
        {
            Debug(string.Format(format, arg0, arg1, arg2));
        }

        public static void Debug(string format, object arg0, object arg1, object arg2, object arg3)
        {
            Debug(string.Format(format, arg0, arg1, arg2, arg3));
        }

        public static void Info(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void Info(string format, object arg0)
        {
            Info(string.Format(format, arg0));
        }

        public static void Info(string format, object arg0, object arg1)
        {
            Info(string.Format(format, arg0, arg1));
        }

        public static void Info(string format, object arg0, object arg1, object arg2)
        {
            Info(string.Format(format, arg0, arg1, arg2));
        }

        public static void Info(string format, object arg0, object arg1, object arg2, object arg3)
        {
            Info(string.Format(format, arg0, arg1, arg2, arg3));
        }

        public static void Warning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        public static void Warning(string format, object arg0)
        {
            Warning(string.Format(format, arg0));
        }

        public static void Warning(string format, object arg0, object arg1)
        {
            Warning(string.Format(format, arg0, arg1));
        }

        public static void Warning(string format, object arg0, object arg1, object arg2)
        {
            Warning(string.Format(format, arg0, arg1, arg2));
        }

        public static void Warning(string format, object arg0, object arg1, object arg2, object arg3)
        {
            Warning(string.Format(format, arg0, arg1, arg2, arg3));
        }

        public static void Error(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public static void Error(string format, object arg0)
        {
            Error(string.Format(format, arg0));
        }

        public static void Error(string format, object arg0, object arg1)
        {
            Error(string.Format(format, arg0, arg1));
        }

        public static void Error(string format, object arg0, object arg1, object arg2)
        {
            Error(string.Format(format, arg0, arg1, arg2));
        }

        public static void Error(string format, object arg0, object arg1, object arg2, object arg3)
        {
            Error(string.Format(format, arg0, arg1, arg2, arg3));
        }
    }

    public class ILog : YooAsset.ILogger
    {
        public void Error(string message)
        {
            AOT.Log.Error(message);
        }

        public void Exception(Exception exception)
        {
            AOT.Log.Error(exception);
        }

        public void Log(string message)
        {
            AOT.Log.Info(message);
        }

        public void Warning(string message)
        {
            AOT.Log.Warning(message);
        }
    }
}