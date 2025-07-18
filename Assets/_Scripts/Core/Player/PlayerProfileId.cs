using System;
using System.Runtime.InteropServices;

namespace Core.Player
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct PlayerProfileId : IEquatable<PlayerProfileId>
    {
        [FieldOffset(0)] private readonly byte _raw;
        [FieldOffset(0)] public byte VariantIndex;

        public bool Equals(PlayerProfileId other)
            => this._raw == other._raw;

        public override bool Equals(object obj)
            => obj is PlayerProfileId other && this._raw == other._raw;

        public override int GetHashCode()
            => this._raw.GetHashCode();

        public static bool operator ==(PlayerProfileId left, PlayerProfileId right)
            => left._raw == right._raw;

        public static bool operator !=(PlayerProfileId left, PlayerProfileId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(VariantIndex)}: {VariantIndex}";
        }

    }

}