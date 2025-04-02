using Core.GameEntity;
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

    }

}
