using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace Core.Misc.WorldMap.ChunkInnerPathCost
{
    [StructLayout(LayoutKind.Explicit)]
    public struct InnerPathKey : IEquatable<InnerPathKey>
    {
        [FieldOffset(0)] private readonly int2 raw;
        [FieldOffset(0)] private int FirstCellMapIndex;
        [FieldOffset(4)] private int SecondCellMapIndex;


        public InnerPathKey(int firstCellMapIndex, int secondCellMapIndex) : this()
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

        public static bool operator ==(InnerPathKey first, InnerPathKey second) => first.Equals(second);

        public static bool operator !=(InnerPathKey first, InnerPathKey second) => !(first == second);

        public bool Equals(InnerPathKey other) => this.raw.Equals(other.raw);

        public override int GetHashCode() => this.raw.GetHashCode();

        public override string ToString() => $"{raw}";

    }

}
