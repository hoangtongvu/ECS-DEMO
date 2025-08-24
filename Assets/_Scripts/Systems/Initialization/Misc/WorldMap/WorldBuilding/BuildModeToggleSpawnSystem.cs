using Unity.Entities;
using Core.UI.Identification;
using Core.UI.Pooling;
using Components.UI.Pooling;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BuildModeToggleSpawnSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<UIPoolMapInitializedTag>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            UICtrlPoolMap.Instance.Rent(UIType.BuildModeTrigger);
        }

    }

}