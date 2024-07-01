using Core.MyEntity;
using UnityEngine;

namespace Core.Unit
{
    [CreateAssetMenu(fileName = "UnitProfile", menuName = "SO/MyEntity/UnitProfile")]
    public class UnitProfileSO : EntityProfileSO
    {
        [Header("Unit Profile")]
        public UnitType UnitType;
    }
}