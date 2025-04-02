using Unity.Entities;
using Components;
using Core.MyEvent.PubSub.Messengers;
using ZBase.Foundation.PubSub;
using Core.MyEvent.PubSub.Messages;
using Components.GameEntity.EntitySpawning;

namespace Systems.Simulation.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class SetSpawnCountTextSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    EntitySpawningProfileElement
                    , UISpawned>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            foreach (var (spawningProfiles, uiSpawnedRef, spawnerEntity) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , RefRO<UISpawned>>()
                    .WithEntityAccess())
            {
                if (!uiSpawnedRef.ValueRO.IsSpawned) continue;

                int profileCount = spawningProfiles.Length;

                for (int i = 0; i < profileCount; i++)
                {
                    var profile = spawningProfiles[i];

                    if (!profile.SpawnCount.ValueChanged) continue;
                    this.UpdateUI(in spawnerEntity, i, in profile);
                }

            }

        }

        private void UpdateUI(in Entity spawnerEntity, int profileIndex, in EntitySpawningProfileElement profile)
        {
            GameplayMessenger.MessagePublisher.Publish(new SetIntTextMessage
            {
                SpawnerEntity = spawnerEntity,
                SpawningProfileElementIndex = profileIndex,
                Value = profile.SpawnCount.Value,
            });
        }

    }

}