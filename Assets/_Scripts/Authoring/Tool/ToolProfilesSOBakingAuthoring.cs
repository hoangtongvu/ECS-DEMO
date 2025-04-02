using Authoring.Utilities.Helpers.GameEntity;
using Components.Tool;
using Core.Tool;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Tool
{
    public class ToolProfilesSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private ToolProfilesSO profilesSO;

        private class Baker : Baker<ToolProfilesSOBakingAuthoring>
        {
            public override void Bake(ToolProfilesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new ToolProfilesSOHolder
                {
                    Value = authoring.profilesSO,
                });

                ProfilesSOBakingHelper.BakeGameEntityProfileElementBuffer(this, authoring.profilesSO, entity);

            }

        }

    }

}
