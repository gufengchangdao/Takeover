using UnityEngine;

namespace GameFramework.Hot
{
    public class BTRepeatNode : BTDecoratorNode
    {
        private int conunter;//当前重复次数
        private int limit;//重复限度，-1为无限循环
        private int lastFrame = -1; //每帧只执行一个

        public BTRepeatNode(int limit)
        {
            this.limit = limit;
        }

        protected override void OnInitialize()
        {
            conunter = 0;//进入时，将次数清零
        }

        protected override EBehaviorStatus OnUpdate()
        {
            if (lastFrame == Time.frameCount)
                return EBehaviorStatus.Running;

            lastFrame = Time.frameCount;

            child.Tick();
            if (child.IsRunning)
                return EBehaviorStatus.Running;
            if (child.IsFailure)
                return EBehaviorStatus.Failure;
            //子节点执行成功，就增加一次计算，达到设定限度才返回成功
            if (limit != -1 && ++conunter >= limit)
                return EBehaviorStatus.Success;
            child.Reset();
            return EBehaviorStatus.Running;
        }
    }
}