using System;
using System.Runtime.InteropServices;

namespace Core.Unit
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct UnitId : IEquatable<UnitId>
    {
        [FieldOffset(0)] private readonly short _raw; // 2 bytes
        [FieldOffset(0)] public UnitType UnitType;
        [FieldOffset(4)] public byte VariantIndex;

        public bool Equals(UnitId other)
            => this._raw == other._raw;

        public override bool Equals(object obj)
            => obj is UnitId other && this._raw == other._raw;

        public override int GetHashCode()
            => this._raw.GetHashCode();

        public static bool operator ==(UnitId left, UnitId right)
            => left._raw == right._raw;

        public static bool operator !=(UnitId left, UnitId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(UnitType)}: {UnitType}, {nameof(VariantIndex)}: {VariantIndex}";
        }

    }

}