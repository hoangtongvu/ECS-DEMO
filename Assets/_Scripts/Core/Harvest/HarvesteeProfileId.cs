using System;
using System.Runtime.InteropServices;

namespace Core.Harvest
{
    [System.Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct HarvesteeProfileId : IEquatable<HarvesteeProfileId>
    {
        [FieldOffset(0)] private ushort _raw;
        [FieldOffset(0)] public HarvesteeType HarvesteeType;
        [FieldOffset(1)] public byte VariantIndex;

        public bool Equals(HarvesteeProfileId other) => this._raw == other._raw;

        public override bool Equals(object obj)
        {
            return obj is HarvesteeProfileId other && this._raw == other._raw;
        }

        public override string ToString()
        {
            return $"{nameof(HarvesteeType)}: {HarvesteeType}, {nameof(VariantIndex)}: {VariantIndex}";
        }

        public override int GetHashCode() => this._raw.GetHashCode();

        public static bool operator ==(HarvesteeProfileId left, HarvesteeProfileId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HarvesteeProfileId left, HarvesteeProfileId right)
        {
            return !(left == right);
        }
    }
}