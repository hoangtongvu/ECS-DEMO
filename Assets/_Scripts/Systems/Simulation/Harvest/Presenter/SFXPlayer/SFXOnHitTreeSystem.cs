using Audio.JSAM;
using Core.Harvest;
using Unity.Entities;

namespace Systems.Simulation.Harvest.Presenter.SFXPlayer
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SFXOnHitTreeSystem : BaseSFXOnHitSystem
    {
        protected override HarvesteeType GetHarvesteeType() => HarvesteeType.Tree;

        protected override HarvesteeSound_LibrarySounds GetSoundType() => HarvesteeSound_LibrarySounds.OnHitTreeSound;

    }

}