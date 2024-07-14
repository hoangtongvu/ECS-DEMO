using Core.MyEntity;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace Core.Unit
{
    [CreateAssetMenu(fileName = "UnitProfile", menuName = "SO/MyEntity/UnitProfile")]
    public class UnitProfileSO : EntityProfileSO
    {
        [Header("Unit Profile")]
        public UnitType UnitType;

        [SerializedDictionary("MoveAffecter", "Priority")]
        public SerializedDictionary<MoveAffecter, byte> MoveAffecterPriorities;
    }
}