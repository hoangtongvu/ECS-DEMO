using UnityEngine;

namespace Core.GameEntity.Misc
{
    [CreateAssetMenu(fileName = "GeneralEntityDataSO", menuName = "SO/GameEntity/GeneralEntityDataSO")]
    public class GeneralEntityDataSO : ScriptableObject
    {
        public Material FlashOnTakeHitMaterial;
    }

}