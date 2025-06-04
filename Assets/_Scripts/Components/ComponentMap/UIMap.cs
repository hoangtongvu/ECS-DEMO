using Core.Misc;
using Core.UI;
using Core.UI.Identification;
using UnityEngine;

namespace Components.ComponentMap
{
    public class UIPrefabAndPool
    {
        public uint GlobalID;
        public GameObject Prefab;
        public ObjPool<BaseUICtrl> UIPool;
        public Transform DefaultHolderTransform; // this is parent transform for default.
    }

    public class UIPrefabAndPoolMap : BaseMap<UIType, UIPrefabAndPool> { }

    public class SpawnedUIMap : BaseMap<UIID, BaseUICtrl> { }

}