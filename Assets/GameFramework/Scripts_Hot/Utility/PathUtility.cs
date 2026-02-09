using System.IO;

namespace GameFramework.Hot
{
    public static class PathUtility
    {
        /// <summary>
        /// 拼接路径，拼接符为 '/'
        /// </summary>
        public static string Combine(params string[] paths)
        {
            return Path.Combine(paths).Replace('\\', '/');
        }
    }
}