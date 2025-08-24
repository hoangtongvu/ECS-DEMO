using Components.Misc.Presenter.PresenterPrefabGO;
using Core.Misc.Presenter;
using Unity.Entities;

namespace Systems.Initialization.Misc.Presenter.PresenterPrefabGO
{
    [UpdateInGroup(typeof(PresenterPrefabGOMapInitSystemGroup))]
    public partial class BasePresenterPoolMapInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<PresenterPrefabGOMap>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var presenterPrefabGOMap = SystemAPI.GetSingleton<PresenterPrefabGOMap>().Value;
            var poolMap = BasePresenterPoolMap.Instance;

            foreach (var kVPair in presenterPrefabGOMap)
            {
                var basePresenter = kVPair.Value.Value;

                poolMap.poolMap.TryAdd(basePresenter.gameObject, new()
                {
                    Prefab = basePresenter.gameObject,
                });
            }
            
        }

    }

}