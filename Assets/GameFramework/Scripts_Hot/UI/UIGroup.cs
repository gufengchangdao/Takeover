using System.Collections.Generic;

namespace GameFramework.Hot
{
    public class UIGroup
    {
        public static List<string> GROUPS = new List<string>() {
            SCENE,
            FILTER,
            MAIN,
            LOADING,
            NOTIFY,
            REVERSO
        };

        /// <summary>
        /// 和场景相关联的内容，例如跳字，血条
        /// </summary>
        public const string SCENE = "SCENE";
        /// <summary>
        /// 滤镜层
        /// </summary>
        public const string FILTER = "FILTER";
        /// <summary>
        /// 窗口层
        /// </summary>
        public const string MAIN = "MAIN";
        /// <summary>
        /// 加载层
        /// </summary>
        public const string LOADING = "LOADING";
        /// <summary>
        /// 服务消息跑马灯、提示
        /// </summary>
        public const string NOTIFY = "NOTIFY";
        /// <summary>
        /// 水印
        /// </summary>
        public const string REVERSO = "REVERSO";
    }
}