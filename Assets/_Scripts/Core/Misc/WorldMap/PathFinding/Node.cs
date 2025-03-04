using System;

namespace Core.Misc.WorldMap.PathFinding
{
    public struct Node : IEquatable<Node>, IComparable, IComparable<Node>
    {
        public int ParentNodeIndex;
        public int NodeIndex;
        public int ChunkIndex;
        public bool IsExitOrdered;
        public float GCost;
        public float FCost;

        public override bool Equals(object obj)
        {
            if (obj is Node node)
                return this.Equals(node);

            return base.Equals(obj);
        }

        public static bool operator ==(Node first, Node second) => first.Equals(second);

        public static bool operator !=(Node first, Node second) => !(first == second);

        public bool Equals(Node other) => this.NodeIndex.Equals(other.NodeIndex);

        public override int GetHashCode() => this.NodeIndex.GetHashCode();

        public int CompareTo(object obj)
        {
            if (!(obj is Node))
            {
                throw new ArgumentException(nameof(obj) + " is not a " + nameof(Node));
            }

            return CompareTo((Node)obj);
        }

        public int CompareTo(Node other) => this.FCost.CompareTo(other.FCost);

        public override string ToString()
        {
            return $"{nameof(ParentNodeIndex)}: {ParentNodeIndex}, {nameof(NodeIndex)}: {NodeIndex}, {nameof(ChunkIndex)}: {ChunkIndex}, {nameof(IsExitOrdered)}: {IsExitOrdered}, {nameof(GCost)}: {GCost}, {nameof(FCost)}: {FCost}";
        }

    }

}
