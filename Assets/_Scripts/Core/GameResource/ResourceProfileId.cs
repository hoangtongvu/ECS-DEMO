using System;
using System.Runtime.InteropServices;

namespace Core.GameResource
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct ResourceProfileId : IEquatable<ResourceProfileId>
    {
        [FieldOffset(0)] private readonly short _raw; // 2 bytes
        [FieldOffset(0)] public ResourceType ResourceType;
        [FieldOffset(1)] public byte VariantIndex;

        public bool Equals(ResourceProfileId other)
            => this._raw == other._raw;

        public override bool Equals(object obj)
            => obj is ResourceProfileId other && this._raw == other._raw;

        public override int GetHashCode()
            => this._raw.GetHashCode();

        public static bool operator ==(ResourceProfileId left, ResourceProfileId right)
            => left._raw == right._raw;

        public static bool operator !=(ResourceProfileId left, ResourceProfileId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(ResourceType)}: {ResourceType}, {nameof(VariantIndex)}: {VariantIndex}";
        }

    }

}