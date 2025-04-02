using System;
using Unity.Entities;

namespace Components.GameEntity
{
    public struct PrimaryPrefabEntityHolder : IComponentData, IEquatable<Entity>
    {
        public readonly int Index;
        public readonly int Version;

        public PrimaryPrefabEntityHolder(Entity entity)
        {
            this.Index = entity.Index;
            this.Version = entity.Version;
        }

        public override bool Equals(object obj)
        {
            if (obj is Entity primaryPrefabEntityHolder)
            {
                return Equals(primaryPrefabEntityHolder);
            }

            return base.Equals(obj);
        }

        public static bool operator ==(PrimaryPrefabEntityHolder first, Entity second) => first.Equals(second);

        public static bool operator !=(PrimaryPrefabEntityHolder first, Entity second) => !(first == second);

        public bool Equals(Entity other) => Index.Equals(other.Index) && Version.Equals(other.Version);

        public override int GetHashCode() => this.Index.GetHashCode();

        public static implicit operator Entity(PrimaryPrefabEntityHolder primaryPrefabEntityHolder)
        {
            return new Entity
            {
                Index = primaryPrefabEntityHolder.Index,
                Version = primaryPrefabEntityHolder.Version,
            };
        }

    }

}
