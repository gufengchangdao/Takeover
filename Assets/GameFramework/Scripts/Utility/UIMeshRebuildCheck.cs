using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.AOT
{
    /// <summary>
    /// 检查网格重建的UI
    /// </summary>
    public class UIMeshRebuildCheck : MonoBehaviour
    {
        private IList<ICanvasElement> m_LayoutRebuildQueue;

        private IList<ICanvasElement> m_GraphicRebuildQueue;

        void Awake()
        {
            System.Type type = typeof(CanvasUpdateRegistry);

            FieldInfo field = type.GetField("m_LayoutRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);
            m_LayoutRebuildQueue = field.GetValue(CanvasUpdateRegistry.instance) as IList<ICanvasElement>;

            field = type.GetField("m_GraphicRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);
            m_GraphicRebuildQueue = field.GetValue(CanvasUpdateRegistry.instance) as IList<ICanvasElement>;
        }

        void Update()
        {
            for (int i = 0; i < m_LayoutRebuildQueue.Count; i++)
            {
                var rebuild = m_LayoutRebuildQueue[i];
                if (ObjectValidForUpdate(rebuild))
                {
                    Debug.LogFormat("{0}引起{1}网格重建", rebuild.transform.name, rebuild.transform.GetComponent<Graphic>().canvas.name);
                }
            }

            for (int i = 0; i < m_GraphicRebuildQueue.Count; i++)
            {
                var element = m_GraphicRebuildQueue[i];
                if (ObjectValidForUpdate(element))
                {
                    Debug.LogFormat("{0}引起{1}网格重建", element.transform.name, element.transform.GetComponent<Graphic>().canvas.name);
                }
            }
        }

        private bool ObjectValidForUpdate(ICanvasElement element)
        {
            var valid = element != null;
            var isUnityObject = element is Object;
            if (isUnityObject)
                valid = (element as Object) != null;
            return valid;
        }
    }
}