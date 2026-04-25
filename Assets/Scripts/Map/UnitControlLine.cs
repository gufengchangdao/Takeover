using UnityEngine;

namespace Takeover
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LineRenderer))]
    public class UnitControlLine : MonoBehaviour
    {
        [SerializeField] private float controlPointHeight = 3f;
        [SerializeField] private int resolution = 30;

        private LineRenderer lineRenderer;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void Draw(Vector2 start, Vector2 end)
        {
            // 计算控制点
            Vector3 p1 = (start + end) / 2;
            p1.y += controlPointHeight;
            lineRenderer.positionCount = resolution;
            for (int i = 0; i < resolution; i++)
            {
                float t = i / (float)(resolution - 1);
                Vector3 point = CalculateQuadraticBezierPoint(t, start, p1, end);
                lineRenderer.SetPosition(i, point);
            }
        }

        public void Clear()
        {
            lineRenderer.positionCount = 0;
        }

        private static Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0;           // (1-t)² * P0
            p += 2 * u * t * p1;           // 2(1-t)t * P1
            p += tt * p2;                  // t² * P2

            return p;
        }
    }

}