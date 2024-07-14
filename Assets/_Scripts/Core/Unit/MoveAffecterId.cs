using System.Runtime.InteropServices;
using System;

namespace Core.Unit
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct MoveAffecterId : IEquatable<MoveAffecterId>
    {

        [FieldOffset(0)] private readonly int _raw; // 4 bytes

        [FieldOffset(0)] public readonly UnitType Unit; // 1 byte
        [FieldOffset(1)] public readonly MoveAffecter MoveAffecter; // 1 byte
        [FieldOffset(2)] public readonly ushort LocalIndex; // 2 bytes

        public MoveAffecterId(UnitType unit, MoveAffecter moveAffecter) : this()
        {
            this.Unit = unit;
            this.MoveAffecter = moveAffecter;
        }

        public MoveAffecterId(UnitType unit, MoveAffecter moveAffecter, ushort localIndex) : this()
        {
            this.Unit = unit;
            this.MoveAffecter = moveAffecter;
            this.LocalIndex = localIndex;
        }


        public bool Equals(MoveAffecterId other)
            => this._raw == other._raw;

        public override bool Equals(object obj)
            => obj is MoveAffecterId other && this._raw == other._raw;

        public override int GetHashCode()
            => this._raw.GetHashCode();

        public static bool operator ==(MoveAffecterId left, MoveAffecterId right)
            => left._raw == right._raw;

        public static bool operator !=(MoveAffecterId left, MoveAffecterId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(Unit)}: {Unit}, {nameof(MoveAffecter)}: {MoveAffecter}, {nameof(LocalIndex)}: {LocalIndex}";
        }

    }
}