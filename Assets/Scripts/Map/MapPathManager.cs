using System;
using System.Collections.Generic;
using GameFramework.Hot;
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
            foreach (Transform child in transform)
                child.gameObject.SetActive(false);

            gameObject.GFGetOrAddComponent<UnitController>();
        }

        public Transform GeInRangeUnit(Vector2 pos)
        {
            // 城堡
            var hit = Physics2D.OverlapPoint(pos);
            if (hit != null && hit.CompareTag(GameObjectTag.Castle))
                return hit.gameObject.transform;

            // 路径点
            int nodeIndex = GetInRangeNodeIndex(pos);
            if (nodeIndex >= 0)
                return nodeDatas[nodeIndex].node;

            return null;
        }

        private int GetInRangeNodeIndex(Vector2 pos)
        {
            for (int i = 0; i < nodeDatas.Length; i++)
                if (Vector2.Distance(pos, nodeDatas[i].node.position) < nodeTriggerRange)
                    return i;
            return -1;
        }


        private HashSet<int> findPathVisited = new();
        private Dictionary<int, float> findPathNodeDistances = new();
        private Queue<int> findPathQueue = new();
        private Dictionary<int, int> findPathNodePaths = new();
        private List<int> findPathNodeList = new();

        // 计算从给定位置到指定位置经过的节点列表
        public List<int> UpdatePathNodeList(Vector2 curPos, Vector2 targetPos)
        {
            int beginNodeIndex = GetClosestNodeIndex(curPos);
            int endNodeIndex = GetClosestNodeIndex(targetPos);
            var list = UpdatePathNodeList(beginNodeIndex, endNodeIndex);

            if (list.Count >= 2)
            {
                var pos0 = nodeDatas[list[0]].node.position;
                var pos1 = nodeDatas[list[1]].node.position;
                if (Vector3.Distance(curPos, pos1) < Vector3.Distance(pos0, pos1))
                    list.RemoveAt(0);
            }

            if (list.Count >= 2)
            {
                var pos0 = nodeDatas[list[list.Count - 1]].node.position;
                var pos1 = nodeDatas[list[list.Count - 2]].node.position;
                if (Vector3.Distance(targetPos, pos1) < Vector3.Distance(pos0, pos1))
                    list.RemoveAt(list.Count - 1);
            }

            return list;
        }

        private List<int> UpdatePathNodeList(int beginNodeIndex, int endNodeIndex)
        {
            findPathNodeList.Clear();
            findPathVisited.Clear();
            findPathNodeDistances.Clear();
            findPathNodePaths.Clear();
            findPathQueue.Clear();

            if (beginNodeIndex == endNodeIndex)
                return findPathNodeList;

            findPathQueue.Enqueue(beginNodeIndex);
            findPathVisited.Add(beginNodeIndex);

            for (int i = 0; i < nodeDatas.Length; i++)
            {
                if (i == beginNodeIndex)
                    findPathNodeDistances[i] = 0;
                else
                    findPathNodeDistances[i] = float.MaxValue;
            }

            while (findPathQueue.Count > 0)
            {
                int curIndex = findPathQueue.Dequeue();
                float curDistance = findPathNodeDistances[curIndex];
                Vector2 pos = nodeDatas[curIndex].node.position;
                var adjoinNodes = nodeDatas[curIndex].adjoinNodes;
                for (int i = 0; i < adjoinNodes.Count; i++)
                {
                    int adjoinNodeIndex = adjoinNodes[i];
                    if (findPathVisited.Contains(adjoinNodeIndex))
                        continue;

                    float distance = curDistance + Vector2.Distance(pos, nodeDatas[adjoinNodeIndex].node.position);
                    if (distance < findPathNodeDistances[adjoinNodeIndex])
                    {
                        findPathNodeDistances[adjoinNodeIndex] = distance; //更新最短距离
                        findPathNodePaths[adjoinNodeIndex] = curIndex; //更新路径
                    }

                    findPathQueue.Enqueue(adjoinNodeIndex);
                    findPathVisited.Add(adjoinNodeIndex);
                }
            }

            int targetNodeIndex = endNodeIndex;
            while (targetNodeIndex != beginNodeIndex)
            {
                findPathNodeList.Add(targetNodeIndex);
                targetNodeIndex = findPathNodePaths[targetNodeIndex];
            }
            findPathNodeList.Add(beginNodeIndex);
            findPathNodeList.Reverse();

            return findPathNodeList;
        }

        /// <summary>
        /// 根据起点和鼠标位置更新路径节点显隐，查找一条从起点到目标点最近的路径节点并显示
        /// </summary>
        /// <param name="targetPos"></param>
        public void UpdatePathNodeVisible(Vector2 curPos, Vector2 targetPos)
        {
            HideAllPathNode();




            // // 显示经过的节点
            // int targetNodeIndex = findPathTargetNodeIndex;
            // while (targetNodeIndex != findPathStartNodeIndex)
            // {
            //     nodeDatas[targetNodeIndex].node.gameObject.SetActive(true);
            //     targetNodeIndex = findPathNodePaths[targetNodeIndex];
            // }
            // nodeDatas[findPathStartNodeIndex].node.gameObject.SetActive(true);
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