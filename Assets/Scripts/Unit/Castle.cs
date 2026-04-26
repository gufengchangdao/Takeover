using TableStructure;
using GameFramework.Hot;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Takeover
{
    /// <summary>
    /// 城堡
    /// </summary>
    [RequireComponent(typeof(UnitHealth))]
    [RequireComponent(typeof(Camp))]
    public partial class Castle : MonoBehaviour, IPointerClickHandler
    {
        public const int MAX_LEVEL = 2;

        [SerializeField] private string _tableId = CastleDataIDS.Castle;
        public string TableId => _tableId;

        public UnitHealth Health { get; private set; }

        private Camp campComp;
        public ECamp Camp => campComp.CurCamp;

        /// <summary>
        /// 驻城的小队
        /// </summary>
        public List<Army> Defenders { get; private set; } = new();

        /// <summary>
        /// 城堡当前等级
        /// </summary>
        public BindableProperty<int> Level = new();

        /// <summary>
        /// 升到下一级所需金币
        /// </summary>
        public int NextLevelCost
        {
            get
            {
                return Level.Value switch
                {
                    0 => 350,
                    1 => 500,
                    _ => -1
                };
            }
        }

        public bool IsActive { get; private set; } = true;

        void Awake()
        {
            campComp = GetComponent<Camp>();
            Health = GetComponent<UnitHealth>();
        }

        public void OnArmyEnter(Army army)
        {
            if (!Defenders.Contains(army))
                Defenders.Add(army);
        }

        public void OnArmyExit(Army army)
        {
            Defenders.Remove(army);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GFGlobal.UI.OpenPanel<CastleOperateControl>(userData: this);
        }
    }
}