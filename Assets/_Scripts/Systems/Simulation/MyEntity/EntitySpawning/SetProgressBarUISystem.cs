using Unity.Entities;
using Components;
using Components.MyEntity.EntitySpawning;
using Core.MyEvent.PubSub.Messengers;
using ZBase.Foundation.PubSub;
using Core.MyEvent.PubSub.Messages;
using Components.Unit.UnitSelection;

namespace Systems.Simulation.MyEntity.EntitySpawning
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SetProgressBarUISystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitSelectedTag
                    , EntitySpawningProfileElement
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
                    .WithAll<UnitSelectedTag>()
                    .WithEntityAccess())
            {
                if (!uiSpawnedRef.ValueRO.IsSpawned) continue;

                int profileCount = spawningProfiles.Length;

                for (int i = 0; i < profileCount; i++)
                {
                    var profile = spawningProfiles[i];

                    if (profile.SpawnCount.Value <= 0) continue;

                    // This means when nothing to spawned, UI won't update.
                    float progressValue =
                        profile.SpawnDuration.DurationCounterSeconds / profile.SpawnDuration.SpawnDurationSeconds;

                    GameplayMessenger.MessagePublisher.Publish(new SetProgressBarMessage
                    {
                        SpawnerEntity = spawnerEntity,
                        SpawningProfileElementIndex = i,
                        Value = progressValue,
                    });

                }

            }

        }
        
    }

}