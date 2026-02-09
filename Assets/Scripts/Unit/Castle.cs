using System.Collections.Generic;
using TableStructure;
using GameFramework.Hot;
using UnityEngine;
using GameFramework.AOT;

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

        [SerializeField]
        private List<Camp> mapDecorations = new();

        public Camp Camp { get; private set; }

        protected void Awake()
        {
            Camp = GetComponent<Camp>();
            Camp.OnCampChange += OnCampChange;
        }

        void OnDestroy()
        {
            Camp.OnCampChange -= OnCampChange;
        }

        void OnMouseDown()
        {
            if (!GFGlobal.UI.HasPanel<CastleOperateControl>())
                GFGlobal.UI.OpenPanel<CastleOperateControl>("CastleOperate", 0, this);
        }

        private void OnCampChange(ECamp camp)
        {
            // 附近装饰物“刷漆”
            foreach (var decorate in mapDecorations)
                decorate.CurCamp = camp;

            var spriteMouseEvent = GetComponentInChildren<SpriteMouseEvent>();
            if (spriteMouseEvent)
            {
                spriteMouseEvent.OnMouseDownAction -= OnMouseDown;
                spriteMouseEvent.OnMouseDownAction += OnMouseDown;
            }
            else
            {
                Log.Error($"城堡{name} {Camp.CurCamp}阵营没有找到SpriteMouseEvent组件，无法处理点击");
            }
        }
    }
}