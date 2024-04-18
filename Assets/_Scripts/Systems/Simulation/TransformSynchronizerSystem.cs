using Components.ComponentMap;
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

            if (!SystemAPI.ManagedAPI.TryGetSingleton<UnityTransformMap>(out UnityTransformMap transformMap))
            {
                Debug.LogError("UnityTransformMap Singleton not found");
                return;
            }

            //new SyncJob
            //{
            //    ObjectMap = objectMap,
            //}.Schedule();

            this.SyncFunc(transformMap);

        }

        private void SyncFunc(in UnityTransformMap transformMap)
        {
            foreach (var (idRef, transformRef) in
                SystemAPI.Query<RefRO<UniqueId>, RefRO<LocalTransform>>())
            {
                if (!transformMap.Value.TryGetValue(idRef.ValueRO, out UnityEngine.Transform unityTransform)) continue;

                Transform gameObjTransform = unityTransform;

                float3 enityPos = transformRef.ValueRO.Position;
                gameObjTransform.position = new Vector3(enityPos.x, enityPos.y, enityPos.z);
            }
        }

        private partial struct SyncJob : IJobEntity
        {
            public UnityTransformMap TransformMap;

            private void Execute(in UniqueId id, in LocalTransform transform)
            {
                if (!this.TransformMap.Value.TryGetValue(id, out UnityEngine.Transform unityTransform)) return;

                Transform gameObjTransform = unityTransform;

                float3 enityPos = transform.Position;
                gameObjTransform.position = new Vector3(enityPos.x, enityPos.y, enityPos.z);

            }
        }

    }
}