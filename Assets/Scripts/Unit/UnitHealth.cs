using System;
using UnityEngine;

namespace Takeover
{
    public class UnitHealth : MonoBehaviour
    {
        private int m_CurHealth = 100;
        public int CurHealth
        {
            get
            {
                return m_CurHealth;
            }
            set
            {
                value = Mathf.Max(value, 0);
                if (m_CurHealth != value)
                {
                    int oldHealth = m_CurHealth;
                    m_CurHealth = value;
                    OnHealthChange?.Invoke(value, oldHealth);
                    if (m_CurHealth <= 0)
                        OnDeath?.InvokeSafe();
                }

            }
        }

        // 不走序列化，走配表
        private int _maxHealth = 100;
        public int MaxHealth
        {
            get
            {
                return _maxHealth;
            }
            set
            {
                _maxHealth = value;
                CurHealth = _maxHealth;
            }
        }

        public bool IsInvincible { get; set; }
        public bool IsDead => CurHealth <= 0;

        public event Action OnDeath;
        public event Action<int, int> OnHealthChange;

        void Awake()
        {
            CurHealth = MaxHealth;
        }

        void OnDestroy()
        {
            OnDeath = null;
            OnHealthChange = null;
        }
    }
}