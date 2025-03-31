using System;

namespace Core.Misc.Presenter.PresenterPrefabGO
{
    [Serializable]
    public struct PresenterPrefabGOKey : IEquatable<PresenterPrefabGOKey>
    {
        public uint Id;

        public static readonly PresenterPrefabGOKey Null = new()
        {
            Id = 0,
        };

        public static readonly PresenterPrefabGOKey DefaultNotNull = new()
        {
            Id = 1,
        };

        public override bool Equals(object obj)
        {
            if (obj is PresenterPrefabGOKey presenterPrefabGOKey)
            {
                return Equals(presenterPrefabGOKey);
            }

            return base.Equals(obj);
        }

        public static bool operator ==(PresenterPrefabGOKey first, PresenterPrefabGOKey second) => first.Equals(second);

        public static bool operator !=(PresenterPrefabGOKey first, PresenterPrefabGOKey second) => !(first == second);

        public bool Equals(PresenterPrefabGOKey other) => Id.Equals(other.Id);

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}";
        }

    }

}
