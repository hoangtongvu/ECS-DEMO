using Components.GameEntity;
using Components.Misc.Presenter;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Initialization.GameEntity
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial class SetupPresenterEntitiesSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<BakedGameEntityProfileElement>();
            this.RequireForUpdate<NeedSpawnPresenterTag>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var bakedProfiles in
                SystemAPI.Query<
                    DynamicBuffer<BakedGameEntityProfileElement>>())
            {
                int count = bakedProfiles.Length;

                for (int i = 0; i < count; i++)
                {
                    var prefabsElement = bakedProfiles[i];

                    if (prefabsElement.PresenterEntity == Entity.Null) continue;
                    if (!SystemAPI.HasComponent<NeedSpawnPresenterTag>(prefabsElement.PrimaryEntity)) continue;
                    if (!SystemAPI.IsComponentEnabled<NeedSpawnPresenterTag>(prefabsElement.PrimaryEntity)) continue;

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

                    if (SystemAPI.HasBuffer<LinkedEntityGroup>(prefabsElement.PresenterEntity))
                    {
                        var presenterLinkedSystemGroup = SystemAPI.GetBuffer<LinkedEntityGroup>(prefabsElement.PresenterEntity);

                        int length = presenterLinkedSystemGroup.Length;
                        if (length == 1) continue;

                        var presenterLinkedSystemGroupArray = presenterLinkedSystemGroup.ToNativeArray(Allocator.Temp);

                        for (int j = 1; j < length; j++)
                        {
                            linkedEntityGroup.Add(presenterLinkedSystemGroupArray[j]);
                        }
                        
                        presenterLinkedSystemGroupArray.Dispose();
                    }

                }

            }

            entityCommandBuffer.Playback(this.EntityManager);
            entityCommandBuffer.Dispose();

        }

    }

}