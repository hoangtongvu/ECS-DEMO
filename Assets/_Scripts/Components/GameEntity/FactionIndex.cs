using System;
using Unity.Entities;

namespace Components.GameEntity
{
    public struct FactionIndex : IComponentData, IEquatable<FactionIndex>
    {
        public byte Value;
        public static readonly FactionIndex Neutral = new()
        {
            Value = 0,
        };

        public override bool Equals(object obj)
        {
            if (obj is FactionIndex factionIndex)
            {
                return Equals(factionIndex);
            }

            return base.Equals(obj);
        }

        public static bool operator ==(FactionIndex first, FactionIndex second) => first.Equals(second);

        public static bool operator !=(FactionIndex first, FactionIndex second) => !(first == second);

        public bool Equals(FactionIndex other) => Value.Equals(other.Value);

        public override int GetHashCode() => this.Value.GetHashCode();

    }

}
