using Unity.Entities;

namespace Components
{

    public struct SelfEntityRef : IComponentData
    {
        public Entity Value;
    }

}
