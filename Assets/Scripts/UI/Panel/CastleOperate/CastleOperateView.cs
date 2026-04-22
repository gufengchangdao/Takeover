using GameFramework.Hot;
using UnityEngine;
using GameFramework.AOT;
using System.Collections.Generic;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;

namespace Takeover
{
    public class CastleOperateView : BaseView<CastleOperateControl>
    {
        [SerializeField] private RectTransform groupNode;
        [SerializeField] private RectTransform groupBtn;
        [SerializeField] private BtnArmyNode btnArmy;
        [SerializeField] private BtnArmyNode btnUpgrade;
        [SerializeField] private TextMeshProUGUI txtName;

        private List<BtnArmyNode> btnUnitList;

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
                BtnOnClick(node.btn, (eventData) => OnClickUnitBtn(index));
                var spriteAtals = GFGlobal.Resource.LoadAssetSync<SpriteAtlas>(GFGlobal.Tables.TbGlobalSettingData.ArmyIconPath);
                node.imgIcon.sprite = spriteAtals.GetSprite(armyId);
                btnUnitList.Add(node);
            }

            // 升级
            BtnOnClick(btnUpgrade.btn, (data) =>
            {
                Log.Error("升级");
            });
            btnUpgrade.transform.SetAsLastSibling();

            txtName.text = GFGlobal.Tables.TbCastleData[Control.Castle.TableId].ShowName;

            OnGoldOrSupplyChange();

            // 计算坐标
            groupNode.GetComponent<Camp>().CurCamp = Control.Camp;
            var worldPos = Control.Castle.GetComponent<NodeMap>().GetTransform("UICenterPos").transform.position;
            groupNode.transform.localPosition = groupNode.transform.WorldToUILocalPosition(worldPos, GFGlobal.UI.UICamera);
            RefreshGroupBtnPosition();
        }

        // 计算按钮相对位置，左边或者右边
        private void RefreshGroupBtnPosition()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(groupBtn); //刷新自适应尺寸

            var screenPos = RectTransformUtility.WorldToScreenPoint(GFGlobal.UI.UICamera, groupNode.position);
            bool isLeftSide = screenPos.x < Screen.width * 0.5f;
            float offsetX = (groupNode.rect.width + groupBtn.rect.width) * 0.5f;

            var localPos = groupBtn.localPosition;
            localPos.x = isLeftSide ? offsetX : -offsetX;
            groupBtn.localPosition = localPos;
        }

        private void OnClickUnitBtn(int index)
        {
            Log.Error("点击了第" + index + "个单位");
        }

        // 金币或人口改变时
        private void OnGoldOrSupplyChange()
        {
            for (int i = 0; i < Control.showArmies.Count; i++)
            {
                var node = btnUnitList[i];
                bool canBuy = Global.CombotantData.CheckCanBuy(Control.showArmies[i]);
                node.Gray = !canBuy;
                node.btn.enabled = canBuy;
            }

            bool canUpgrade = Global.CombotantData.CheckCanUpgradeCastle(Control.Castle);
            btnUpgrade.Gray = !canUpgrade;
            btnUpgrade.enabled = canUpgrade;
        }
    }
}
