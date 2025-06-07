using Components.GameEntity;
using Components.Misc.Presenter.PresenterPrefabGO;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Misc.Presenter.PresenterPrefabGO
{
    [UpdateInGroup(typeof(PresenterPrefabGOMapInitSystemGroup))]
    public partial class OriginalPresenterGOMapInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<BakedGameEntityProfileElement>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var su = SingletonUtilities.GetInstance(this.EntityManager);
            var map = new OriginalPresenterGOMap
            {
                Value = new(15, Allocator.Persistent),
            };

            foreach (var bakedProfiles in
                SystemAPI.Query<
                    DynamicBuffer<BakedGameEntityProfileElement>>())
            {
                int count = bakedProfiles.Length;

                for (int i = 0; i < count; i++)
                {
                    var targetEntity = bakedProfiles[i].PrimaryEntity;
                    var presenterEntity = bakedProfiles[i].PresenterEntity;
                    var originalPresenterGO = bakedProfiles[i].OriginalPresenterGO.Value;

                    if (targetEntity == Entity.Null) continue;
                    if (presenterEntity == Entity.Null) continue;

                    map.Value.Add(targetEntity, originalPresenterGO);

                }

            }

            su.AddOrSetComponentData(map);

        }

    }

}