using Core.GameEntity;
using Core.GameEntity.Damage;
using Unity.Entities;
using UnityEngine;

namespace Components.GameEntity
{
    public struct BakedGameEntityProfileElement : IBufferElementData
    {
        public UnityObjectRef<GameObject> OriginalPresenterGO;
        public Entity PresenterEntity;
        public Entity PrimaryEntity;

        public GameEntitySize GameEntitySize;

        public bool HasHpComponents;
        public HpData HpData;

    }

}
