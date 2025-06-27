using Core.Misc;
using Core.UI.Identification;
using Core.UI.MyCanvas;
using Core.Utilities.Helpers;
using System.Collections.Generic;

namespace Core.UI
{
    public abstract class BaseUICtrl : SaiMonoBehaviour
    {
        public UIID RuntimeUIID;
        public CanvasType CanvasType;
        public CanvasAnchorPreset CanvasAnchorPreset;

        public abstract UIType GetUIType();

        public virtual void Despawn(
            Dictionary<UIType, UIPrefabAndPool> uiPrefabAndPoolMap
            , Dictionary<UIID, BaseUICtrl> spawnedUIMap)
        {
            UISpawningHelper.Despawn(uiPrefabAndPoolMap, spawnedUIMap, this);
        }

    }

}