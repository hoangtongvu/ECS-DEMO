using Components;
using Components.CustomIdentification;
using Unity.Entities;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UnityTransformMapSystem : SystemBase
    {
        protected override void OnCreate()
        {
            Entity entity = EntityManager.CreateEntity();

            EntityManager.AddComponent<UnityTransformMap>(entity);
            EntityManager.SetComponentData(entity, new UnityTransformMap
            {
                Value = new System.Collections.Generic.Dictionary<UniqueId, UnityEngine.Transform>(),
            });

            EntityManager.SetName(entity, "UnityTransformMap");

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }
    }
}