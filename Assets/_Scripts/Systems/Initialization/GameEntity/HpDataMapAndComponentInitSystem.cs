using Components.GameEntity;
using Components.GameEntity.Damage;
using Core.GameEntity;
using Unity.Collections;
using Unity.Entities;
using UnityFileDebugLogger;
using Utilities;

namespace Systems.Initialization.GameEntity
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class HpDataMapAndComponentInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    BakedGameEntityProfileElement>()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var map = new HpDataMap
            {
                Value = new(20, Allocator.Persistent),
            };

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var fileDebugLogger = FileDebugLogger.CreateLogger128Bytes(10, Allocator.Temp);

            foreach (var (bakedProfiles, bakerEntity) in
                SystemAPI.Query<
                    DynamicBuffer<BakedGameEntityProfileElement>>()
                    .WithEntityAccess())
            {
                int count = bakedProfiles.Length;

                for (int i = 0; i < count; i++)
                {
                    var bakedProfile = bakedProfiles[i];
                    var primaryEntity = bakedProfile.PrimaryEntity;
                    if (primaryEntity == Entity.Null) continue;

                    if (!bakedProfile.HasHpComponents) continue;

                    if (!map.Value.TryAdd(primaryEntity, bakedProfile.HpData))
                    {
                        UnityEngine.Debug.LogWarning(
                            $"{nameof(HpDataMap)} already contains key: [{this.EntityManager.GetName(primaryEntity)} - {primaryEntity}]" +
                            $", which mean more than 2 {nameof(GameEntityProfileElement)} use the same GO as PrimaryEntityPrefab.\n" +
                            $"<b>The Data with duplicated key lay in [{this.EntityManager.GetName(bakerEntity)} - {bakerEntity}] and they will be discarded.</b>");

                    }

                    ecb.AddComponent(primaryEntity, new CurrentHp
                    {
                        Value = bakedProfile.HpData.MaxHp,
                    });

                    ecb.AddSharedComponent(primaryEntity, new HpDataHolder
                    {
                        Value = bakedProfile.HpData,
                    });

                    ecb.AddBuffer<HpChangeRecordElement>(primaryEntity);
                    ecb.AddComponent<FrameHpChange>(primaryEntity);
                    ecb.AddComponent<IsAlive>(primaryEntity);

                    fileDebugLogger.Log($"Added [{this.EntityManager.GetName(primaryEntity)} - {primaryEntity}] - [{bakedProfile.HpData}]");

                }

            }

            su.AddOrSetComponentData(map);
            ecb.Playback(this.EntityManager);
            fileDebugLogger.Save("HpDataMapAndComponentInitSystemLogs.txt");
        }

    }

}