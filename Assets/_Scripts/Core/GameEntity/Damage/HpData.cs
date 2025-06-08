using System;
using System.Runtime.InteropServices;

namespace Core.GameEntity.Damage
{
    [System.Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct HpData : IEquatable<HpData>
    {
        [FieldOffset(0)] public readonly int raw;
        [FieldOffset(0)] public int MaxHp;

        public HpData()
        {
            this.raw = 0;
            this.MaxHp = 100;
        }

        public override bool Equals(object obj)
        {
            if (obj is HpData hpData)
            {
                return Equals(hpData);
            }

            return base.Equals(obj);
        }

        public static bool operator ==(HpData first, HpData second) => first.Equals(second);

        public static bool operator !=(HpData first, HpData second) => !(first == second);

        public readonly bool Equals(HpData other) => this.raw.Equals(other.raw);
            
        public override readonly int GetHashCode() => this.raw.GetHashCode();

    }
}