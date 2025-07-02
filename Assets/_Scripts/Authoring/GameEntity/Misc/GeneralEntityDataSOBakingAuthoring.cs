using Components.GameEntity.Misc;
using Core.GameEntity.Misc;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameEntity.Misc
{
    public class GeneralEntityDataSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private GeneralEntityDataSO profilesSO;

        private class Baker : Baker<GeneralEntityDataSOBakingAuthoring>
        {
            public override void Bake(GeneralEntityDataSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new GeneralEntityDataSOHolder
                {
                    Value = authoring.profilesSO,
                });

            }

        }

    }

}
