#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.AOT
{
    /// <summary>
    /// 把UI对象的raycastTarget范围绘制出来，把不需要的raycastTarget关掉，来优化游戏性能。
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class UIDebugDraw : MonoBehaviour
    {
        void Awake()
        {
#if UNITY_EDITOR
            Menu.SetChecked("工具/绘制/绘制UI的raycast边界", true);
#else
            GameObject.DestroyImmediate(gameObject);
#endif
        }

        private Vector3[] fourCorners = new Vector3[4];
        void OnDrawGizmos()
        {
            foreach (MaskableGraphic g in FindObjectsOfType<MaskableGraphic>())
            {
                if (g.raycastTarget)
                {
                    RectTransform rectTransform = g.transform as RectTransform;
                    rectTransform.GetWorldCorners(fourCorners);
                    Gizmos.color = Color.red;
                    for (int i = 0; i < 4; i++)
                        Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);
                }
            }
        }

        void OnDestroy()
        {
            Menu.SetChecked("工具/绘制/绘制UI的raycast边界", false);
        }
    }
}