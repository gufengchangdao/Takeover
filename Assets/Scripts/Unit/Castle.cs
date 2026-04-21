using TableStructure;
using GameFramework.Hot;
using UnityEngine;
using GameFramework.AOT;
using System.Collections.Generic;

namespace Takeover
{
    /// <summary>
    /// 城堡
    /// </summary>
    [RequireComponent(typeof(UnitHealth))]
    [RequireComponent(typeof(Camp))]
    public partial class Castle : MonoBehaviour
    {
        public string castleId;

        public UnitHealth Health { get; private set; }

        private Camp campComp;
        public ECamp Camp => campComp.CurCamp;

        /// <summary>
        /// 驻城的小队
        /// </summary>
        public List<Army> Defenders { get; private set; } = new();

        void Awake()
        {
            campComp = GetComponent<Camp>();
            Health = GetComponent<UnitHealth>();
        }

        void Start()
        {
            campComp.OnCampChange += OnCampChange;
            OnCampChange(Camp);
        }

        void OnMouseDown()
        {
            GFGlobal.UI.OpenPanel<CastleOperateControl>(userData: this);
        }

        private void OnCampChange(ECamp camp)
        {
            var spriteMouseEvent = GetComponentInChildren<SpriteMouseEvent>();
            if (spriteMouseEvent)
            {
                spriteMouseEvent.OnMouseDownAction -= OnMouseDown;
                spriteMouseEvent.OnMouseDownAction += OnMouseDown;
            }
        }
    }
}