using System;
using GameFramework.AOT;
using UnityEngine;

namespace Takeover
{
    public partial class Unit
    {
        // 位移增量驱动位置更新
        private float impulseDX;
        private float impulseDY;

        private Vector2 _targetPos;

        /// <summary>
        /// 要移动到的位置
        /// </summary>
        public Vector2 TargetPos
        {
            get
            {
                return _targetPos;
            }
            set
            {
                var pos = Position;
                AtTarget = Mathf.Approximately(pos.x, value.x) && Mathf.Approximately(pos.y, value.y);
                _targetPos = value;

                behaviorCD.SetDone(); //看看能不能移动
            }
        }

        public float TargetAngle { get; private set; }

        public Vector2 Position
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        /// <summary>
        /// 是否到达目标位置
        /// </summary>
        public bool _atTarget;
        public bool AtTarget
        {
            get
            {
                return _atTarget;
            }
            set
            {
                if (_atTarget != value)
                {
                    _atTarget = value;
                    if (value)
                        OnArriveTarget?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// 到达目标位置后
        /// </summary>
        public event Action<Unit> OnArriveTarget;

        public float Speed { get; private set; }

        private static int ANIM_PARAMS_IS_UP = Animator.StringToHash("IsUp");

        /// <summary>
        /// 添加位移冲量
        /// </summary>
        public void AddMoveImpulse(float dx, float dy)
        {
            impulseDX += dx;
            impulseDY += dy;
        }

        public void ResetImpulse()
        {
            impulseDX = 0;
            impulseDY = 0;
        }

        private void UpdateAnimDir(float dx, float dy)
        {
            if (!Mathf.Approximately(dy, 0))
                Animator.SetFloat(ANIM_PARAMS_IS_UP, dy > 0 ? 1 : 0);
        }

        private void ApplyMoveImpulse()
        {
            var pos = Position;
            pos.x += impulseDX;
            pos.y += impulseDY;
            Position = pos;
        }

        private bool UpdateMove(float dt)
        {
            var curPos = transform.position;
            float dx = TargetPos.x - curPos.x;
            float dy = TargetPos.y - curPos.y;
            var distXdist = dx * dx + dy * dy;
            var speedDist = GetSpeedDistance(dt);
            if (distXdist < speedDist * speedDist)
                return true;

            UpdateAnimDir(dx, dy);
            var angl = Mathf.Atan2(dy, dx);
            AddMoveImpulse(Mathf.Cos(angl) * speedDist, Mathf.Sin(angl) * speedDist);
            TargetAngle = Mathf.Atan2(dy, dx);
            return false;
        }

        private float GetSpeedDistance(float time)
        {
            return Speed * time;
        }

        public void UpdateStateGotoXY(float dt)
        {
            if (UpdateMove(dt))
            {
                ResetImpulse(); //到达目标重置冲量
                Position = TargetPos;//精准吸附
                AtTarget = true;
            }
        }
    }
}