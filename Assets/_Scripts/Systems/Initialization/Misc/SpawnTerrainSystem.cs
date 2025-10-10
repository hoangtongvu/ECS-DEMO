using Components.Misc.TerrainBaking;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems.Initialization.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SpawnTerrainSystem : SystemBase
    {
        private EntityQuery query0;

        protected override void OnCreate()
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    TerrainPresenterPrefabHolder
                    , TerrainPosition>()
                .Build();

            this.RequireForUpdate(this.query0);
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var prefab = this.query0.GetSingleton<TerrainPresenterPrefabHolder>().Value.Value;
            float3 spawnPos = this.query0.GetSingleton<TerrainPosition>();

            var terrainPresenter = Object.Instantiate(prefab);
            terrainPresenter.transform.position = spawnPos;
        }

    }

}