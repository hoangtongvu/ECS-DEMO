using Unity.Entities;

namespace Components.GameEntity.Damage
{
    /// <summary>
    /// Positive number -> healing, negative number -> taking damage.
    /// </summary>
    public struct HpChangeRecordElement : IBufferElementData
    {
        public int Value;
    }
}
