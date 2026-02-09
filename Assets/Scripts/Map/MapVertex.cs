using System;
using System.Collections.Generic;
using UnityEngine;

namespace Takeover
{
    /// <summary>
    /// 地图路径顶点，使用邻接链表存储
    /// </summary>
    [Serializable]
    [ExecuteAlways]
    public partial class MapVertex : MonoBehaviour
    {
        [SerializeField]
        private List<MapVertex> neighborVertices = new List<MapVertex>();

        void Update()
        {
#if UNITY_EDITOR
            _CheckDistance();
#endif
        }

        void OnDestroy()
        {
            foreach (var neighbor in neighborVertices)
                if (neighbor != null)
                    neighbor.neighborVertices.Remove(this);
            neighborVertices.Clear();
        }

        public MapVertex GetRightNeighbor()
        {
            if (neighborVertices.Count == 0)
                return null;

            float maxX = transform.position.x;
            MapVertex next = null;
            foreach (var neighbor in neighborVertices)
            {
                if (neighbor.transform.position.x > maxX)
                {
                    maxX = neighbor.transform.position.x;
                    next = neighbor;
                }
            }
            return next;
        }
    }
}