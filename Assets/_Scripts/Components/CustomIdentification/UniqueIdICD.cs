using Core.CustomIdentification;
using System;
using Unity.Entities;

namespace Components.CustomIdentification
{
    [Serializable]
    public struct UniqueIdICD : IEquatable<UniqueIdICD>, IComponentData
    {

        public UniqueId BaseId;

        public override string ToString() => this.BaseId.ToString();

        public bool Equals(UniqueIdICD other) => this.BaseId.Equals(other.BaseId);

        public override bool Equals(object obj) => obj is UniqueIdICD other && Equals(other);

        public override int GetHashCode() => this.BaseId.GetHashCode();

    }

}