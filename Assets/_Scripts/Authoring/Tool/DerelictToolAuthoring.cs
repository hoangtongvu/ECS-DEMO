using Components.GameEntity.EntitySpawning;
using Components.Misc.Presenter;
using Components.Tool;
using Unity.Entities;
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

                AddComponent<NewlySpawnedTag>(entity);
                AddComponent<DerelictToolTag>(entity);
                AddComponent<SpawnerEntityRef>(entity);

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
