using Components.Misc.Presenter.PresenterPrefabGO;
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
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var presenterPrefabGOMap = new PresenterPrefabGOMap()
            {
                Value = new(15, Allocator.Persistent),
            };

            su.AddOrSetComponentData(presenterPrefabGOMap);

        }

    }

}