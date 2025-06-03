using Core.GameEntity.Misc;
using Unity.Entities;

namespace Components.GameEntity.Misc
{
    public struct ArmedStateHolder : IComponentData
    {
        public ArmedState Value;
    }

}
