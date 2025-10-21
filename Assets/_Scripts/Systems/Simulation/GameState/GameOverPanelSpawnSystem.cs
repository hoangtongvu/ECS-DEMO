using Components.GameState;
using Components.Unit.DarkUnit;
using Core.UI.GameOverPanel;
using Core.UI.Identification;
using Core.UI.Pooling;
using Unity.Entities;
using Utilities;

namespace Systems.Simulation.GameState
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class GameOverPanelSpawnSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<GameOverEvent>();
            this.RequireForUpdate<DarkUnitSpawnCycleCounter>();
        }

        protected override void OnUpdate()
        {
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            if (!su.IsComponentEnabled<GameOverEvent>()) return;

            var waveCount = SystemAPI.GetSingleton<DarkUnitSpawnCycleCounter>().Value;

            var panel = (GameOverPanelCtrl)UICtrlPoolMap.Instance.Rent(UIType.GameOverPanel);

            panel.TotalSurvivedTimeElapsedText.SetTimeElapsed(SystemAPI.Time.ElapsedTime);
            panel.TotalWaveSurvivedText.SetWaveCount(waveCount);

            panel.gameObject.SetActive(true);
        }

    }

}