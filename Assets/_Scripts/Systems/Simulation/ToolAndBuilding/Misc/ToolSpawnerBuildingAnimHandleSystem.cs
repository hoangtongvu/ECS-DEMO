using Unity.Entities;
using Components.Misc.Presenter;
using Core.ToolAndBuilding.ToolSpawnerBuilding.Presenter;
using Components.GameEntity.EntitySpawning.SpawningProcess;

namespace Systems.Simulation.ToolAndBuilding.Misc
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class ToolSpawnerBuildingAnimHandleSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder
                    , JustBeginSpawningProcessTag
                    , JustEndSpawningProcessTag>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            foreach (var presenterHolderRef in SystemAPI
                .Query<
                    RefRO<PresenterHolder>>()
                .WithAll<
                    JustBeginSpawningProcessTag>())
            {
                if (presenterHolderRef.ValueRO.Value.Value is not ToolSpawnerPresenter toolSpawnerPresenter) continue;
                toolSpawnerPresenter.Work();
            }

            foreach (var presenterHolderRef in SystemAPI
                .Query<
                    RefRO<PresenterHolder>>()
                .WithAll<
                    JustEndSpawningProcessTag>())
            {
                if (presenterHolderRef.ValueRO.Value.Value is not ToolSpawnerPresenter toolSpawnerPresenter) continue;
                toolSpawnerPresenter.EndWorking();
            }

        }

    }

}