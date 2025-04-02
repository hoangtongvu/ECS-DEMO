using UnityEngine;

namespace Core.GameEntity
{
    [System.Serializable]
    public abstract class GameEntityProfileElement
    {
        [Header("GameEntity ProfileElement")]
        public string Name;
        public Sprite ProfilePicture;

        public GameObject PresenterPrefab;
        public bool IsPresenterEntity;

        public GameObject PrimaryEntityPrefab;

        public GameEntitySize GameEntitySize;

    }

}