using Components;
using Components.CustomIdentification;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class TransformSynchronizerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<UniqueId>();
            RequireForUpdate<LocalTransform>();
        }

        protected override void OnUpdate()
        {
            //Sync Transform from Entity world to corresponding Transform in GameObject world.

            if (!SystemAPI.ManagedAPI.TryGetSingleton<UnityObjectMap>(out UnityObjectMap objectMap))
            {
                Debug.LogError("UnityObjectMap Singleton not found");
                return;
            }

            //new SyncJob
            //{
            //    ObjectMap = objectMap,
            //}.Schedule();

            this.SyncFunc(objectMap);

        }

        private void SyncFunc(in UnityObjectMap objectMap)
        {
            foreach (var (idRef, transformRef) in
                SystemAPI.Query<RefRO<UniqueId>, RefRO<LocalTransform>>())
            {
                if (!objectMap.Value.TryGetValue(idRef.ValueRO, out UnityEngine.Object gameObj)) continue;

                Transform gameObjTransform = ((GameObject)gameObj).transform;

                float3 enityPos = transformRef.ValueRO.Position;
                gameObjTransform.position = new Vector3(enityPos.x, enityPos.y, enityPos.z);
            }
        }

        private partial struct SyncJob : IJobEntity
        {
            public UnityObjectMap ObjectMap;

            private void Execute(in UniqueId id, in LocalTransform transform)
            {
                if (!this.ObjectMap.Value.TryGetValue(id, out UnityEngine.Object gameObj)) return;

                Transform gameObjTransform = ((GameObject) gameObj).transform;

                float3 enityPos = transform.Position;
                gameObjTransform.position = new Vector3(enityPos.x, enityPos.y, enityPos.z);

            }
        }

    }
}