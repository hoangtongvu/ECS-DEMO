using System;
using System.Runtime.InteropServices;

namespace Core.Misc.WorldMap.WorldBuilding
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct GameBuildingProfileId : IEquatable<GameBuildingProfileId>
    {
        [FieldOffset(0)] private readonly byte _raw;
        [FieldOffset(0)] public byte VariantIndex;

        public bool Equals(GameBuildingProfileId other)
            => this._raw == other._raw;

        public override bool Equals(object obj)
            => obj is GameBuildingProfileId other && this._raw == other._raw;

        public override int GetHashCode()
            => this._raw.GetHashCode();

        public static bool operator ==(GameBuildingProfileId left, GameBuildingProfileId right)
            => left._raw == right._raw;

        public static bool operator !=(GameBuildingProfileId left, GameBuildingProfileId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(VariantIndex)}: {VariantIndex}";
        }

    }

}