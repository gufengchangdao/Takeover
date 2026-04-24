using UnityEngine;

namespace Takeover
{
    /// <summary>
    /// 一个士兵作为一个单位
    /// </summary>
    [RequireComponent(typeof(UnitHealth))]
    public class Unit : MonoBehaviour
    {
        public UnitHealth Health { get; private set; }

        void Awake()
        {
            Health = GetComponent<UnitHealth>();
        }

        public void Init(int maxHealth)
        {
            Health.MaxHealth = maxHealth;
        }
    }
}