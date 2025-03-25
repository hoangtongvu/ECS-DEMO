using Components.ComponentMap;
using Components.UI.MyCanvas;
using Core.UI;
using Core.UI.Identification;
using Core.UI.MyCanvas;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Utilities;

namespace Systems.Initialization.UI
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UIMapInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<CanvasesCtrlHolder>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var canvasesCtrl = SystemAPI.GetSingleton<CanvasesCtrlHolder>();

            var uiPrefabAndPoolMap = new UIPrefabAndPoolMap
            {
                Value = new Dictionary<UIType, UIPrefabAndPool>(),
            };

            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(uiPrefabAndPoolMap);

            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(new SpawnedUIMap
                {
                    Value = new Dictionary<UIID, Core.UI.BaseUICtrl>(),
                });

            this.LoadAllUIPrefabs(out var uiCtrls);
            this.AddUIsIntoMap(uiPrefabAndPoolMap, canvasesCtrl.Value, uiCtrls);

        }

        private void LoadAllUIPrefabs(out BaseUICtrl[] uiCtrls) => uiCtrls = Resources.LoadAll<BaseUICtrl>("UI");

        private void AddUIsIntoMap(UIPrefabAndPoolMap uiPrefabAndPoolMap, CanvasesCtrl canvasesCtrl, BaseUICtrl[] uiCtrls)
        {
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
                    , out var objPool);

                bool canAddIntoUIMap = uiPrefabAndPoolMap.Value.TryAdd(
                    type
                    , new UIPrefabAndPool
                    {
                        GlobalID = 0,
                        Prefab = uiCtrl.gameObject,
                        UIPool = objPool,
                        DefaultHolderTransform = objPool.transform,
                    });

                if (canAddIntoUIMap) continue;
                throw new System.Exception($"Another BaseUICtrl has already been registered with UIType = {type}");

            }

        }

        private void CreateUIPoolGameObj(
            CanvasesCtrl canvasesCtrl
            , UIType type
            , CanvasType canvasType
            , CanvasAnchorPreset canvasAnchorPreset
            , out BaseUIPool objPool)
        {
            GameObject newGameObject = new($"{type} Pool");
            newGameObject.AddComponent<RectTransform>();

            newGameObject.transform.SetParent(this.GetParentTransform(canvasesCtrl, canvasType, canvasAnchorPreset));
            newGameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            objPool = newGameObject.AddComponent<BaseUIPool>();
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