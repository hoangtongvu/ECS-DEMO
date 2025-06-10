using Components.GameEntity;
using Components.Misc.Presenter;
using Systems.Initialization.Misc.Presenter;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Initialization.GameEntity
{
    // Note: This system run only once in order to set-up presenter entity as child of primary entity **PREFABs**.
    // While **ClearNeedSpawnPresenterTagsSystem** can't remove tags for **PREFABs**, we will remove tags right in this system with ecb.
    [UpdateInGroup(typeof(NeedSpawnPresenterTagProcessSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct SetupPresenterEntitiesSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BakedGameEntityProfileElement>();
            state.RequireForUpdate<NeedSpawnPresenterTag>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

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

                    ecb.RemoveComponent<NeedSpawnPresenterTag>(prefabsElement.PrimaryEntity);

                    this.AddPresenterEntityAsChild(
                        ref state
                        , ecb
                        , in prefabsElement.PrimaryEntity
                        , in prefabsElement.PresenterEntity);

                }

            }

            ecb.Playback(state.EntityManager);

        }

        [BurstCompile]
        private void AddPresenterEntityAsChild(
            ref SystemState state
            , EntityCommandBuffer ecb
            , in Entity primaryEntity
            , in Entity presenterEntity)
        {
            ecb.AddComponent(presenterEntity, new Parent
            {
                Value = primaryEntity,
            });

            var linkedEntityGroup = ecb.AddBuffer<LinkedEntityGroup>(primaryEntity);
            linkedEntityGroup.Add(new()
            {
                Value = primaryEntity,
            });

            linkedEntityGroup.Add(new()
            {
                Value = presenterEntity,
            });

            if (SystemAPI.HasBuffer<LinkedEntityGroup>(presenterEntity))
            {
                var presenterLinkedSystemGroup = SystemAPI.GetBuffer<LinkedEntityGroup>(presenterEntity);

                int length = presenterLinkedSystemGroup.Length;
                if (length == 1) return;

                var presenterLinkedSystemGroupArray = presenterLinkedSystemGroup.ToNativeArray(Allocator.Temp);

                for (int j = 1; j < length; j++)
                {
                    linkedEntityGroup.Add(presenterLinkedSystemGroupArray[j]);
                }

            }

        }

    }

}