using Core.Spawner;
using Core.UI.Identification;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class TestUISpawnerSystem : SystemBase
    {

        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            for (int i = 0; i < 3; i++)
            {
                var spawnedPrefab = UISpawner.Instance.Spawn(
                    UIType.UnitSpawnProfileUI
                    , new float3(i * 5, 0, 0)
                    , quaternion.identity);

                spawnedPrefab.gameObject.SetActive(true);
            }

        }
    }
}
