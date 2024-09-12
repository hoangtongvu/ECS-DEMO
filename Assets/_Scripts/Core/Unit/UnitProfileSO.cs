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

        [SerializedDictionary("MoveCommandSource", "Priority")]
        public SerializedDictionary<MoveCommandSource, byte> MoveCommandSourcePriorities;

    }
}