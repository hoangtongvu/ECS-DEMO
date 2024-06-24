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
            UIPoolMap uiPoolMap
            , SpawnedUIMap spawnedUIMap
            , UIType uiType
            , float3 position
            , quaternion quaternion)
        {
            if (!uiPoolMap.Value.TryGetValue(uiType, out var uiPoolMapValue))
            {
                Debug.LogError($"Can't find UI prefab of type {uiType}");
                return null;
            }

            uint newID = uiPoolMapValue.GlobalID + 1;
            var uiPool = uiPoolMapValue.UIPool;

            // Obj pool has no ref to prefab so it can't spawn new instance itself.
            if (!uiPool.TryGetFromPool(out BaseUICtrl baseUICtrl))
            {
                baseUICtrl =
                    Object.Instantiate(uiPoolMapValue.Prefab)
                    .GetComponent<BaseUICtrl>();
            }

            baseUICtrl.transform.SetPositionAndRotation(position, quaternion);

            // Set ID.
            baseUICtrl.UIID.LocalId = newID;
            uiPoolMapValue.GlobalID = newID;

            // Set parent for newly spawned UI Element.
            baseUICtrl.transform
                .SetParent(GetParentTransform(uiPoolMapValue));


            AddSpawnedUIIntoMap(spawnedUIMap, baseUICtrl);
            return baseUICtrl;
        }

        public static void Despawn(
            UIPoolMap uiPoolMap
            , SpawnedUIMap spawnedUIMap
            , UIID uiID)
        {
            if (!spawnedUIMap.Value.TryGetValue(uiID, out BaseUICtrl baseUICtrl))
            {
                Debug.LogError($"Can't find BaseUICtrl with ID = {uiID}");
                return;
            }

            if (!uiPoolMap.Value.TryGetValue(uiID.Type, out var uiPoolMapValue))
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

        private static Transform GetParentTransform(UIPoolMapValue uiPoolMapValue) => uiPoolMapValue.DefaultHolderTransform;

        private static void AddSpawnedUIIntoMap(
            SpawnedUIMap spawnedUIMap
            , BaseUICtrl baseUICtrl)
        {
            if (spawnedUIMap.Value.TryAdd(baseUICtrl.UIID, baseUICtrl)) return;
            Debug.LogError($"SpawnedUIMap has already contained ID = {baseUICtrl.UIID}");
        }

    }
}