using System;

namespace GameFramework.Hot
{
    public abstract class FsmBase
    {
        /// <summary>
        /// 获取有限状态机名称。
        /// </summary>
        public string Name { get; protected set; } = string.Empty;

        public abstract Type OwnerType { get; }

        /// <summary>
        /// 关闭并清理有限状态机。
        /// </summary>
        internal abstract void Shutdown();

        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">当前已流逝时间，以秒为单位。</param>
        public abstract void Update(float elapseSeconds, float realElapseSeconds);
    }
}