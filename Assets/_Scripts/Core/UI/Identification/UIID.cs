using System;

namespace Core.UI.Identification
{
    [System.Serializable]
    public struct UIID : IEquatable<UIID>
    {
        public UIType Type;
        public uint LocalId;

        public override bool Equals(object obj) => obj is UIID other && this.Equals(other);

        public bool Equals(UIID other) => this.Type == other.Type && this.LocalId == other.LocalId;

        public override string ToString() => $"{this.Type}-{this.LocalId}";


        public override int GetHashCode()
        {
            ulong kind = (ulong)this.Type;
            ulong combined = kind << 32 | LocalId;
            return combined.GetHashCode();

        }

    }
}