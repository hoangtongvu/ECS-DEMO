using Components.ComponentMap;
using Components.CustomIdentification;
using Unity.Entities;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BaseAnimatorMapSystem : SystemBase
    {
        protected override void OnCreate()
        {
            Entity entity = EntityManager.CreateEntity();

            EntityManager.AddComponent<BaseAnimatorMap>(entity);
            EntityManager.SetComponentData(entity, new BaseAnimatorMap
            {
                Value = new System.Collections.Generic.Dictionary<UniqueId, Core.Animator.BaseAnimator>(),
            });

            EntityManager.SetName(entity, "BaseAnimatorMap");

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }
    }
}