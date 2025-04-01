using Core.GameEntity;
using UnityEngine;

namespace Core.Harvest
{
    [System.Serializable]
    public class HarvesteeProfileElement : GameEntityProfileElement
    {
        [Header("Harvestee ProfileElement")]
        public HarvesteeType HarvesteeType;
        public uint MaxHp;
        public ResourceDropInfo ResourceDropInfo;

    }

    [CreateAssetMenu(fileName = "HarvesteeProfilesSO", menuName = "SO/GameEntity/HarvesteeProfilesSO")]
    public class HarvesteeProfilesSO : GameEntityProfilesSO<HarvesteeProfileId, HarvesteeProfileElement>
    {
        public static readonly string DefaultAssetPath = "Misc/HarvesteeProfilesSO";
    }

}