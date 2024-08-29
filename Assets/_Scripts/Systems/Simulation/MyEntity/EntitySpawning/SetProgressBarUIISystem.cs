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
    public partial class SetProgressBarUIISystem : SystemBase
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

            foreach (var (spawningProfiles, uiSpawnedRef) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , RefRO<UISpawned>>()
                    .WithAll<UnitSelectedTag>())
            {
                if (!uiSpawnedRef.ValueRO.IsSpawned) continue;

                foreach (var profile in spawningProfiles)
                {
                    if (profile.SpawnCount.Value <= 0) continue;
                    
                    // This means when nothing to spawned, UI won't update.
                    float progressValue =
                        profile.SpawnDuration.DurationCounterSecond / profile.SpawnDuration.DurationPerSpawn;

                    GameplayMessenger.MessagePublisher.Publish(new SetProgressBarMessage
                    {
                        UIID = profile.UIID.Value,
                        Value = progressValue,
                    });
                }
            }

        }

        
        

    }
}