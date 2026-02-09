using System;
using UnityEngine;

namespace Takeover
{
    [DisallowMultipleComponent]
    public class SpriteMouseEvent : MonoBehaviour
    {
        public event Action OnMouseDownAction;

        void OnMouseDown()
        {
            OnMouseDownAction?.InvokeSafe();
        }

        void OnDestroy()
        {
            OnMouseDownAction = null;
        }
    }
}