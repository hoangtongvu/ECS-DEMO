using Components.GameEntity.EntitySpawning;
using Components.Misc.Presenter;
using Components.Tool;
using Components.Tool.Misc;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace Authoring.Tool
{
    public class DerelictToolAuthoring : MonoBehaviour
    {
        private class Baker : Baker<DerelictToolAuthoring>
        {
            public override void Bake(DerelictToolAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<PhysicsGravityFactor>(entity);

                AddComponent<NewlySpawnedTag>(entity);
                AddComponent<ToolTag>(entity);
                AddComponent<DerelictToolTag>(entity);
                AddComponent<SpawnerEntityHolder>(entity);

                AddComponent<CanBePickedTag>(entity);
                SetComponentEnabled<CanBePickedTag>(entity, false);

                AddComponent(entity, new ToolPickerEntity
                {
                    Value = Entity.Null,
                });

                AddComponent<NeedSpawnPresenterTag>(entity);

            }

        }

    }

}
