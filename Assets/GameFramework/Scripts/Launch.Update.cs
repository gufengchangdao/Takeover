using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.AOT
{
    /// <summary>
    /// 自定义Update方法的好处：
    /// 1. 可以给Update添加优先级，还可以在CPU高的时候跳过一些低优先级的update
    /// 2. 避免直接使用MonoBehavior的Update，避免本地-托管的桥接，减少性能开销
    /// </summary>
    public partial class Launch
    {

        private List<IUpdateable> updateableObjects = new();

        public static void RegisterUpdateableObject(IUpdateable obj)
        {
            if (!Instance)
                return; //有可能Launch没有实例化，有可能游戏正在退出Launch被销毁
            if (!Instance.updateableObjects.Contains(obj))
                Instance.updateableObjects.Add(obj);
        }

        public static void DeregisterUpdateableObject(IUpdateable obj)
        {
            if (!Instance)
                return;
            Instance.updateableObjects.Remove(obj);
        }

        void Update()
        {
            float dt = Time.deltaTime;
            for (int i = 0; i < updateableObjects.Count; i++)
                updateableObjects[i].OnUpdate(dt);
        }

        void LateUpdate()
        {
            float dt = Time.deltaTime;
            for (int i = 0; i < updateableObjects.Count; i++)
                updateableObjects[i].OnLateUpdate(dt);
        }
    }


    public interface IUpdateable
    {
        void OnUpdate(float dt);
        void OnLateUpdate(float dt);
    }

    public abstract class UpdateableComponent : MonoBehaviour, IUpdateable
    {
        protected virtual void Start()
        {
            Launch.RegisterUpdateableObject(this);
        }

        public virtual void OnUpdate(float dt) { }

        public virtual void OnLateUpdate(float dt) { }

        protected virtual void OnDestroy()
        {
            Launch.DeregisterUpdateableObject(this);
        }
    }
}