using Core.Harvest;
using System;
using System.Runtime.InteropServices;

namespace Core.Tool
{
    [System.Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct ToolHarvesteePairId : IEquatable<ToolHarvesteePairId>
    {
        [FieldOffset(0)] private ushort _raw;
        [FieldOffset(0)] public ToolType ToolType;
        [FieldOffset(1)] public HarvesteeType HarvesteeType;

        public bool Equals(ToolHarvesteePairId other) => this._raw == other._raw;

        public override bool Equals(object obj)
        {
            return obj is ToolHarvesteePairId other && this._raw == other._raw;
        }

        public override string ToString()
        {
            return $"{nameof(HarvesteeType)}: {HarvesteeType}, {nameof(ToolType)}: {ToolType}";
        }

        public override int GetHashCode() => this._raw.GetHashCode();

        public static bool operator ==(ToolHarvesteePairId left, ToolHarvesteePairId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ToolHarvesteePairId left, ToolHarvesteePairId right)
        {
            return !(left == right);
        }
    }
}