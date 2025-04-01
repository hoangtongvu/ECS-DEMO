using System;
using System.Runtime.InteropServices;

namespace Core.Misc.WorldMap.WorldBuilding
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct PlayerBuildingProfileId : IEquatable<PlayerBuildingProfileId>
    {
        [FieldOffset(0)] private readonly byte _raw;
        [FieldOffset(0)] public byte VariantIndex;

        public bool Equals(PlayerBuildingProfileId other)
            => this._raw == other._raw;

        public override bool Equals(object obj)
            => obj is PlayerBuildingProfileId other && this._raw == other._raw;

        public override int GetHashCode()
            => this._raw.GetHashCode();

        public static bool operator ==(PlayerBuildingProfileId left, PlayerBuildingProfileId right)
            => left._raw == right._raw;

        public static bool operator !=(PlayerBuildingProfileId left, PlayerBuildingProfileId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(VariantIndex)}: {VariantIndex}";
        }

    }

}