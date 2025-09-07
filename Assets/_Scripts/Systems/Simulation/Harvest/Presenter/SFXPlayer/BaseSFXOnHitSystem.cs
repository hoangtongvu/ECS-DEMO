using Audio.JSAM;
using Components.GameEntity.Damage;
using Components.Harvest;
using Core.Harvest;
using JSAM;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Simulation.Harvest.Presenter.SFXPlayer
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public abstract partial class BaseSFXOnHitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    HarvesteeTag
                    , HarvesteeProfileIdHolder
                    , LocalTransform>()
                .WithAll<
                    TakeHitEvent>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            var harvesteeType = this.GetHarvesteeType();
            var soundType = this.GetSoundType();

            foreach (var (harvesteeProfileIdHolderRef, transformRef) in SystemAPI
                .Query<
                    RefRO<HarvesteeProfileIdHolder>
                    , RefRO<LocalTransform>>()
                .WithAll<
                    HarvesteeTag>()
                .WithAll<
                    TakeHitEvent>())
            {
                if (harvesteeProfileIdHolderRef.ValueRO.Value.HarvesteeType != harvesteeType) continue;
                AudioManager.PlaySound(soundType, transformRef.ValueRO.Position);
            }

        }

        protected abstract HarvesteeType GetHarvesteeType();

        protected abstract HarvesteeSound_LibrarySounds GetSoundType();

    }

}