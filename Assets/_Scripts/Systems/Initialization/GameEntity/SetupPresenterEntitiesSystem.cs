using Components.GameEntity;
using Components.Misc.Presenter;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Initialization.GameEntity
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SetupPresenterEntitiesSystem : SystemBase
    {

        protected override void OnCreate()
        {
            this.RequireForUpdate<AfterBakedPrefabsElement>();
            this.RequireForUpdate<NeedSpawnPresenterTag>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var afterBakedPrefabsElements in
                SystemAPI.Query<
                    DynamicBuffer<AfterBakedPrefabsElement>>())
            {
                int count = afterBakedPrefabsElements.Length;

                for (int i = 0; i < count; i++)
                {
                    var prefabsElement = afterBakedPrefabsElements[i];

                    if (prefabsElement.PresenterEntity == Entity.Null) continue;
                    if (!SystemAPI.HasComponent<NeedSpawnPresenterTag>(prefabsElement.PrimaryEntity)) continue;
                    if (!SystemAPI.IsComponentEnabled<NeedSpawnPresenterTag>(prefabsElement.PrimaryEntity)) continue;

                    var children = entityCommandBuffer.AddBuffer<Child>(prefabsElement.PrimaryEntity);

                    children.Add(new()
                    {
                        Value = prefabsElement.PresenterEntity,
                    });

                    SystemAPI.SetComponentEnabled<NeedSpawnPresenterTag>(prefabsElement.PrimaryEntity, false);

                    entityCommandBuffer.AddComponent(prefabsElement.PresenterEntity, new Parent
                    {
                        Value = prefabsElement.PrimaryEntity,
                    });

                    var linkedEntityGroup = entityCommandBuffer.AddBuffer<LinkedEntityGroup>(prefabsElement.PrimaryEntity);
                    linkedEntityGroup.Add(new()
                    {
                        Value = prefabsElement.PrimaryEntity,
                    });

                    linkedEntityGroup.Add(new()
                    {
                        Value = prefabsElement.PresenterEntity,
                    });

                }

            }

            entityCommandBuffer.Playback(this.EntityManager);
            entityCommandBuffer.Dispose();

        }

    }

}