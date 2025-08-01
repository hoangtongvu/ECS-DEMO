using System;
using System.Runtime.InteropServices;

namespace Core.Animator.AnimationEvents
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct AnimationEventChannel : IEquatable<AnimationEventChannel>
    {
        [FieldOffset(0)] private readonly int _raw;
        [FieldOffset(0)] public int Index;

        public bool Equals(AnimationEventChannel other)
            => this._raw == other._raw;

        public override bool Equals(object obj)
            => obj is AnimationEventChannel other && this._raw == other._raw;

        public override int GetHashCode()
            => this._raw.GetHashCode();

        public static bool operator ==(AnimationEventChannel left, AnimationEventChannel right)
            => left._raw == right._raw;

        public static bool operator !=(AnimationEventChannel left, AnimationEventChannel right)
            => left._raw != right._raw;

    }

}