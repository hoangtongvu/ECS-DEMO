using System;
using System.Runtime.InteropServices;

namespace Core.Unit
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct UnitProfileId : IEquatable<UnitProfileId>
    {
        [FieldOffset(0)] private readonly short _raw; // 2 bytes
        [FieldOffset(0)] public UnitType UnitType;
        [FieldOffset(1)] public byte VariantIndex;

        public bool Equals(UnitProfileId other)
            => this._raw == other._raw;

        public override bool Equals(object obj)
            => obj is UnitProfileId other && this._raw == other._raw;

        public override int GetHashCode()
            => this._raw.GetHashCode();

        public static bool operator ==(UnitProfileId left, UnitProfileId right)
            => left._raw == right._raw;

        public static bool operator !=(UnitProfileId left, UnitProfileId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(UnitType)}: {UnitType}, {nameof(VariantIndex)}: {VariantIndex}";
        }

    }

}