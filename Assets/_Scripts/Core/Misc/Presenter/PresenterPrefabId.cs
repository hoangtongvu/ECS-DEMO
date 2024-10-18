using System.Runtime.InteropServices;
using System;

namespace Core.Misc.Presenter
{
    [System.Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct PresenterPrefabId : IEquatable<PresenterPrefabId>
    {
       
        [FieldOffset(0)] private readonly ushort _raw;
        [FieldOffset(0)] public PresenterType PresenterType;
        [FieldOffset(1)] public byte LocalIndex;


        public bool Equals(PresenterPrefabId other)
            => _raw == other._raw;

        public override bool Equals(object obj)
            => obj is PresenterPrefabId other && _raw == other._raw;

        public override int GetHashCode()
            => _raw.GetHashCode();

        public static bool operator ==(PresenterPrefabId left, PresenterPrefabId right)
            => left._raw == right._raw;

        public static bool operator !=(PresenterPrefabId left, PresenterPrefabId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(PresenterType)}: {PresenterType}, {nameof(LocalIndex)}: {LocalIndex}";
        }


    }
}