using Components.ComponentMap;
using Core.UI;
using Core.UI.Identification;
using Unity.Mathematics;
using UnityEngine;

namespace Utilities.Helpers
{
    public static class UISpawningHelper
    {
        public static BaseUICtrl Spawn(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , UIType uiType)
        {
            if (!uiPrefabAndPoolMap.Value.TryGetValue(uiType, out var uiPrefabAndPool))
            {
                Debug.LogError($"Can't find UI prefab of type {uiType}");
                return null;
            }

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
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , UIType uiType
            , float3 position
            , quaternion quaternion)
        {
            var baseUICtrl = Spawn(uiPrefabAndPoolMap, spawnedUIMap, uiType);
            baseUICtrl?.transform.SetPositionAndRotation(position, quaternion);
            return baseUICtrl;
        }

        public static BaseUICtrl Spawn(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , UIType uiType
            , float3 position)
        {
            var baseUICtrl = Spawn(uiPrefabAndPoolMap, spawnedUIMap, uiType);

            if (baseUICtrl != null)
                baseUICtrl.transform.position = position;

            return baseUICtrl;
        }

        public static void Despawn(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , UIID uiID)
        {
            if (!spawnedUIMap.Value.TryGetValue(uiID, out BaseUICtrl baseUICtrl))
            {
                Debug.LogError($"Can't find BaseUICtrl with ID = {uiID}");
                return;
            }

            if (!uiPrefabAndPoolMap.Value.TryGetValue(uiID.Type, out var uiPoolMapValue))
            {
                Debug.LogError($"Can't find UI prefab of type {uiID.Type}");
                return;
            }

            baseUICtrl.gameObject.SetActive(false);
            baseUICtrl.transform
                .SetParent(GetParentTransform(uiPoolMapValue));
            spawnedUIMap.Value.Remove(uiID);
            uiPoolMapValue.UIPool.AddToPool(baseUICtrl);
        }

        private static Transform GetParentTransform(UIPrefabAndPool uiPrefabAndPool) => uiPrefabAndPool.DefaultHolderTransform;

        private static void AddSpawnedUIIntoMap(
            SpawnedUIMap spawnedUIMap
            , BaseUICtrl baseUICtrl)
        {
            if (spawnedUIMap.Value.TryAdd(baseUICtrl.RuntimeUIID, baseUICtrl)) return;
            Debug.LogError($"SpawnedUIMap has already contained ID = {baseUICtrl.RuntimeUIID}");
        }

    }

}