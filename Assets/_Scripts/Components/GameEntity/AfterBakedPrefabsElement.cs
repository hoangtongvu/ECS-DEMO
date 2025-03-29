using Unity.Entities;

namespace Components.GameEntity
{
    public struct AfterBakedPrefabsElement : IBufferElementData
    {
        public Entity PresenterEntity;
        public Entity PrimaryEntity;
    }

}
