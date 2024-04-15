using Unity.Entities;
using Components;
using Components.Unit.UnitSpawning;
using Components.Unit;
using Core.UI.HouseUI;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Core.UI.UnitProfile;
using ZBase.Foundation.PubSub;
using Core.MyEvent;
using Core.MyEvent.PubSub.Messengers;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SpawnHouseUISystem : SystemBase
    {

        public struct UIChangedMessage : IMessage
        {
            
        }

        protected override void OnCreate()
        {
            this.RequireForUpdate<UnitSpawningProfileElement>();
            this.RequireForUpdate<HouseUICtrlRef>();
            this.RequireForUpdate<UnitSelected>();
        }

        protected override void OnUpdate()
        {

            foreach (var (selectedRef, spawningProfiles, houseUICtrlRef, uiSpawnedRef, transformRef) in
                SystemAPI.Query<
                    RefRO<UnitSelected>
                    , DynamicBuffer<UnitSpawningProfileElement>
                    , RefRW<HouseUICtrlRef>
                    , RefRW<UISpawned>
                    , RefRO<LocalTransform>>())
            {
                if (selectedRef.ValueRO.Value
                    && !uiSpawnedRef.ValueRO.IsSpawned)
                {
                    this.Spawn(
                        transformRef.ValueRO.Position
                        , uiSpawnedRef.ValueRO.SpawnPosOffset
                        , ref uiSpawnedRef.ValueRW
                        , ref houseUICtrlRef.ValueRW
                        , spawningProfiles);
                    // Invoke UI changed event.
                    this.InvokeUIChangedEvent();
                }

                if (!selectedRef.ValueRO.Value
                    && houseUICtrlRef.ValueRO.Value.IsValid()
                    && uiSpawnedRef.ValueRO.IsSpawned)
                {
                    // Despawn
                    Debug.Log("Despawned");
                    uiSpawnedRef.ValueRW.IsSpawned = false;

                    HouseUICtrl houseUICtrl = houseUICtrlRef.ValueRW.Value;
                    houseUICtrl.UnitProfileHolder.DespawnAll();
                    houseUICtrl.Despawner.DespawnObject();

                    houseUICtrlRef.ValueRW.Value.Value = null;
                    // Invoke UI changed event.
                    this.InvokeUIChangedEvent();
                }
            }

        }

        private void Spawn(
            in float3 spawnerPos
            , in float3 spawnPosOffset
            , ref UISpawned uISpawned
            , ref HouseUICtrlRef worldUICtrlRef
            , DynamicBuffer<UnitSpawningProfileElement> spawningProfiles)
        {
            Debug.Log("Spawned");

            // Spawn Main UI.
            HouseUICtrl houseUICtrl =
                HouseUISpawner.Instance.Spawn(
                    "HouseUI"
                    , spawnerPos + spawnPosOffset
                    , Quaternion.identity);

            houseUICtrl.gameObject.SetActive(true);

            // TODO: Consider merge these 2 below into 1.
            uISpawned.IsSpawned = true;
            worldUICtrlRef.Value = houseUICtrl;

            // Spawn sub UI then add them into holder.
            // use For instead of foreach
            int buttonID = 0;
            foreach (var profile in spawningProfiles)
            {
                // Grid layout won't config Z dimension, that why setting unitProfileUICtrl position is required.
                UnitProfileUICtrl unitProfileUICtrl =
                    UnitProfileUISpawner.Instance.Spawn("UnitProfileUI", spawnerPos, Quaternion.identity);

                unitProfileUICtrl.ProfilePic.sprite = profile.UnitSprite.Value;
                houseUICtrl.UnitProfileHolder.Add(unitProfileUICtrl);

                // Set buttonID & MessagePublisher.
                // Make Button already contain Publisher.
                unitProfileUICtrl.UnitProfileUIButton
                    .SetButtonIDAndPublisher(buttonID, houseUICtrl.Messenger.MessagePublisher);

                unitProfileUICtrl.gameObject.SetActive(true);

                buttonID++;

            }


        }

        
        private void InvokeUIChangedEvent() => GameplayMessenger.MessagePublisher.Publish(new UIChangedMessage());


    }
}