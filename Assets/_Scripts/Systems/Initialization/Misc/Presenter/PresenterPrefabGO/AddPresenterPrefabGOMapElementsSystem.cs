using Components.GameEntity;
using Components.Misc.Presenter.PresenterPrefabGO;
using Core.Misc.Presenter;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.Misc.Presenter.PresenterPrefabGO
{
    [UpdateInGroup(typeof(PresenterPrefabGOMapInitSystemGroup))]
    public partial class AddPresenterPrefabGOMapElementsSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<BakedGameEntityProfileElement>();
            this.RequireForUpdate<PresenterPrefabGOMap>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var presenterPrefabGOMap = SystemAPI.GetSingleton<PresenterPrefabGOMap>();

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var bakedProfiles in
                SystemAPI.Query<
                    DynamicBuffer<BakedGameEntityProfileElement>>())
            {
                int count = bakedProfiles.Length;

                for (int i = 0; i < count; i++)
                {
                    var targetEntity = bakedProfiles[i].PrimaryEntity;
                    var presenterEntity = bakedProfiles[i].PresenterEntity;
                    var presenterPrefabGO = bakedProfiles[i].OriginalPresenterGO.Value;

                    if (targetEntity == Entity.Null) continue;
                    if (presenterEntity != Entity.Null) continue;

                    if (presenterPrefabGO == null) continue;
                    if (!presenterPrefabGO.TryGetComponent<BasePresenter>(out var basePresenter))
                    {
                        UnityEngine.Debug.LogError($"{presenterPrefabGO.name} does not contain {nameof(BasePresenter)} component");
                        continue;
                    }

                    ecb.AddComponent<HasPresenterPrefabGOTag>(targetEntity);

                    presenterPrefabGOMap.Value.Add(targetEntity, basePresenter);

                }

            }

            ecb.Playback(this.EntityManager);

        }

    }

}