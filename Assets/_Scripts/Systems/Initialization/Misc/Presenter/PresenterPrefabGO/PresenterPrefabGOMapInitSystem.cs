using Components.GameEntity;
using Components.Misc.Presenter.PresenterPrefabGO;
using Core.Misc.Presenter;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Misc.Presenter.PresenterPrefabGO
{
    [UpdateInGroup(typeof(PresenterPrefabGOMapInitSystemGroup), OrderFirst = true)]
    public partial class PresenterPrefabGOMapInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<BakedGameEntityProfileElement>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var su = SingletonUtilities.GetInstance(this.EntityManager);
            var presenterPrefabGOMap = new PresenterPrefabGOMap()
            {
                Value = new(15, Allocator.Persistent),
            };

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
                        UnityEngine.Debug.LogError($"{presenterPrefabGO.name} does not contain {nameof(BasePresenter)} component", presenterPrefabGO);
                        continue;
                    }

                    ecb.AddComponent<HasPresenterPrefabGOTag>(targetEntity);

                    presenterPrefabGOMap.Value.Add(targetEntity, basePresenter);

                }

            }

            ecb.Playback(this.EntityManager);
            su.AddOrSetComponentData(presenterPrefabGOMap);

        }

    }

}