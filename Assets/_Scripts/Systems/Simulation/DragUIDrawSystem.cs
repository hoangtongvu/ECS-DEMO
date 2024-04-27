using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Components;
using Components.CustomIdentification;
using Components.ComponentMap;
using Core.Extensions;

namespace Systems.Simulation
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class DragUIDrawSystem : SystemBase
    {
        private float3 startPos;
        private Transform spriteTransform;

        protected override void OnStartRunning()
        {
            var transformMap = SystemAPI.ManagedAPI.GetSingleton<UnityTransformMap>();

            transformMap.Value.TryGetValue(
                new UniqueId
                {
                    Id = 1,
                    Kind = UniqueKind.None
                }
                , out this.spriteTransform);
        }

        protected override void OnUpdate()
        {
            var dragSelectionData = SystemAPI.GetSingleton<DragSelectionData>();

            if (!dragSelectionData.IsDragging)
            {
                this.spriteTransform.localScale = float3.zero;
                return;
            }

            this.startPos = dragSelectionData.StartWorldPos;

            float3 currentPos = dragSelectionData.CurrentWorldPos;

            float3 centerPos = math.lerp(this.startPos, currentPos, 0.5f);

            float2 size =
                new float2(this.startPos.x, this.startPos.z) -
                new float2(currentPos.x, currentPos.z);

            // set sprite pos = centerPos.
            this.spriteTransform.position = centerPos.Add(y: 0.05f);

            float3 tempScale = new(size.x, size.y, 0);
            this.spriteTransform.localScale = tempScale * 100;
        }


    }
}