using Components.GameEntity.EntitySpawning;
using Components.GameEntity.InteractableActions;
using Components.GameEntity.Misc;
using Components.Player;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Simulation.GameEntity.EntitySpawning.InteractableActions
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SetSpawnCountTextSystem : SystemBase
    {
        private EntityQuery playerQuery;

        protected override void OnCreate()
        {
            this.playerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , FactionIndex>()
                .Build();

            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    EntitySpawningProfileElement
                    , ActionsContainerUIShownTag>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            var playerFactionIndex = this.playerQuery.GetSingleton<FactionIndex>();

            foreach (var (factionIndexRef, spawningProfiles, spawnerEntity) in SystemAPI
                .Query<
                    RefRO<FactionIndex>
                    , DynamicBuffer<EntitySpawningProfileElement>>()
                .WithAll<ActionsContainerUIShownTag>()
                .WithEntityAccess())
            {
                if (factionIndexRef.ValueRO.Value != playerFactionIndex.Value) continue;

                int profileCount = spawningProfiles.Length;

                for (int i = 0; i < profileCount; i++)
                {
                    var profile = spawningProfiles[i];
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