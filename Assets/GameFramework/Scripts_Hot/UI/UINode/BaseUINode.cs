using UnityEngine;

namespace GameFramework.Hot
{
    /// <summary>
    /// UI节点
    /// 1. 用于MVC的view继承和各种自定义节点的继承，封装一些方法，简化管理
    /// 2. 提供回收机制
    /// </summary>
    public abstract class BaseUINode : MonoBehaviour
    {
        public int SourceId { get; set; }
        public bool Visible
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public Vector3 localPosition
        {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }

        public virtual void OnInit()
        {
        }

        public virtual void OnRecycle()
        {
        }
    }
}