using Core.GameEntity.Damage;
using System;
using Unity.Entities;

namespace Components.GameEntity.Damage
{
    public struct HpDataHolder : ISharedComponentData, IEquatable<HpDataHolder>
    {
        public HpData Value;

        public override bool Equals(object obj)
        {
            if (obj is HpDataHolder hpDataHolder)
            {
                return Equals(hpDataHolder);
            }

            return base.Equals(obj);
        }

        public static bool operator ==(HpDataHolder first, HpDataHolder second) => first.Equals(second);

        public static bool operator !=(HpDataHolder first, HpDataHolder second) => !(first == second);

        public bool Equals(HpDataHolder other) => this.Value.Equals(other.Value);

        public override int GetHashCode() => this.Value.GetHashCode();

    }

}
