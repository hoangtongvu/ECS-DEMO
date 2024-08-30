using System;
using System.Runtime.InteropServices;

namespace Core.Harvest
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct HealthId : IEquatable<HealthId>
    {
        [FieldOffset(0)] private readonly long _raw; // 8 bytes
        [FieldOffset(0)] public int Index;
        [FieldOffset(4)] public int Version;

        public bool Equals(HealthId other)
            => this._raw == other._raw;

        public override bool Equals(object obj)
            => obj is HealthId other && this._raw == other._raw;

        public override int GetHashCode()
            => this._raw.GetHashCode();

        public static bool operator ==(HealthId left, HealthId right)
            => left._raw == right._raw;

        public static bool operator !=(HealthId left, HealthId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(Index)}: {Index}, {nameof(Version)}: {Version}";
        }
    }
}