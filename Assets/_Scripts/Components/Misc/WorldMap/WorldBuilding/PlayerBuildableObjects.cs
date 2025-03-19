using Unity.Entities;
using UnityEngine;

namespace Components.Misc.WorldMap.WorldBuilding
{
    public struct PlayerBuildableObjectElement : IBufferElementData
    {
        public Entity Entity;
        public UnityObjectRef<Sprite> PreviewSprite;
        public int SquareRadius;
    }

}
