using Core.UI;
using Core.UI.Identification;
using UnityEngine;

namespace Components.ComponentMap
{
    public class UIPoolMapValue
    {
        public uint GlobalID;
        public GameObject Prefab;
        public ObjPool<BaseUICtrl> UIPool; // this is parent transform for default.
    }

    public class UIPoolMap : BaseMap<UIType, UIPoolMapValue> { }

    public class SpawnedUIMap : BaseMap<UIID, BaseUICtrl> { }

}

