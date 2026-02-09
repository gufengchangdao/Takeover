using System.Collections.Generic;
using GameFramework.AOT;
using UnityEngine;
namespace Takeover
{
    public partial class MapVertex
    {
#if UNITY_EDITOR
        private static bool _DrawPath = true;
        private static float _VertexSize = 0.2f;
        private static float _MaxDistance = 0;
        private Vector3 _lastPosition = default;
        private List<int> _lastNeighbors;

        void OnDrawGizmos()
        {
            if (!_DrawPath)
                return;

            if (_VertexSize > 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position, _VertexSize);
            }

            Gizmos.color = Color.yellow;
            foreach (var v in neighborVertices)
                Gizmos.DrawLine(transform.position, v.transform.position);
        }

        private void _AddNextVertex()
        {
            var go = Instantiate(this, transform.parent);
            go.ResetInstantiateName(this);
            go.neighborVertices.Clear();
            go.transform.position = transform.position + Vector3.right * Mathf.Clamp(_MaxDistance, 0, 1);
            neighborVertices.Add(go);
            go.neighborVertices.Add(this);
            UnityEditor.Selection.activeGameObject = go.gameObject;
        }

        private void _OnNeighborVerticesChange()
        {
            if (_lastNeighbors == null)
            {
                _lastNeighbors = new List<int>();
                foreach (var v in neighborVertices)
                {
                    if (v == null) continue;
                    _lastNeighbors.Add(v.GetInstanceID());
                }
                return;
            }

            // 检查移除
            foreach (var id in _lastNeighbors)
            {
                var v = UnityEditor.EditorUtility.InstanceIDToObject(id) as MapVertex;
                if (v == null) continue;
                if (!neighborVertices.Contains(v))
                    v.neighborVertices.Remove(this);
            }

            // 检查新增
            _lastNeighbors.Clear();
            foreach (var v in neighborVertices)
            {
                if (v == null) continue;
                if (!v.neighborVertices.Contains(this))
                    v.neighborVertices.Add(this);
                _lastNeighbors.Add(v.GetInstanceID());
            }
        }
#endif

        private void _CheckDistance()
        {
#if UNITY_EDITOR
            if (_lastPosition == default)
            {
                _lastPosition = transform.position;
                return;
            }

            if (transform.position == _lastPosition)
                return;

            _lastPosition = transform.position;
            if (neighborVertices.Count == 0)
                return;

            if (_MaxDistance <= 0f)
                return;

            // 多个邻接点时可能存在互相“拉扯”，这里做几次迭代让结果尽量收敛
            const int Iterations = 4;
            Vector3 p = transform.position;

            for (int it = 0; it < Iterations; it++)
            {
                bool changed = false;
                foreach (var v in neighborVertices)
                {
                    if (v == null) continue;

                    Vector3 c = v.transform.position;
                    Vector3 delta = p - c;
                    float dist = delta.magnitude;

                    if (dist > _MaxDistance)
                    {
                        p = c + delta / dist * _MaxDistance;
                        changed = true;
                    }
                }

                if (!changed) break;
            }

            if (transform.position != p)
            {
                transform.position = p;
                _lastPosition = p;
            }
#endif
        }
    }
}