using Core.MyEntity;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Core.Unit.MyMoveCommand;

namespace Core.Unit
{
    [CreateAssetMenu(fileName = "UnitProfile", menuName = "SO/MyEntity/UnitProfile")]
    public class UnitProfileSO : EntityProfileSO
    {
        [Header("Unit Profile")]
        public UnitType UnitType;

        [SerializedDictionary("MoveAffecter", "Priority")]
        public SerializedDictionary<MoveAffecter, byte> MoveAffecterPriorities;

        [SerializedDictionary("MoveCommand", "Priority")]
        public SerializedDictionary<MoveCommand, byte> MoveCommandPriorities;

    }
}