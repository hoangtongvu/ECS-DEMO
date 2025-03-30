using System;
using System.Runtime.InteropServices;

namespace Core.Tool
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct ToolId : IEquatable<ToolId>
    {
        [FieldOffset(0)] private readonly short _raw; // 2 bytes
        [FieldOffset(0)] public ToolType ToolType;
        [FieldOffset(4)] public byte VariantIndex;

        public bool Equals(ToolId other)
            => this._raw == other._raw;

        public override bool Equals(object obj)
            => obj is ToolId other && this._raw == other._raw;

        public override int GetHashCode()
            => this._raw.GetHashCode();

        public static bool operator ==(ToolId left, ToolId right)
            => left._raw == right._raw;

        public static bool operator !=(ToolId left, ToolId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(ToolType)}: {ToolType}, {nameof(VariantIndex)}: {VariantIndex}";
        }

    }

}