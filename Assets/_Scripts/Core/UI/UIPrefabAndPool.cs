using Core.Misc;
using UnityEngine;

namespace Core.UI
{
    public class UIPrefabAndPool
    {
        public uint GlobalID;
        public GameObject Prefab;
        public ObjPool<BaseUICtrl> UIPool;
        public Transform DefaultHolderTransform; // this is parent transform for default.
    }
}