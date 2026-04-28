using GameFramework.AOT;
using GameFramework.Hot;
using UnityEngine;


public class Test : MonoBehaviour
{
    private BehaviorTree tree;
    void Start()
    {
        var builder = new BehaviorTreeBuilder();
        tree = builder
            .Repeat(3)
                .Sequence()
                    .DebugNode("1")
                    .DebugNode("2")
                    .DebugNode("3")
                    .AddNode(new BTMoveNode(transform))
                .Back()
            .End();
    }

    void Update()
    {
        if (!tree.IsTerminated)
            tree.Tick();
    }

    public class BTMoveNode : BehaviorNode
    {
        private Transform owner;
        private float curTime;

        public BTMoveNode(Transform owner)
        {
            this.owner = owner;
        }

        protected override void OnInitialize()
        {
            curTime = 0;
            owner.transform.position = Vector2.zero;
        }

        protected override EBehaviorStatus OnUpdate()
        {
            var pos = owner.transform.position;
            curTime += Time.deltaTime;
            pos.x = curTime;
            owner.transform.position = pos;
            if (pos.x > 1)
                return EBehaviorStatus.Success;
            return EBehaviorStatus.Running;
        }
    }
}
