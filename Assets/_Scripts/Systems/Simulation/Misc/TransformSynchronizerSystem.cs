using Components.ComponentMap;
using Components.CustomIdentification;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Simulation.Misc
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class TransformSynchronizerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<UniqueIdICD>();
            this.RequireForUpdate<LocalTransform>();
            this.RequireForUpdate<UnityTransformMap>();
        }

        protected override void OnUpdate()
        {
            //Sync Transform from Entity world to corresponding Transform in GameObject world.

            if (!SystemAPI.ManagedAPI.TryGetSingleton<UnityTransformMap>(out UnityTransformMap transformMap))
            {
                Debug.LogError("UnityTransformMap Singleton not found");
                return;
            }

            this.SyncFunc(transformMap);

        }

        private void SyncFunc(in UnityTransformMap transformMap)
        {
            foreach (var (idRef, transformRef) in
                SystemAPI.Query<RefRO<UniqueIdICD>, RefRO<LocalTransform>>())
            {
                if (!transformMap.Value.TryGetValue(idRef.ValueRO, out UnityEngine.Transform unityTransform)) continue;

                Transform gameObjTransform = unityTransform;

                float3 enityPos = transformRef.ValueRO.Position;
                gameObjTransform.position = new Vector3(enityPos.x, enityPos.y, enityPos.z);

                gameObjTransform.transform.rotation = transformRef.ValueRO.Rotation;

            }

        }

    }

}