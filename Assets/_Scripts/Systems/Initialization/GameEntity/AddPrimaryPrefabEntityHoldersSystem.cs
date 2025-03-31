using Components.GameEntity;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.GameEntity
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial class AddPrimaryPrefabEntityHoldersSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<AfterBakedPrefabsElement>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var afterBakedPrefabsElements in
                SystemAPI.Query<
                    DynamicBuffer<AfterBakedPrefabsElement>>())
            {
                int count = afterBakedPrefabsElements.Length;

                for (int i = 0; i < count; i++)
                {
                    var primaryEntity = afterBakedPrefabsElements[i].PrimaryEntity;

                    if (primaryEntity == Entity.Null) continue;

                    ecb.AddComponent(primaryEntity, new PrimaryPrefabEntityHolder(primaryEntity));

                }

            }

            ecb.Playback(this.EntityManager);
            ecb.Dispose();

        }

    }

}