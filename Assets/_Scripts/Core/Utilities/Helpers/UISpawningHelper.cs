using Core.UI;
using Core.UI.Identification;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Utilities.Helpers
{
    public static class UISpawningHelper
    {
        public static BaseUICtrl Spawn(
            Dictionary<UIType, UIPrefabAndPool> uiPrefabAndPoolMap
            , Dictionary<UIID, BaseUICtrl> spawnedUIMap
            , UIType uiType)
        {
            var uiPrefabAndPool = uiPrefabAndPoolMap[uiType];

            uint newID = uiPrefabAndPool.GlobalID + 1;
            var uiPool = uiPrefabAndPool.UIPool;

            // Obj pool has no ref to prefab so it can't spawn new instance itself.
            // Instantiate with parent as parameter.
            if (!uiPool.TryGetFromPool(out BaseUICtrl baseUICtrl))
            {
                baseUICtrl =
                    Object.Instantiate(uiPrefabAndPool.Prefab, GetParentTransform(uiPrefabAndPool), false) // false to keep relative position.
                    .GetComponent<BaseUICtrl>();

                // Set ID.
                baseUICtrl.RuntimeUIID.Type = baseUICtrl.GetUIType();
                baseUICtrl.RuntimeUIID.LocalId = newID;
                uiPrefabAndPool.GlobalID = newID;

            }

            AddSpawnedUIIntoMap(spawnedUIMap, baseUICtrl);
            return baseUICtrl;
        }

        public static BaseUICtrl Spawn(
            Dictionary<UIType, UIPrefabAndPool> uiPrefabAndPoolMap
            , Dictionary<UIID, BaseUICtrl> spawnedUIMap
            , UIType uiType
            , float3 position
            , quaternion quaternion)
        {
            var baseUICtrl = Spawn(uiPrefabAndPoolMap, spawnedUIMap, uiType);
            baseUICtrl?.transform.SetPositionAndRotation(position, quaternion);
            return baseUICtrl;
        }

        public static BaseUICtrl Spawn(
            Dictionary<UIType, UIPrefabAndPool> uiPrefabAndPoolMap
            , Dictionary<UIID, BaseUICtrl> spawnedUIMap
            , UIType uiType
            , float3 position)
        {
            var baseUICtrl = Spawn(uiPrefabAndPoolMap, spawnedUIMap, uiType);

            if (baseUICtrl != null)
                baseUICtrl.transform.position = position;

            return baseUICtrl;
        }

        public static void Despawn(
            Dictionary<UIType, UIPrefabAndPool> uiPrefabAndPoolMap
            , Dictionary<UIID, BaseUICtrl> spawnedUIMap
            , UIID uiID)
        {
            var baseUICtrl = spawnedUIMap[uiID];
            baseUICtrl.Despawn(uiPrefabAndPoolMap, spawnedUIMap);
        }

        public static void Despawn(
            Dictionary<UIType, UIPrefabAndPool> uiPrefabAndPoolMap
            , Dictionary<UIID, BaseUICtrl> spawnedUIMap
            , BaseUICtrl baseUICtrl)
        {
            var uiPoolMapValue = uiPrefabAndPoolMap[baseUICtrl.GetUIType()];

            baseUICtrl.gameObject.SetActive(false);
            baseUICtrl.transform
                .SetParent(GetParentTransform(uiPoolMapValue));
            spawnedUIMap.Remove(baseUICtrl.RuntimeUIID);
            uiPoolMapValue.UIPool.AddToPool(baseUICtrl);
        }

        private static Transform GetParentTransform(UIPrefabAndPool uiPrefabAndPool) => uiPrefabAndPool.DefaultHolderTransform;

        private static void AddSpawnedUIIntoMap(
            Dictionary<UIID, BaseUICtrl> spawnedUIMap
            , BaseUICtrl baseUICtrl)
        {
            spawnedUIMap.Add(baseUICtrl.RuntimeUIID, baseUICtrl);
        }

    }

}