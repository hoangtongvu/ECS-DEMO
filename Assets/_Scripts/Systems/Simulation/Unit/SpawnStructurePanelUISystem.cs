using Unity.Entities;
using Components;
using Components.Unit.UnitSpawning;
using Components.Unit;
using Core.UI.HouseUI;
using Unity.Transforms;
using Unity.Mathematics;
using Core.UI.UnitProfile;
using Core.UI.Identification;
using Core.Spawner;

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

            foreach (var (selectedRef, spawningProfiles, uiSpawnedRef, transformRef) in
                SystemAPI.Query<
                    RefRO<UnitSelected>
                    , DynamicBuffer<UnitSpawningProfileElement>
                    , RefRW<UISpawned>
                    , RefRO<LocalTransform>>())
            {
                if (selectedRef.ValueRO.Value
                    && !uiSpawnedRef.ValueRO.IsSpawned)
                {
                    // Spawn.

                    float3 spawnPos = transformRef.ValueRO.Position + uiSpawnedRef.ValueRO.SpawnPosOffset;

                    // Spawn Main UI.
                    var houseUICtrl = (HouseUICtrl) UISpawner.Instance.Spawn(
                        UIType.MainStructurePanel
                        , spawnPos
                        , quaternion.identity);

                    houseUICtrl.gameObject.SetActive(true);

                    // Spawn UnitProfileUI.
                    for (int i = 0; i < spawningProfiles.Length; i++)
                    {
                        ref var profile = ref spawningProfiles.ElementAt(i);

                        // Grid layout won't config Z dimension, that why setting unitProfileUICtrl position is required.
                        var unitProfileUICtrl =
                            (UnitProfileUICtrl)UISpawner.Instance.Spawn(
                                UIType.UnitSpawnProfileUI
                                , spawnPos
                                , quaternion.identity);

                        profile.UIID = unitProfileUICtrl.UIID;

                        unitProfileUICtrl.ProfilePic.sprite = profile.UnitSprite.Value;
                        houseUICtrl.UnitProfileHolder.Add(unitProfileUICtrl);

                        unitProfileUICtrl.gameObject.SetActive(true);
                    }


                    uiSpawnedRef.ValueRW.IsSpawned = true;
                }

                if (!selectedRef.ValueRO.Value
                    && uiSpawnedRef.ValueRO.IsSpawned)
                {
                    // Despawn

                }
            }

        }


        
    }
}