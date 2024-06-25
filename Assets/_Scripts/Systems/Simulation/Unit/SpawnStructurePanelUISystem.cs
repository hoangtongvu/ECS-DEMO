using Unity.Entities;
using Components;
using Components.Unit.UnitSpawning;
using Components.Unit;
using Unity.Transforms;
using Unity.Mathematics;
using Core.UI.Identification;
using Core.UI.StructurePanel;
using Core.UI.StructurePanel.UnitProfile;
using Components.ComponentMap;
using Utilities.Helpers;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SpawnStructurePanelUISystem : SystemBase
    {

        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitSelected
                    , UnitSpawningProfileElement
                    , UISpawned
                    , LocalTransform>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPoolMap>();


            foreach (var (selectedRef, spawningProfiles, uiSpawnedRef, transformRef) in
                SystemAPI.Query<
                    RefRO<UnitSelected>
                    , DynamicBuffer<UnitSpawningProfileElement>
                    , RefRW<UISpawned>
                    , RefRO<LocalTransform>>())
            {
                if (this.CanSpawn(selectedRef, uiSpawnedRef))
                {

                    float3 spawnPos = transformRef.ValueRO.Position + uiSpawnedRef.ValueRO.SpawnPosOffset;

                    this.SpawnMainPanel(
                        uiPoolMap
                        , spawnedUIMap
                        , spawnPos
                        , ref uiSpawnedRef.ValueRW
                        , out var structurePanelUICtrl);

                    this.SpawnUnitProfileUI(
                        uiPoolMap
                        , spawnedUIMap
                        , spawningProfiles
                        , spawnPos
                        , structurePanelUICtrl);

                    uiSpawnedRef.ValueRW.IsSpawned = true;
                }

            }

        }

        private bool CanSpawn(
            RefRO<UnitSelected> selectedRef
            , RefRW<UISpawned> uiSpawnedRef) => selectedRef.ValueRO.Value && !uiSpawnedRef.ValueRO.IsSpawned;

        private void SpawnMainPanel(
            UIPoolMap uiPoolMap
            , SpawnedUIMap spawnedUIMap
            , float3 spawnPos
            , ref UISpawned uiSpawned
            , out StructurePanelUICtrl structurePanelUICtrl)
        {

            structurePanelUICtrl =
                (StructurePanelUICtrl) UISpawningHelper.Spawn(
                    uiPoolMap
                    , spawnedUIMap
                    , UIType.MainStructurePanel
                    , spawnPos
                    , quaternion.identity);

            uiSpawned.UIID = structurePanelUICtrl.UIID;

            structurePanelUICtrl.gameObject.SetActive(true);
        }

        private void SpawnUnitProfileUI(
            UIPoolMap uiPoolMap
            , SpawnedUIMap spawnedUIMap
            , DynamicBuffer<UnitSpawningProfileElement> spawningProfiles
            , float3 spawnPos
            , StructurePanelUICtrl structurePanelUICtrl)
        {
            for (int i = 0; i < spawningProfiles.Length; i++)
            {
                ref var profile = ref spawningProfiles.ElementAt(i);

                // Grid layout won't config Z dimension, that why setting unitProfileUICtrl position is required.
                var unitProfileUICtrl =
                    (UnitProfileUICtrl) UISpawningHelper.Spawn(
                        uiPoolMap
                        , spawnedUIMap
                        , UIType.UnitSpawnProfileUI
                        , spawnPos
                        , quaternion.identity);

                unitProfileUICtrl.ProgressBar.ClearProgress();

                profile.UIID = unitProfileUICtrl.UIID;

                unitProfileUICtrl.ProfilePic.sprite = profile.UnitSprite.Value;
                structurePanelUICtrl.UnitProfileUIHolder.Add(unitProfileUICtrl);

                unitProfileUICtrl.gameObject.SetActive(true);
            }
        }

    }
}