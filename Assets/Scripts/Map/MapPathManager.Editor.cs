#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Takeover
{
    public partial class MapPathManager
    {
        private bool _drawTriggerRange = false;
        private bool _drawAdjoinNodeArrow = true;

        void OnDrawGizmosSelected()
        {
            // 响应鼠标的有效范围
            if (_drawTriggerRange)
            {
                Gizmos.color = Color.blue;
                foreach (Transform child in transform)
                    Gizmos.DrawWireSphere(child.position, nodeTriggerRange);
            }

            // 绘制指向邻接顶点的箭头
            if (_drawAdjoinNodeArrow)
                for (int i = 0; i < nodeDatas.Length; i++)
                {
                    var nodeData = nodeDatas[i];
                    foreach (var adjoinNodeIndex in nodeData.adjoinNodes)
                    {
                        var from = nodeData.node.position;
                        if (adjoinNodeIndex >= nodeDatas.Length)
                        {
                            Debug.LogError($"节点{i}的邻接顶点{adjoinNodeIndex}索引超出范围");
                            return;
                        }
                        var to = nodeDatas[adjoinNodeIndex].node.position;
                        var dir = (to - from).normalized;
                        if (dir == Vector3.zero)
                        {
                            Debug.LogError($"相邻节点和节点是同一个: {i},{adjoinNodeIndex}");
                            continue;
                        }
                        UnityEditor.Handles.color = Color.red;
                        float arrowSize = 0.3f;
                        Vector3 arrowPos = to - dir * (arrowSize * 0.6f);
                        Quaternion arrowRot = Quaternion.LookRotation(dir, Vector3.up);
                        UnityEditor.Handles.ArrowHandleCap(0, from, arrowRot, arrowSize, EventType.Repaint);
                    }
                }
        }

        // 编辑器下生成节点数据
        private void TrySpawnNodeData(float splineDistance = 1.5f)
        {
            Castle[] castles = FindObjectsOfType<Castle>();
            List<Transform> nodeTransforms = new(transform.childCount + castles.Length);
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                    nodeTransforms.Add(child);
            }
            foreach (var castle in castles)
                nodeTransforms.Add(castle.transform);

            nodeDatas = new NodeData[nodeTransforms.Count];
            for (int i = 0; i < nodeTransforms.Count; i++)
            {
                var transform = nodeTransforms[i];
                var node = new NodeData();
                nodeDatas[i] = node;
                node.node = transform;
                var pos = transform.position;
                for (int j = 0; j < nodeTransforms.Count; j++)
                {
                    var trans = nodeTransforms[j];
                    if (i != j
                    && Vector2.Distance(pos, trans.position) <= splineDistance)
                    {
                        if (nodeTransforms.Contains(trans))
                            trans.name = $"Node{j}";
                        node.adjoinNodes.Add(j);
                    }
                }
            }
        }
    }
}
#endif