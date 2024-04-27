using Core;
using Unity.Entities;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial class DefaultSingletonEntityInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.Enabled = false;
            SingletonUtilities.Setup(EntityManager);
        }

        protected override void OnUpdate()
        {
        }
    }
}