using System;

namespace Core.CustomIdentification
{
    [Serializable]
    public struct UniqueId : IEquatable<UniqueId>
    {

        public UniqueKind Kind;
        public uint Id;

        public override string ToString() => $"{this.Kind}-{this.Id}";

        public bool Equals(UniqueId other) => this.Id == other.Id && this.Kind == other.Kind;

        public override bool Equals(object obj) => obj is UniqueId other && Equals(other);

        public override int GetHashCode()
        {
            ulong kind = (ulong) this.Kind;
            ulong combined = kind << 32 | Id;
            return combined.GetHashCode();

        }


    }

}