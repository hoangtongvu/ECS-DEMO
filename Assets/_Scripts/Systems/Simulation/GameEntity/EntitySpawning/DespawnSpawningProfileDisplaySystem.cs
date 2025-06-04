using Unity.Entities;
using Unity.Transforms;
using Components.ComponentMap;
using Utilities.Helpers;
using Core.UI.EntitySpawningPanel;
using Components.GameEntity.EntitySpawning;
using Components.Misc;
using LitMotion;
using Unity.Mathematics;

namespace Systems.Simulation.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class DespawnEntitySpawningPanelSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    WithinPlayerAutoInteractRadiusTag
                    , EntitySpawningProfileElement
                    , UISpawned
                    , LocalTransform>()
                .Build();

            this.RequireForUpdate(query);
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<UIPrefabAndPoolMap>();
        }

        protected override void OnUpdate()
        {
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();

            foreach (var (spawningProfiles, uiSpawnedRef, transformRef) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , RefRW<UISpawned>
                    , RefRO<LocalTransform>>()
                    .WithDisabled<WithinPlayerAutoInteractRadiusTag>())
            {
                bool canDespawn = uiSpawnedRef.ValueRO.IsSpawned;
                if (!canDespawn) continue;

                spawnedUIMap.Value.TryGetValue(uiSpawnedRef.ValueRO.UIID.Value, out var baseUICtrl);
                var spawningPanel = (EntitySpawningPanelCtrl)baseUICtrl;

                this.DespawnEntitySpawningPanel(ref uiSpawnedRef.ValueRW);
                this.SetTween(uiPrefabAndPoolMap, spawnedUIMap, spawningPanel, transformRef.ValueRO.Position);

                uiSpawnedRef.ValueRW.IsSpawned = false;

            }

        }

        private void DespawnEntitySpawningPanel(ref UISpawned uiSpawned) => uiSpawned.UIID = null;

        private void SetTween(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , EntitySpawningPanelCtrl entitySpawningPanelCtrl
            , float3 toPos)
        {
            var spawningProfileDisplayCtrls = entitySpawningPanelCtrl.SpawningDisplaysHolder.SpawningProfileDisplayCtrls;
            int profileCount = spawningProfileDisplayCtrls.Count;

            LMotion.Create(new float3(1, 1, 1), float3.zero, 1f)
                .WithEase(Ease.OutExpo)
                .WithOnComplete(() =>
                {
                    UISpawningHelper.Despawn(uiPrefabAndPoolMap, spawnedUIMap, entitySpawningPanelCtrl.RuntimeUIID);
                })
                .Bind(tempScale => entitySpawningPanelCtrl.transform.localScale = tempScale);

            LMotion.Create(entitySpawningPanelCtrl.transform.position, toPos, 1f)
                .WithEase(Ease.OutExpo)
                .Bind(tempPos => entitySpawningPanelCtrl.transform.position = tempPos);

            for (int i = 0; i < profileCount; i++)
            {
                var profileDisplayCtrl = spawningProfileDisplayCtrls[i];

                LMotion.Create(new float3(1, 1, 1), float3.zero, 1f)
                    .WithEase(Ease.OutExpo)
                    .WithOnComplete(() =>
                    {
                        UISpawningHelper.Despawn(uiPrefabAndPoolMap, spawnedUIMap, profileDisplayCtrl.RuntimeUIID);
                    })
                    .Bind(tempScale => profileDisplayCtrl.transform.localScale = tempScale);

            }

            spawningProfileDisplayCtrls.Clear();

        }

    }

}