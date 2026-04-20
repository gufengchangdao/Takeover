using System;
using System.Diagnostics;

namespace GameFramework.AOT
{
    public static class Log
    {
        private static string FormatMessage(string level, object message)
        {
            string text = message?.ToString() ?? "null";
#if UNITY_EDITOR
            return text;
#else
            return string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}][{1}] {2}", DateTime.Now, level, text);
#endif
        }

        private static string FormatErrorMessage(object message)
        {
            string text = FormatMessage("ERROR", message);
#if UNITY_EDITOR
            return text;
#else
            if (message is Exception)
                return text;

            string stackTrace = new StackTrace(2, true).ToString();
            if (string.IsNullOrWhiteSpace(stackTrace))
                return text;

            return string.Format("{0}\n{1}", text, stackTrace);
#endif
        }

        public static void Debug(object message)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(string.Format("<color=#888888>{0}</color>", FormatMessage("DEBUG", message)));
#else
            UnityEngine.Debug.Log(FormatMessage("DEBUG", message));
#endif
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
            UnityEngine.Debug.Log(FormatMessage("INFO", message));
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
            UnityEngine.Debug.LogWarning(FormatMessage("WARN", message));
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
            UnityEngine.Debug.LogError(FormatErrorMessage(message));
        }

        public static void Error(string format, object arg0)
        {
            UnityEngine.Debug.LogError(FormatErrorMessage(string.Format(format, arg0)));
        }

        public static void Error(string format, object arg0, object arg1)
        {
            UnityEngine.Debug.LogError(FormatErrorMessage(string.Format(format, arg0, arg1)));
        }

        public static void Error(string format, object arg0, object arg1, object arg2)
        {
            UnityEngine.Debug.LogError(FormatErrorMessage(string.Format(format, arg0, arg1, arg2)));
        }

        public static void Error(string format, object arg0, object arg1, object arg2, object arg3)
        {
            UnityEngine.Debug.LogError(FormatErrorMessage(string.Format(format, arg0, arg1, arg2, arg3)));
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
