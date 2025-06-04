using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;
using Core.GameEntity.Misc;

namespace Core.GameEntity.Movement.MoveCommand
{
    [System.Serializable]
    public class PrioritiesElement
    {
        [SerializedDictionary("MoveCommandSource", "PlaceHolder")]
        public SerializedDictionary<Key, PlaceHolderValue> Value;

        [System.Serializable]
        public struct Key : IEquatable<Key>
        {
            public MoveCommandSource MoveCommandSource;
            public byte Priority;

            public bool Equals(Key other)
            {
                return this.MoveCommandSource == other.MoveCommandSource
                    || this.Priority == other.Priority;
            }

            public override int GetHashCode() => 0;

        }

        [System.Serializable]
        public struct PlaceHolderValue
        {
        }

    }

    [CreateAssetMenu(fileName = "MoveCommandPrioritiesSO", menuName = "SO/GameEntity/Unit/MoveCommandPrioritiesSO")]
    public class MoveCommandPrioritiesSO : ScriptableObject
    {
        public static readonly string DefaultAssetPath = "Misc/UnitProfilesSO";

        [SerializedDictionary("ArmedState", "PrioritiesElement")]
        public SerializedDictionary<ArmedState, PrioritiesElement> MoveCommandSourcePriorities;

    }

}