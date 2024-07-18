using Unity.Entities;
using Components;
using Components.MyEntity.EntitySpawning;
using Core.MyEvent.PubSub.Messengers;
using ZBase.Foundation.PubSub;
using Core.MyEvent.PubSub.Messages;

namespace Systems.Simulation.MyEntity.EntitySpawning
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

            foreach (var (spawningProfiles, uiSpawnedRef) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , RefRO<UISpawned>>())
            {
                if (!uiSpawnedRef.ValueRO.IsSpawned) continue;

                foreach (var profile in spawningProfiles)
                {
                    if (!profile.SpawnCount.ValueChanged) continue;
                    this.UpdateUI(in profile);
                }
            }

        }

        private void UpdateUI(in EntitySpawningProfileElement profile)
        {
            GameplayMessenger.MessagePublisher.Publish(new SetIntTextMessage
            {
                UIID = profile.UIID.Value,
                Value = profile.SpawnCount.Value,
            });
        }
        

    }
}