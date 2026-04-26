using System;
using System.Collections.Generic;
using UnityEngine;

namespace Takeover
{
    public partial class MapPathManager : MonoBehaviour
    {
        [SerializeField] private float nodeTriggerRange = 0.26f;

        [SerializeField]
        private NodeData[] nodeDatas;

        void Awake()
        {
            foreach (var nodeData in nodeDatas)
            {
                if (nodeData.node.parent == transform)
                {
                    nodeData.node.gameObject.SetActive(false);
                    nodeData.isPathNode = true;
                }
            }

            gameObject.GFGetOrAddComponent<ArmyController>();
        }


        /// <summary>
        /// 如果这个节点是城堡的话返回城堡组件
        /// </summary>
        public Castle GetCastleByNodeIndex(int nodeIndex)
        {
            if (!nodeDatas[nodeIndex].isPathNode)
                return nodeDatas[nodeIndex].node.GetComponent<Castle>();
            return null;
        }

        public Vector2 GetNodePosition(int nodeIndex)
        {
            return nodeDatas[nodeIndex].node.position;
        }

        public int GetNodeIndex(Castle castle)
        {
            return GetNodeIndex(castle.transform);
        }

        public int GetNodeIndex(Transform transform)
        {
            for (int i = 0; i < nodeDatas.Length; i++)
            {
                if (nodeDatas[i].node == transform)
                    return i;
            }
            return -1;
        }

        public int GetMouseRangeNodeIndex(Vector2 pos)
        {
            // 路径点
            int nodeIndex = GetInRangeNodeIndex(pos);
            if (nodeIndex >= 0)
                return nodeIndex;

            // 城堡
            var hit = Physics2D.OverlapPoint(pos);
            if (hit != null && hit.CompareTag(GameObjectTag.Castle))
                return GetNodeIndex(hit.gameObject.transform);

            return -1;
        }

        public Transform GeInRangeNodeTransform(Vector2 pos)
        {
            int nodeIndex = GetInRangeNodeIndex(pos);
            if (nodeIndex == -1)
                return null;
            return nodeDatas[nodeIndex].node;
        }

        public int GetInRangeNodeIndex(Vector2 pos)
        {
            for (int i = 0; i < nodeDatas.Length; i++)
                if (Vector2.Distance(pos, nodeDatas[i].node.position) < nodeTriggerRange)
                    return i;
            return -1;
        }

        public int GetClosestNodeIndex(Vector2 pos)
        {
            int closestIndex = -1;
            float closestDistance = float.MaxValue;
            for (int i = 0; i < nodeDatas.Length; i++)
            {
                float distance = Vector2.Distance(pos, nodeDatas[i].node.position);
                if (distance < closestDistance)
                {
                    closestIndex = i;
                    closestDistance = distance;
                }
            }
            return closestIndex;
        }

        private List<int> findPathNodeList = new();

        // 计算从给定位置到指定位置经过的节点列表
        private List<int> UpdatePathNodeList(Vector2 curPos, Vector2 targetPos)
        {
            int beginNodeIndex = GetClosestNodeIndex(curPos);
            int endNodeIndex = GetClosestNodeIndex(targetPos);
            var list = UpdatePathNodeList(beginNodeIndex, endNodeIndex);

            // 检查第一个节点是不是最优的，有没有往回走
            if (list.Count >= 2)
            {
                var pos0 = nodeDatas[list[0]].node.position;
                var pos1 = nodeDatas[list[1]].node.position;
                if (Vector3.Distance(curPos, pos1) < Vector3.Distance(pos0, pos1))
                    list.RemoveAt(0);
            }

            return list;
        }

        private List<int> UpdatePathNodeList(int beginNodeIndex, int endNodeIndex)
        {
            findPathNodeList.Clear();

            int n = nodeDatas.Length;
            if (n == 0 || beginNodeIndex < 0 || endNodeIndex < 0 || beginNodeIndex >= n || endNodeIndex >= n)
                return findPathNodeList;

            if (beginNodeIndex == endNodeIndex)
            {
                findPathNodeList.Add(endNodeIndex); //起点和终点一样，返回终点
                return findPathNodeList;
            }

            float[] dist = new float[n];
            int[] prev = new int[n];
            bool[] done = new bool[n];

            for (int i = 0; i < n; i++)
            {
                dist[i] = float.MaxValue;
                prev[i] = -1;
            }
            dist[beginNodeIndex] = 0f;

            for (int step = 0; step < n; step++)
            {
                // 1) 选出当前未确定且 dist 最小的节点 u
                int u = -1;
                float best = float.MaxValue;
                for (int i = 0; i < n; i++)
                {
                    if (!done[i] && dist[i] < best)
                    {
                        best = dist[i];
                        u = i;
                    }
                }

                // 剩下节点都不可达
                if (u == -1)
                    break;

                // 提前结束
                if (u == endNodeIndex)
                    break;

                done[u] = true;

                Vector2 uPos = nodeDatas[u].node.position;
                var adjoinNodes = nodeDatas[u].adjoinNodes;

                // 2) 松弛相邻边
                for (int i = 0; i < adjoinNodes.Count; i++)
                {
                    int v = adjoinNodes[i];
                    if (v < 0 || v >= n || done[v]) continue;

                    float alt = dist[u] + Vector2.Distance(uPos, nodeDatas[v].node.position);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }

            // 3) 不可达保护
            if (prev[endNodeIndex] == -1)
            {
                findPathNodeList.Add(endNodeIndex);
                return findPathNodeList;
            }

            // 4) 回溯路径
            int cur = endNodeIndex;
            while (cur != -1)
            {
                findPathNodeList.Add(cur);
                if (cur == beginNodeIndex) break;
                cur = prev[cur];
            }

            // 正常情况下应回到起点；否则视为失败
            if (findPathNodeList[findPathNodeList.Count - 1] != beginNodeIndex)
            {
                findPathNodeList.Clear();
                findPathNodeList.Add(endNodeIndex);
                return findPathNodeList;
            }

            findPathNodeList.Reverse();
            return findPathNodeList;
        }

        /// <summary>
        /// 返回单位从指定位置到目标位置的路径节点列表，注意列表是倒叙的
        /// </summary>
        public List<int> GetPathNodeList(Vector2 curPos, Vector2 targetPos)
        {
            var list = UpdatePathNodeList(curPos, targetPos);
            list = new(list);
            list.Reverse();
            return list;
        }

        /// <summary>
        /// 根据起点和鼠标位置更新路径节点显隐，查找一条从起点到目标点最近的路径节点并显示
        /// </summary>
        /// <param name="targetPos"></param>
        public void UpdatePathNodeVisible(Vector2 curPos, Vector2 targetPos)
        {
            HideAllPathNode();

            var list = UpdatePathNodeList(curPos, targetPos);
            foreach (var nodeIndex in list)
            {
                var node = nodeDatas[nodeIndex];
                if (node.isPathNode)
                    node.node.gameObject.SetActive(true);   // 显示经过的节点
            }
        }

        public void HideAllPathNode()
        {
            for (int i = 0; i < nodeDatas.Length; i++)
                if (nodeDatas[i].isPathNode)
                    nodeDatas[i].node.gameObject.SetActive(false);
        }

        [Serializable]
        public class NodeData
        {
            public Transform node;
            public List<int> adjoinNodes = new();
            [HideInInspector] public bool isPathNode;
        }
    }
}