using Unity.Entities;
using UnityEngine;

namespace Components.GameEntity.EntitySpawning.SpawningProfiles
{
    public struct LocalSpawningProfilePictureElement : IBufferElementData
    {
        public UnityObjectRef<Sprite> Value;
    }

    public struct LocalSpawningDurationSecondsElement : IBufferElementData
    {
        public float Value;
    }

    public struct LocalSpawningCostElement : IBufferElementData
    {
        public uint Value;
    }

}
