using Components.GameEntity;
using Core.GameEntity;
using Unity.Collections;
using Unity.Entities;
using UnityFileDebugLogger;
using Utilities;

namespace Systems.Initialization.GameEntity
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GameEntitySizeMapInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    BakedGameEntityProfileElement >()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var gameEntitySizeMap = new GameEntitySizeMap
            {
                Value = new(20, Allocator.Persistent),
            };

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
                    var keyEntity = bakedProfile.PrimaryEntity;
                    if (keyEntity == Entity.Null) continue;

                    if (!gameEntitySizeMap.Value.TryAdd(keyEntity, bakedProfile.GameEntitySize))
                    {
                        UnityEngine.Debug.LogWarning(
                            $"{nameof(GameEntitySizeMap)} already contains key: [{this.EntityManager.GetName(keyEntity)} - {keyEntity}]" +
                            $", which mean more than 2 {nameof(GameEntityProfileElement)} use the same GO as PrimaryEntityPrefab.\n" +
                            $"<b>The Data with duplicated key lay in [{this.EntityManager.GetName(bakerEntity)} - {bakerEntity}] and they will be discarded.</b>");

                    }

                    fileDebugLogger.Log($"Added [{this.EntityManager.GetName(keyEntity)} - {keyEntity}] - [{bakedProfile.GameEntitySize}]");

                }

            }

            su.AddOrSetComponentData(gameEntitySizeMap);
            fileDebugLogger.Save("GameEntitySizeMapInitSystemLogs.txt");

        }

    }

}