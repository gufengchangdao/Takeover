using GameFramework.AOT;
using UnityEngine;

namespace GameFramework.Hot
{
    public partial class GFUI
    {
        private KeyedReferencePool<int, BaseUINode> nodePool;

        public T GetCacheNode<T>(int sourceId) where T : BaseUINode
        {
            var node = nodePool.Get(sourceId);
            if (node == null)
                return null;

            if (node is T res)
            {
                res.transform.SetAsLastSibling();
                res.Visible = true;
                return res;
            }
            else
            {
                Log.Error("[UI] GetCacheNode: {0} != {1}", typeof(T), node.GetType());
                return null;
            }
        }

        public void RecycleNode(BaseUINode node)
        {
            node.Visible = false;
            nodePool.Recycle(node.SourceId, node);
        }

        public void DestroyCacheNode(int sourceId)
        {
            nodePool.Remove(sourceId);
        }

        private void PoolRecycleNode(BaseUINode node)
        {
            node.Visible = false;
            node.OnRecycle();
        }

        private void PoolDestoryNode(BaseUINode node)
        {
            foreach (var childNode in node.gameObject.GetComponentsInChildren<BaseUINode>(true))
                nodePool.Remove(childNode.SourceId, false);
            GameObject.Destroy(node.gameObject);
        }
    }
}