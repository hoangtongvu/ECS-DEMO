using System.Runtime.InteropServices;
using System;
using Core.GameResource;

namespace Core.Unit
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct UnitCostId : IEquatable<UnitCostId>
    {

        [FieldOffset(0)] private readonly int _raw; // 4 bytes

        [FieldOffset(0)] public readonly UnitType Unit; // 1 byte
        [FieldOffset(1)] public readonly ResourceType Cost; // 1 byte
        [FieldOffset(2)] public readonly ushort LocalIndex; // 2 bytes

        public UnitCostId(UnitType unit, ResourceType cost) : this()
        {
            this.Unit = unit;
            this.Cost = cost;
        }

        public UnitCostId(UnitType unit, ResourceType cost, ushort localIndex) : this()
        {
            this.Unit = unit;
            this.Cost = cost;
            this.LocalIndex = localIndex;
        }


        public bool Equals(UnitCostId other)
            => this._raw == other._raw;

        public override bool Equals(object obj)
            => obj is UnitCostId other && this._raw == other._raw;

        public override int GetHashCode()
            => this._raw.GetHashCode();

        public static bool operator ==(UnitCostId left, UnitCostId right)
            => left._raw == right._raw;

        public static bool operator !=(UnitCostId left, UnitCostId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(Unit)}: {Unit}, {nameof(Cost)}: {Cost}, {nameof(LocalIndex)}: {LocalIndex}";
        }

    }
}