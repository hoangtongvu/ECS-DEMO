using Components.GameEntity;
using Unity.Collections;
using Unity.Entities;
using UnityFileDebugLogger;

namespace Systems.Initialization.GameEntity
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial class AddPrimaryPrefabEntityHoldersSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<BakedGameEntityProfileElement>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var fileDebugLogger = FileDebugLogger.CreateLogger128Bytes(20, Allocator.Temp);
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var bakedProfiles in
                SystemAPI.Query<
                    DynamicBuffer<BakedGameEntityProfileElement>>())
            {
                int count = bakedProfiles.Length;

                for (int i = 0; i < count; i++)
                {
                    var primaryEntity = bakedProfiles[i].PrimaryEntity;

                    if (primaryEntity == Entity.Null) continue;

                    ecb.AddComponent(primaryEntity, new PrimaryPrefabEntityHolder(primaryEntity));
                    fileDebugLogger.Log($"Added {nameof(PrimaryPrefabEntityHolder)} for Entity: [{this.EntityManager.GetName(primaryEntity)} - {primaryEntity}]");
                }

            }

            ecb.Playback(this.EntityManager);
            fileDebugLogger.Save("AddPrimaryPrefabEntityHoldersSystemLogs.txt");

        }

    }

}