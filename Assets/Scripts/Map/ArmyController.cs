using GameFramework.AOT;
using GameFramework.Hot;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Takeover
{
    public class ArmyController : UpdateableComponent
    {
        private Army selectArmy;
        private Castle selectCastle;
        private CooldownTimer pathCalculationCD = new(.1f);

        private UnitControlLine unitControlLine;

        protected override void Start()
        {
            base.Start();
            unitControlLine = FindAnyObjectByType<UnitControlLine>();
            if (unitControlLine == null)
            {
                var go = GFGlobal.Resource.InstantiatePrefab(GFGlobal.GlobalTableData.UnitLineParefabPath);
                unitControlLine = go.GetComponent<UnitControlLine>();
            }
            GFGlobal.Input.RegisterAction("Player", InputEnum.UnitControl, StartUnitControl, null, StopUnitControl);
            GFGlobal.Input.RegisterAction("Player", InputEnum.CancelControl, CancelUnitControl, null, null);
        }

        protected override void OnDestroy()
        {
            GFGlobal.Input.UnregisterAction("Player", InputEnum.UnitControl, StartUnitControl, null, StopUnitControl);
            GFGlobal.Input.UnregisterAction("Player", InputEnum.CancelControl, CancelUnitControl, null, null);
            base.OnDestroy();
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            if (!selectArmy)
                return;

            // 路径节点吸附
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector3 targetPos = mouseWorldPos;
            Transform selectUnit = Global.MapPath.GeInRangeNodeTransform(mouseWorldPos);
            if (selectUnit)
                targetPos = selectUnit.position;
            unitControlLine.Draw(selectArmy.transform.position, targetPos);

            // 选中城堡
            // if (selectUnit != null && selectUnit.CompareTag(GameObjectTag.Castle))
            //     selectCastle = selectUnit.GetComponent<Castle>();
            // else
            //     selectCastle = null;

            // 计算并显示路径节点
            if (pathCalculationCD.IsReady(true))
                Global.MapPath.UpdatePathNodeVisible(selectArmy.MainUnitPosition, mouseWorldPos);
        }

        private void StartUnitControl(CallbackContext context)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

            // 2D射线检测：需要目标物体上有Collider2D（例如BoxCollider2D）
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider == null)
                return;

            if (!hit.collider.TryGetComponent(out Army army))
                return;

            selectArmy = army;
            Log.Debug("选中部队" + army);
        }

        private void StopUnitControl(CallbackContext context)
        {
            if (!selectArmy)
            {
                StopUnitControl();
                return;
            }


            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            int nodeIndex = Global.MapPath.GetMouseRangeNodeIndex(mouseWorldPos);
            if (nodeIndex == -1) //鼠标不在节点范围
            {
                StopUnitControl();
                return;
            }

            selectArmy.CommandGotoTarget(nodeIndex);
            StopUnitControl();
        }

        private void CancelUnitControl(CallbackContext context)
        {
            StopUnitControl();
        }

        private void StopUnitControl()
        {
            selectArmy = null;
            unitControlLine.Clear();
            Global.MapPath.HideAllPathNode();
        }
    }
}