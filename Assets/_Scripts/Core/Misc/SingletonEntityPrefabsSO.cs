using UnityEngine;

namespace Core.Misc
{
    [CreateAssetMenu(fileName = "SingletonEntityPrefabsSO", menuName = "SO/Misc/SingletonEntityPrefabsSO")]
    public class SingletonEntityPrefabsSO : ScriptableObject
    {
        public static string SOFilePath = "Misc/SingletonEntityPrefabsSO";
        public GameObject[] Prefabs;

        [ContextMenu("Load Prefabs")]
        private void LoadPrefabs()
        {
            this.Prefabs = Resources.LoadAll<GameObject>("SingletonEntityPrefabs");
        }

    }

}