using Unity.Entities;

namespace Components.GameEntity.Damage
{
    public struct DmgValue : IComponentData, IEnableableComponent
    {
        public int Value;
    }

}
