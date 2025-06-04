using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Physics;
using Components.Player;
using Unity.Mathematics;
using Components.Unit.NearUnitDropItems;
using Utilities;
using Core.Misc;

namespace Systems.Simulation.Unit.NearUnitDropItems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct TimerListUpdateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new NearbyUnitDistanceHitList
                {
                    Value = new(10, Allocator.Persistent),
                });

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new NearbyUnitHitRadius
                {
                    Value = 5f,
                });

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , NearbyUnitDropItemTimerElement
                    , PlayerTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<NearbyUnitDistanceHitList>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var hitList = SystemAPI.GetSingleton<NearbyUnitDistanceHitList>().Value;
            var hitRadius = SystemAPI.GetSingleton<NearbyUnitHitRadius>().Value;

            foreach (var (transformRef, nearbyUnitDropItemTimerList) in
                SystemAPI.Query<
                    RefRO<LocalTransform>
                    , DynamicBuffer<NearbyUnitDropItemTimerElement>>()
                    .WithAll<PlayerTag>())
            {
                bool hasHit = this.OverlapSphere(
                    in physicsWorld
                    , transformRef.ValueRO.Position
                    , hitRadius
                    , ref hitList);

                if (!hasHit)
                {
                    nearbyUnitDropItemTimerList.Clear();
                    continue;
                }

                int oldListLength = nearbyUnitDropItemTimerList.Length;
                int hitListLength = hitList.Length;

                this.RemoveUnusedElements(
                    in nearbyUnitDropItemTimerList
                    , ref oldListLength
                    , in hitList
                    , ref hitListLength);

                this.AddRemainingToOldList(in nearbyUnitDropItemTimerList, in hitList, hitListLength);

            }

        }

        [BurstCompile]
        private bool OverlapSphere(
            in PhysicsWorldSingleton physicsWorld
            , float3 centerPos
            , float radius
            , ref NativeList<DistanceHit> hitList)
        {
            return physicsWorld.OverlapSphere(
                centerPos
                , radius
                , ref hitList
                , new CollisionFilter
                {
                    BelongsTo = (uint)CollisionLayer.Player,
                    CollidesWith = (uint)CollisionLayer.Unit,
                });

        }

        [BurstCompile]
        private void RemoveUnusedElements(
            in DynamicBuffer<NearbyUnitDropItemTimerElement> oldList
            , ref int oldListLength
            , in NativeList<DistanceHit> hitList
            , ref int hitListLength)
        {
            for (int i = 0; i < oldListLength; i++)
            {
                var oldId = oldList[i].UnitEntity;

                bool foundIdInNewList = false;

                for (int j = 0; j < hitListLength; j++)
                {
                    var newId = hitList[j].Entity;

                    if (oldId != newId) continue;

                    hitList.RemoveAtSwapBack(j);
                    j--;
                    hitListLength--;

                    foundIdInNewList = true;
                    break;

                }

                if (foundIdInNewList) continue;

                oldList.RemoveAtSwapBack(i);
                i--;
                oldListLength--;

            }

        }

        [BurstCompile]
        private void AddRemainingToOldList(
            in DynamicBuffer<NearbyUnitDropItemTimerElement> oldList
            , in NativeList<DistanceHit> hitList
            , int hitListLength)
        {
            for (int i = 0; i < hitListLength; i++)
            {
                oldList.Add(new()
                {
                    UnitEntity = hitList[i].Entity,
                    CounterSecond = 0,
                });
            }

        }

    }

}