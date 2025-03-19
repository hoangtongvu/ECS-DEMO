using Unity.Entities;

namespace Components.Misc.WorldMap.WorldBuilding
{
    public struct BuildableObjectChoiceIndex : IComponentData
    {
        public const int NoChoice = -1;
        public int Value;
    }

}
