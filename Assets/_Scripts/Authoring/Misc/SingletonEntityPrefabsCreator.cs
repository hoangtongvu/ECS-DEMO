using Core.Misc;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc
{
    public class SingletonEntityPrefabsCreator : SaiMonoBehaviour
    {
        [SerializeField] private SingletonEntityPrefabsSO singletonEntityPrefabsSO;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            LoadPrefabsSO(out singletonEntityPrefabsSO);
        }

        private void LoadPrefabsSO(out SingletonEntityPrefabsSO singletonEntityPrefabsSO)
        {
            singletonEntityPrefabsSO = Resources.Load<SingletonEntityPrefabsSO>(SingletonEntityPrefabsSO.SOFilePath);
        }

        private class Baker : Baker<SingletonEntityPrefabsCreator>
        {
            public override void Bake(SingletonEntityPrefabsCreator authoring)
            {
                foreach (var prefab in authoring.singletonEntityPrefabsSO.Prefabs)
                {
                    GetEntity(prefab, TransformUsageFlags.Dynamic);
                }

            }
        }

    }

}
