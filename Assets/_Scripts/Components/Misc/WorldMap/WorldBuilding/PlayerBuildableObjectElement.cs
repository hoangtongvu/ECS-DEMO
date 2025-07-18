using Core.GameEntity;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Components.Misc.WorldMap.WorldBuilding
{
    public struct PlayerBuildableObjectElement : IBufferElementData
    {
        public Entity Entity;
        public FixedString64Bytes Name;
        public UnityObjectRef<Sprite> PreviewSprite;
        public GameEntitySize GameEntitySize;
        public float ObjectHeight;
    }

}
