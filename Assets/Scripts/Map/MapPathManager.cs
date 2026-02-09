using System;
using System.Collections.Generic;
using GameFramework.AOT;
using GameFramework.Hot;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
namespace Takeover
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SplineContainer))]
    [RequireComponent(typeof(SplineContainer))]
    public partial class MapPathManager : MonoBehaviour
    {
        public static float SplineDistance = .1f;


        [SerializeField]
        private NodeData[] nodeDatas;

        private Castle[] castles;

        [SerializeField]
        private float nodeTriggerRange = 0.26f;

        private SplineContainer splineContainer;

        void Start()
        {
            splineContainer = GetComponent<SplineContainer>();
            HideAllPathNode();

            castles = FindObjectsOfType<Castle>();
        }

        void OnDestroy()
        {
            nodeDatas = null;
        }

        public void HideAllPathNode()
        {
            for (int i = 0; i < nodeDatas.Length; i++)
                nodeDatas[i].node.gameObject.SetActive(false);
        }

        private HashSet<int> findPathVisited = new();
        private Dictionary<int, float> findPathNodeDistances = new();
        private Queue<int> findPathQueue = new();
        private Dictionary<int, int> findPathNodePaths = new();
        private int findPathStartNodeIndex;
        private int findPathTargetNodeIndex;

        /// <summary>
        /// 根据起点和鼠标位置更新路径节点显隐，查找一条从起点到目标点最近的路径节点并显示
        /// </summary>
        /// <param name="targetPos"></param>
        public void UpdatePathNodeVisible(Vector2 curPos, Vector2 targetPos)
        {
            HideAllPathNode();
            findPathTargetNodeIndex = GetClosestNodeIndex(targetPos);
            if (findPathTargetNodeIndex == -1)
                return;

            // 计算路径
            findPathStartNodeIndex = GetMoveStartNodeIndex(curPos, targetPos);
            BFS(findPathStartNodeIndex);

            // 显示经过的节点
            int targetNodeIndex = findPathTargetNodeIndex;
            while (targetNodeIndex != findPathStartNodeIndex)
            {
                nodeDatas[targetNodeIndex].node.gameObject.SetActive(true);
                targetNodeIndex = findPathNodePaths[targetNodeIndex];
            }
            nodeDatas[findPathStartNodeIndex].node.gameObject.SetActive(true);
        }

        private void BFS(int curNodeIndex)
        {
            findPathVisited.Clear();
            findPathNodeDistances.Clear();
            findPathNodePaths.Clear();
            findPathQueue.Clear();

            findPathQueue.Enqueue(curNodeIndex);
            findPathVisited.Add(curNodeIndex);

            for (int i = 0; i < nodeDatas.Length; i++)
            {
                if (i == curNodeIndex)
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
        }

        public GameObject GeInRangeUnit(Vector2 pos)
        {
            // 城堡
            var hit = Physics2D.OverlapPoint(pos);
            if (hit != null && hit.CompareTag(GameObjectTag.Castle))
                return hit.gameObject;

            // 路径点
            int nodeIndex = GetInRangeNodeIndex(pos);
            if (nodeIndex >= 0)
            {
                return nodeDatas[nodeIndex].node.gameObject;
            }
            return null;
        }

        private int GetInRangeNodeIndex(Vector2 pos)
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

        /// <summary>
        /// 根据目标点计算出发点
        /// </summary>
        public int GetMoveStartNodeIndex(Vector2 pos, Vector2 targetPos)
        {
            // 查找起点所在曲线的哪个点
            int closestSplineIndex = -1;
            float curT = 0;
            float minDistance = float.MaxValue;
            var startPos = new float3(pos.x, pos.y, 0);
            for (int i = 0; i < splineContainer.Splines.Count; i++)
            {
                var spline = splineContainer.Splines[i];
                var nativeSpline = new NativeSpline(spline, splineContainer.transform.localToWorldMatrix); //转换为曲线本地坐标
                var distance = SplineUtility.GetNearestPoint(nativeSpline, startPos, out float3 nearest, out var t);
                if (distance < minDistance)
                {
                    closestSplineIndex = i;
                    curT = t;
                    minDistance = distance;
                }
            }

            // 查找相邻的两侧节点
            float leftT = -float.MaxValue, rightT = float.MaxValue; //左右两侧
            int leftNodeIndex = -1, rightNodeIndex = -1;
            for (int i = 0; i < nodeDatas.Length; i++)
            {
                var nodeData = nodeDatas[i];
                int j = nodeData.splines.IndexOf(closestSplineIndex);
                if (j == -1)
                    continue;

                float t = nodeData.ts[j];
                if (t <= curT && t > leftT)
                {
                    leftT = t;
                    leftNodeIndex = i;
                }
                if (t >= curT && t < rightT)
                {
                    rightT = t;
                    rightNodeIndex = i;
                }
            }

            // 左右两节点取离终点最近的一个
            int closestNodeIndex = -1;
            minDistance = float.MaxValue;
            if (leftNodeIndex != -1)
            {
                float distance = Vector2.Distance(nodeDatas[leftNodeIndex].node.position, targetPos);
                if (distance < minDistance)
                {
                    closestNodeIndex = leftNodeIndex;
                    minDistance = distance;
                }
            }

            if (rightNodeIndex != -1)
            {
                float distance = Vector2.Distance(nodeDatas[rightNodeIndex].node.position, targetPos);
                if (distance < minDistance)
                {
                    closestNodeIndex = rightNodeIndex;
                    minDistance = distance;
                }
            }

            return closestNodeIndex;
        }

        public Vector2 GetNodePosition(int nodeIndex)
        {
            return nodeDatas[nodeIndex].node.position;
        }

        public void GetPathInfo(int curNodeIndex, int nextNodeIndex, out Spline spline, out float startT, out float endT)
        {
            var curNodeData = nodeDatas[curNodeIndex];
            var nextNodeData = nodeDatas[nextNodeIndex];

            spline = null;
            startT = 0;
            endT = 0;
            for (int i = 0; i < curNodeData.splines.Count; i++)
            {
                int splineIndex = curNodeData.splines[i];
                int index = nextNodeData.splines.IndexOf(splineIndex);
                if (index != -1)
                {
                    startT = curNodeData.ts[i];
                    endT = nextNodeData.ts[index];
                    spline = splineContainer.Splines[splineIndex];
                    return;
                }
            }

            Log.Error("没找到路径,curNodeIndex:{0},nextNodeIndex:{1}", curNodeIndex, nextNodeIndex);
        }

        public void GetAnySplineInfo(int nodeIndex, out Spline spline, out float t)
        {
            var nodeData = nodeDatas[nodeIndex];
            spline = splineContainer.Splines[nodeData.splines[0]];
            t = nodeData.ts[0];
        }

        public NodeData GetNodeData(int nodeIndex)
        {
            return nodeDatas[nodeIndex];
        }

        /// <summary>
        /// 返回路径节点列表的倒叙，单位根据这个列表去寻路，注意是倒叙
        /// </summary>
        /// <returns></returns>
        public List<int> GetCurrentPathNodes()
        {
            List<int> pathNodes = new();
            int targetNodeIndex = findPathTargetNodeIndex;
            while (targetNodeIndex != findPathStartNodeIndex)
            {
                pathNodes.Add(targetNodeIndex);
                targetNodeIndex = findPathNodePaths[targetNodeIndex];
            }
            pathNodes.Add(findPathStartNodeIndex);
            return pathNodes;
        }

        [Serializable]
        public class NodeData
        {
            public Transform node;
            public List<int> splines = new();
            public List<float> ts = new();
            public List<int> adjoinNodes = new();
        }
    }
}