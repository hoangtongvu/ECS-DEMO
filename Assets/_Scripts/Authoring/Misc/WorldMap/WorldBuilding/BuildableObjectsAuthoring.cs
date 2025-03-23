using Components.Misc.WorldMap.WorldBuilding;
using Core;
using Core.Misc.WorldMap.WorldBuilding;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.WorldMap.WorldBuilding
{
    public class BuildableObjectsAuthoring : SaiMonoBehaviour
    {
        [SerializeField] private BuildableObjectsSO buildableObjectsSO;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadSO();
        }

        private void LoadSO()
        {
            this.buildableObjectsSO = Resources.Load<BuildableObjectsSO>(BuildableObjectsSO.DefaultAssetPath);
        }

        private class Baker : Baker<BuildableObjectsAuthoring>
        {
            public override void Bake(BuildableObjectsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new BuildableObjectChoiceIndex
                {
                    Value = BuildableObjectChoiceIndex.NoChoice,
                });

                var buffer = AddBuffer<PlayerBuildableObjectElement>(entity);

                foreach (var buildableObjectData in authoring.buildableObjectsSO.BuildableObjects)
                {
                    buffer.Add(new()
                    {
                        Entity = GetEntity(buildableObjectData.Prefab, TransformUsageFlags.None),
                        Name = buildableObjectData.Name,
                        PreviewSprite = buildableObjectData.PreviewSprite,
                        GridSquareSize = buildableObjectData.GridSquareSize,
                        ObjectHeight = buildableObjectData.ObjectHeight,
                    });
                }

            }

        }

    }

}
