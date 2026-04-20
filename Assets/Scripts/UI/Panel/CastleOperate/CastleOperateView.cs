using GameFramework.Hot;
using UnityEngine;
using GameFramework.AOT;
using System.Collections.Generic;
using UnityEngine.U2D;

namespace Takeover
{
    public class CastleOperateView : BaseView<CastleOperateControl>
    {
        [SerializeField]
        private Transform groupNode;
        [SerializeField]
        private BtnArmyNode btnArmy;
        [SerializeField]
        private BtnArmyNode btnUpgrade;

        private List<BaseUINode> btnUnitList;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            // 点击空白区域关闭
            var button = GetComponent<GFButton>();
            BtnOnClick(button, (eventData) => Control.Close());

            btnUnitList = new();
            btnArmy.Visible = false;

            // 单位
            foreach (var armyId in Control.showArmies)
            {
                var node = CloneNode(btnArmy);
                node.name = armyId;
                int index = btnUnitList.Count;
                node.localPosition = GetBtnPosition(index);
                BtnOnClick(node.btn, (eventData) => OnClickUnitBtn(index));
                var spriteAtals = GFGlobal.Resource.LoadAssetSync<SpriteAtlas>(GFGlobal.Tables.TbGlobalSettingData.ArmyIconPath);
                node.imgIcon.sprite = spriteAtals.GetSprite(armyId);
                btnUnitList.Add(node);
            }

            groupNode.GetComponent<Camp>().CurCamp = Control.Camp;
            var worldPos = Control.Castle.GetComponent<NodeMap>().GetTransform("UICenterPos").transform.position;
            groupNode.transform.localPosition = groupNode.transform.WorldToUILocalPosition(worldPos, GFGlobal.UI.UICamera);

            // 升级
            BtnOnClick(btnUpgrade.btn, (data) =>
            {
                Log.Error("升级");
            });
            btnUpgrade.localPosition = GetBtnPosition(Control.showArmies.Count);
        }

        // 根据索引设置每个按钮的位置，考虑btnCount为[1,6]的情况
        private Vector2 GetBtnPosition(int index)
        {
            int btnCount = Control.showArmies.Count + 1;

            float dist = btnArmy.localPosition.magnitude;
            if (btnCount == 1)
                return new Vector2(0, dist);
            else if (btnCount == 2)
            {
                if (index == 0)
                    return new Vector2(0, dist);
                else
                    return new Vector2(0, -dist);
            }
            else
            {
                int i;
                if (index % 2 == 0)
                    i = index / 2;
                else
                    i = (index - 1) / 2;
                float rad = (0.5f + i) * 2 * Mathf.PI / btnCount;
                return new Vector2(dist * Mathf.Sin(rad) * (index % 2 == 0 ? -1 : 1), dist * Mathf.Cos(rad));
            }

        }

        private void OnClickUnitBtn(int index)
        {
            Log.Error("点击了第" + index + "个单位");
        }
    }
}