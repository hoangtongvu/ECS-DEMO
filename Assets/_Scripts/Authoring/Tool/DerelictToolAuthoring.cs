using Components.Misc;
using Components.Misc.Presenter;
using Components.MyEntity.EntitySpawning;
using Components.Tool;
using Core.Tool;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Tool
{
    public class DerelictToolAuthoring : MonoBehaviour
    {
        [SerializeField] private ToolType toolType;
        [SerializeField] private uint baseDmg = 5;
        [SerializeField] private float baseWorkSpeed = 0.5f;

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

                AddComponent(entity, new ToolTypeICD
                {
                    Value = authoring.toolType,
                });

                AddComponent(entity, new BaseDmg
                {
                    Value = authoring.baseDmg,
                });
                AddComponent(entity, new BaseWorkSpeed
                {
                    Value = authoring.baseWorkSpeed,
                });

                AddComponent<NeedSpawnPresenterTag>(entity);

            }

        }

    }

}
