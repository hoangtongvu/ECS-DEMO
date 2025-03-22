using UnityEngine;

namespace Core.Misc.WorldMap.WorldBuilding
{
    [System.Serializable]
    public class BuildableObjectSOElement
    {
        public GameObject Prefab;
        public string Name;
        public Sprite PreviewSprite;
        public int GridSquareSize;
    }

}
