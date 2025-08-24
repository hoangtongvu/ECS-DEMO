using Components.UI.MyCanvas;
using Components.UI.Pooling;
using Core.UI;
using Core.UI.Identification;
using Core.UI.MyCanvas;
using Core.UI.Pooling;
using Unity.Entities;
using UnityEngine;
using Utilities;

namespace Systems.Initialization.UI
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UICtrlPoolMapInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<CanvasesCtrlHolder>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var canvasesCtrl = SystemAPI.GetSingleton<CanvasesCtrlHolder>();

            this.LoadAllUIPrefabs(out var uiCtrls);
            this.AddUIPrefabsToMap(canvasesCtrl.Value, uiCtrls);

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddComponent<UIPoolMapInitializedTag>();
        }

        private void LoadAllUIPrefabs(out BaseUICtrl[] uiCtrls) => uiCtrls = Resources.LoadAll<BaseUICtrl>("UI");

        private void AddUIPrefabsToMap(CanvasesCtrl canvasesCtrl, BaseUICtrl[] uiCtrls)
        {
            var poolMap = UICtrlPoolMap.Instance;

            foreach (var uiCtrl in uiCtrls)
            {
                UIType type = uiCtrl.GetUIType();
                CanvasType canvasType = uiCtrl.CanvasType;
                CanvasAnchorPreset canvasAnchorPreset = uiCtrl.CanvasAnchorPreset;

                this.CreateUIPoolGameObj(
                    canvasesCtrl
                    , type
                    , canvasType
                    , canvasAnchorPreset
                    , out var defaultHolderTransform);

                poolMap.poolMap.Add(type, new()
                {
                    Prefab = uiCtrl.gameObject,
                    DefaultHolderTransform = defaultHolderTransform,
                });
            }

        }

        private void CreateUIPoolGameObj(
            CanvasesCtrl canvasesCtrl
            , UIType type
            , CanvasType canvasType
            , CanvasAnchorPreset canvasAnchorPreset
            , out Transform defaultHolderTransform)
        {
            GameObject newGameObject = new($"{type}_Pool");
            newGameObject.AddComponent<RectTransform>();

            newGameObject.transform.SetParent(this.GetParentTransform(canvasesCtrl, canvasType, canvasAnchorPreset));
            newGameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            defaultHolderTransform = newGameObject.transform;
        }

        private Transform GetParentTransform(CanvasesCtrl canvasesCtrl, CanvasType canvasType, CanvasAnchorPreset canvasAnchorPreset)
        {
            switch (canvasType)
            {
                case CanvasType.WorldSpace:
                    return canvasesCtrl.WorldSpaceCanvasTransform;

                case CanvasType.Overlay:
                    int index = (int)canvasAnchorPreset;
                    return canvasesCtrl.OverlayCanvasManager.GetAnchorPresetTransform(index);

                default:
                    throw new System.Exception($"canvasType is {CanvasType.None}");
            }

        }

    }

}