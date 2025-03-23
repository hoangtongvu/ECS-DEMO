using Core.Misc.WorldMap.ChunkInnerPathCost;
using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace Core.Misc.WorldMap
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ChunkExit : IEquatable<ChunkExit>
    {
        [FieldOffset(0)] private readonly int2 raw;
        [FieldOffset(0)] public readonly int FirstCellMapIndex;
        [FieldOffset(4)] public readonly int SecondCellMapIndex;


        public ChunkExit(int firstCellMapIndex, int secondCellMapIndex) : this()
        {
            (this.FirstCellMapIndex, this.SecondCellMapIndex) = firstCellMapIndex <= secondCellMapIndex
                ? (firstCellMapIndex, secondCellMapIndex)
                : (secondCellMapIndex, firstCellMapIndex);

        }

        public override bool Equals(object obj)
        {
            if (obj is InnerPathKey lineCacheKey)
                return this.Equals(lineCacheKey);

            return base.Equals(obj);
        }

        public static bool operator ==(ChunkExit first, ChunkExit second) => first.Equals(second);

        public static bool operator !=(ChunkExit first, ChunkExit second) => !(first == second);

        public bool Equals(ChunkExit other) => this.raw.Equals(other.raw);

        public override int GetHashCode() => this.raw.GetHashCode();

        public override string ToString() => $"{raw}";

    }

}
