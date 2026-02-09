using System;
using UnityEngine;

namespace GameFramework.Hot
{
    [DisallowMultipleComponent]
    public abstract class GFBaseModule : MonoBehaviour
    {
        /// <summary>
        /// 使用ModuleUpdate来保证模块之间更新有序
        /// </summary>
        [Obsolete("请使用ModuleUpdate方法代替Update方法", true)]
        protected virtual void Update() { }

        /// <summary>
        /// 模块更新，用这个代替Update
        /// </summary>
        public virtual void ModuleUpdate() { }
    }
}