using System;
using Unity.Mathematics;

namespace Core.Misc.WorldMap.LineCaching
{
    public struct LineCacheKey : IEquatable<LineCacheKey>
    {
        public int2 Delta;

        public override bool Equals(object obj)
        {
            if (obj is LineCacheKey lineCacheKey)
                return this.Equals(lineCacheKey);

            return base.Equals(obj);
        }

        public static bool operator ==(LineCacheKey first, LineCacheKey second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(LineCacheKey first, LineCacheKey second)
        {
            return !(first == second);
        }

        public bool Equals(LineCacheKey other) => this.Delta.Equals(other.Delta);

        public override int GetHashCode() => this.Delta.GetHashCode();

        public override string ToString() => $"{Delta}";

    }

}
