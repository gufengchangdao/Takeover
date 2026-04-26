using System;
using GameFramework.AOT;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace GameFramework.Hot
{
    /// <summary>
    /// 输入模块
    /// </summary>
    public class GFInput : GFBaseModule
    {
        private InputActionAsset asset;
        private InputActionAsset sourceAsset;
        private InputSystemUIInputModule inputModule;
        void Awake()
        {
            inputModule = FindAnyObjectByType<InputSystemUIInputModule>();
            inputModule.transform.SetParent(transform);

            var playerInput = gameObject.AddComponent<PlayerInput>();
            playerInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;

            // asset = GFGlobal.Resource.LoadAssetSync<InputActionAsset>(GFGlobal.Config.inputActionAssetPath, false);

            sourceAsset = inputModule.actionsAsset;      // 永远保存原始引用
            asset = Instantiate(sourceAsset);     // 只从原始引用克隆

            asset.Disable();
            AddInputActions();
            asset.Enable();
            playerInput.actions = asset;
            inputModule.actionsAsset = asset;
        }

        void OnDestroy()
        {
            if (inputModule != null) inputModule.actionsAsset = sourceAsset; // 还原
            if (asset != null) Destroy(asset);
        }

        private void AddInputActions()
        {
            foreach (var inputData in GFGlobal.Tables.TbInputData.DataList)
            {
                var actionMap = asset.FindActionMap(inputData.Map);
                if (actionMap == null)
                {
                    actionMap = asset.AddActionMap(inputData.Map);
                    actionMap.Enable();
                }

                Log.Info($"[Input] Add Action {inputData.Id}");
                var action = actionMap.FindAction(inputData.Id);
                if (action != null)
                {
                    Log.Error($"[Input] Action {inputData.Id} have already existed");
                    continue;
                }
                action = actionMap.AddAction(inputData.Id, (InputActionType)inputData.Type); //如果已经存在会报错
                action.AddBinding(inputData.Path);
            }
        }

        public InputAction GetAction(string mapName, string actionName)
        {
            var actionMap = asset.FindActionMap(mapName);
            if (actionMap == null)
            {
                Log.Error($"[Input] map {mapName} not found");
                return null;
            }
            var action = actionMap.FindAction(actionName);
            if (action == null)
            {
                Log.Error($"[Input] action {actionName} not found");
                return null;
            }
            return action;
        }

        public void SetMapEnable(string mapName, bool enable)
        {
            var actionMap = asset.FindActionMap(mapName);
            if (actionMap == null)
            {
                Log.Error($"[Input] map {mapName} not found");
                return;
            }

            if (enable)
                actionMap.Enable();
            else
                actionMap.Disable();
        }

        public void SetActionEnable(string mapName, string actionName, bool enable)
        {
            var action = GetAction(mapName, actionName);
            if (action == null)
                return;

            if (enable)
                action.Enable();
            else
                action.Disable();
        }

        public void RegisterAction(
            string mapName,
            string actionName,
            Action<InputAction.CallbackContext> started = null,
            Action<InputAction.CallbackContext> performed = null,
            Action<InputAction.CallbackContext> canceled = null)
        {
            var action = GetAction(mapName, actionName);
            if (action == null)
                return;

            if (performed != null)
                action.performed += performed;
            if (canceled != null)
                action.canceled += canceled;
            if (started != null)
                action.started += started;
        }

        public void UnregisterAction(
            string mapName,
            string actionName,
            Action<InputAction.CallbackContext> started = null,
            Action<InputAction.CallbackContext> performed = null,
            Action<InputAction.CallbackContext> canceled = null)
        {
            var action = GetAction(mapName, actionName);
            if (action == null)
                return;

            if (performed != null)
                action.performed -= performed;
            if (canceled != null)
                action.canceled -= canceled;
            if (started != null)
                action.started -= started;
        }
    }
}