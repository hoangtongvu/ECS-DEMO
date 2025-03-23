using Unity.Entities;
using UnityEngine;

namespace Core.Misc.WorldMap.WorldBuilding
{
    public struct BuildableObject
    {
        public Entity Entity;
        public UnityObjectRef<Sprite> PreviewSprite;
        public int GridSquareSize;
    }

}
