using System;
using UnityEngine;

namespace Takeover
{
    public class UnitHealth : MonoBehaviour
    {
        private float m_CurHealth = 100;
        public float CurHealth
        {
            get
            {
                return m_CurHealth;
            }
            set
            {
                m_CurHealth = Mathf.Max(value, 0);
                if (m_CurHealth <= 0)
                    OnDeath?.InvokeSafe();
            }
        }

        public float maxHealth = 100;

        public bool IsInvincible { get; set; }
        public bool IsDead => CurHealth <= 0;

        public event Action OnDeath;

        void Awake()
        {
            CurHealth = maxHealth;
        }

        void OnDestroy()
        {
            OnDeath = null;
        }
    }
}