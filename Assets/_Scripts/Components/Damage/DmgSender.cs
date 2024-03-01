using Unity.Entities;

namespace Components.Damage
{
    public struct DmgValue : IComponentData, IEnableableComponent
    {
        public int Value;
    }

}
