using Components.ComponentMap;
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
            var uiPoolMap = new UIPoolMap
            {
                Value = new Dictionary<UIType, UIPoolMapValue>(),
            };

            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(uiPoolMap);

            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(new SpawnedUIMap
                {
                    Value = new Dictionary<UIID, Core.UI.BaseUICtrl>(),
                });

            this.LoadAllUIPrefabs(out var uiCtrls);
            this.AddUIsIntoMap(uiPoolMap, uiCtrls);

            this.Enabled = false;

        }

        protected override void OnUpdate()
        {
        }

        private void LoadAllUIPrefabs(out BaseUICtrl[] uiCtrls) => uiCtrls = Resources.LoadAll<BaseUICtrl>("UI");

        private void AddUIsIntoMap(UIPoolMap uiMap, BaseUICtrl[] uiCtrls)
        {
            foreach (var uiCtrl in uiCtrls)
            {
                UIType type = uiCtrl.UIID.Type;
                CanvasType canvasType = uiCtrl.CanvasType;
                CanvasAnchorPreset canvasAnchorPreset = uiCtrl.CanvasAnchorPreset;

                this.CreateUIPoolGameObj(
                    type
                    , canvasType
                    , canvasAnchorPreset
                    , out var objPool);

                bool canAddIntoUIMap = uiMap.Value.TryAdd(
                    type
                    , new UIPoolMapValue
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
            UIType type
            , CanvasType canvasType
            , CanvasAnchorPreset canvasAnchorPreset
            , out BaseUIPool objPool)
        {
            GameObject newGameObject = new($"{type} Pool");
            newGameObject.AddComponent<RectTransform>();

            newGameObject.transform.SetParent(this.GetParentTransform(canvasType, canvasAnchorPreset));
            newGameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            objPool = newGameObject.AddComponent<BaseUIPool>();
        }

        private Transform GetParentTransform(CanvasType canvasType, CanvasAnchorPreset canvasAnchorPreset)
        {
            switch (canvasType)
            {
                case CanvasType.WorldSpace:
                    return CanvasesCtrl.Instance.WorldSpaceCanvasTransform;

                case CanvasType.Overlay:
                    int index = (int)canvasAnchorPreset;
                    return CanvasesCtrl.Instance.OverlayCanvasManager.GetAnchorPresetTransform(index);

                default:
                    throw new System.Exception($"canvasType is {CanvasType.None}");

            }

        }

    }

}