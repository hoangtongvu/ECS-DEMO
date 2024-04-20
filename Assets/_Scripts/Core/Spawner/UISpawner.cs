using Components.ComponentMap;
using Core.UI;
using Core.UI.Identification;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Spawner
{
	public sealed class UISpawner
	{
        private EntityManager em;
        private readonly UIPoolMap uiPoolMap;
        private readonly SpawnedUIMap spawnedUIMap;

        private static UISpawner instance;

        public static UISpawner Instance => instance ??= new();


        private UISpawner()
        {
            this.em = World.DefaultGameObjectInjectionWorld.EntityManager;

            this.uiPoolMap = this.GetUIPoolMap();
            this.spawnedUIMap = this.GetSpawnedUIMap();

        }

        private UIPoolMap GetUIPoolMap()
        {
            EntityQuery entityQuery = this.em.CreateEntityQuery(typeof(UIPoolMap));
            return entityQuery.GetSingleton<UIPoolMap>();
        }

        private SpawnedUIMap GetSpawnedUIMap()
        {
            EntityQuery entityQuery = this.em.CreateEntityQuery(typeof(SpawnedUIMap));
            return entityQuery.GetSingleton<SpawnedUIMap>();
        }

        public BaseUICtrl Spawn(UIType uiType, float3 position, quaternion quaternion)
        {
            if (!this.uiPoolMap.Value.TryGetValue(uiType, out var uiPoolMapValue))
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
                    Object.Instantiate(
                    uiPoolMapValue.Prefab
                    , position
                    , quaternion).GetComponent<BaseUICtrl>();
            }


            // Set ID.
            baseUICtrl.UIID.LocalId = newID;
            uiPoolMapValue.GlobalID = newID;

            // Set parent for newly spawned UI Element.
            baseUICtrl.transform
                .SetParent(this.GetParentTransform(uiPoolMapValue));


            this.AddSpawnedUIIntoMap(baseUICtrl);
            return baseUICtrl;
        }

        private Transform GetParentTransform(UIPoolMapValue uiPoolMapValue) => uiPoolMapValue.DefaultHolderTransform;

        private void AddSpawnedUIIntoMap(BaseUICtrl baseUICtrl)
        {
            if (this.spawnedUIMap.Value.TryAdd(baseUICtrl.UIID, baseUICtrl)) return;
            Debug.LogError($"SpawnedUIMap has already contained ID = {baseUICtrl.UIID}");
        }

    }




}