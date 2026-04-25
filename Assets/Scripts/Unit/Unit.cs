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

        public bool IsActive => !Health.IsDead;

        void Awake()
        {
            Health = GetComponent<UnitHealth>();
        }

        public void Init(int maxHealth)
        {
            Health.MaxHealth = maxHealth;
        }

        public void OnEnterCastle(Castle castle)
        {
            transform.position = castle.transform.position;
            gameObject.SetActive(false);
        }

        public void OnExitCastle()
        {
            if (!Health.IsDead)
                gameObject.SetActive(true);
        }
    }
}