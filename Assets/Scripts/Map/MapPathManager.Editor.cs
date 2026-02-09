#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

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

        public void TrySpawnNodeData()
        {
            SplineContainer splineContainer = GetComponent<SplineContainer>();
            nodeDatas = new NodeData[transform.childCount];
            Dictionary<int, List<(int, float)>> splineNodes = new();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var nodeData = new NodeData();
                nodeData.node = child;
                child.gameObject.name = $"Node_{i}";

                // 查找所属曲线
                for (int j = 0; j < splineContainer.Splines.Count; j++)
                {
                    var nativeSpline = new NativeSpline(splineContainer.Splines[j], splineContainer.transform.localToWorldMatrix); //转换为曲线本地坐标
                    var distance = SplineUtility.GetNearestPoint(nativeSpline, (float3)child.position, out float3 nearest, out var t);
                    if (distance < SplineDistance)
                    {
                        // 在这个曲线上
                        nodeData.splines.Add(j);
                        nodeData.ts.Add(t);

                        if (!splineNodes.TryGetValue(j, out var nodes))
                        {
                            nodes = new List<(int, float)>();
                            splineNodes[j] = nodes;
                        }
                        nodes.Add((i, t));
                    }
                }
                nodeDatas[i] = nodeData;
            }


            foreach (var nodes in splineNodes.Values)
            {
                // 对每条曲线上的t进行排序
                nodes.Sort((a, b) => a.Item2.CompareTo(b.Item2));

                // 查找相邻节点
                for (int i = 0; i < nodes.Count; i++)
                {
                    int index = nodes[i].Item1;
                    if (i > 0)
                    {
                        int lastIndex = nodes[i - 1].Item1;
                        nodeDatas[index].adjoinNodes.Add(lastIndex);
                    }
                    if (i < nodes.Count - 1)
                    {
                        int nextIndex = nodes[i + 1].Item1;
                        nodeDatas[index].adjoinNodes.Add(nextIndex);
                    }
                }
            }
        }

        private string _nodeDataErrorMessage = "";

        private void _CheckNodeValid()
        {
            _nodeDataErrorMessage = "";
            for (int i = 0; i < nodeDatas.Length; i++)
            {
                var nodeData = nodeDatas[i];
                if (nodeData.node == null)
                {
                    _nodeDataErrorMessage = $"节点 {i} 对象丢失";
                    return;
                }
            }
        }
    }
}
#endif