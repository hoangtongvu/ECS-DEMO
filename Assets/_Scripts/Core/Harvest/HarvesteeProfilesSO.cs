using Core.GameEntity;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Harvest
{
    [System.Serializable]
    public class HarvesteeProfileElement : GameEntityProfileElement
    {
        [Header("Harvestee ProfileElement")]
        public HarvesteeType HarvesteeType;
        public ResourceDropInfo ResourceDropInfo;
        public List<GameObject> PresenterVariances;
    }

    [CreateAssetMenu(fileName = "HarvesteeProfilesSO", menuName = "SO/GameEntity/HarvesteeProfilesSO")]
    public class HarvesteeProfilesSO : GameEntityProfilesSO<HarvesteeProfileId, HarvesteeProfileElement>
    {
        public static readonly string DefaultAssetPath = "Misc/HarvesteeProfilesSO";
    }

}