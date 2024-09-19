using System;
using System.Runtime.InteropServices;

namespace Core.Harvest
{
    [System.Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct HarvesteeProfileId : IEquatable<HarvesteeProfileId>
    {
        [FieldOffset(0)] public byte LocalIndex;

        public bool Equals(HarvesteeProfileId other) => this.LocalIndex == other.LocalIndex;

        public override bool Equals(object obj)
        {
            return obj is HarvesteeProfileId other && this.LocalIndex == other.LocalIndex;
        }

        public override string ToString() => $"{this.LocalIndex}";

        public override int GetHashCode() => this.LocalIndex.GetHashCode();

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