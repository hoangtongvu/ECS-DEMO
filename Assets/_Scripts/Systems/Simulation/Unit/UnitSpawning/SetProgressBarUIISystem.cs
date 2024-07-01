using Unity.Entities;
using Components;
using Components.Unit.UnitSpawning;
using Components.Unit;
using Core.MyEvent.PubSub.Messengers;
using ZBase.Foundation.PubSub;
using Core.MyEvent.PubSub.Messages;

namespace Systems.Simulation.Unit.UnitSpawning
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SetProgressBarUIISystem : SystemBase
    {

        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitSelected
                    , EntitySpawningProfileElement
                    , UISpawned>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {

            foreach (var (selectedRef, spawningProfiles, uiSpawnedRef) in
                SystemAPI.Query<
                    RefRO<UnitSelected>
                    , DynamicBuffer<EntitySpawningProfileElement>
                    , RefRO<UISpawned>>())
            {
                if (!selectedRef.ValueRO.Value) continue;
                if (!uiSpawnedRef.ValueRO.IsSpawned) continue;

                foreach (var profile in spawningProfiles)
                {
                    if (profile.SpawnCount <= 0) continue;
                    
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