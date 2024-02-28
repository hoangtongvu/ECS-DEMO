using Components;
using Components.CustomIdentification;
using Unity.Entities;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UnityObjectMapSystem : SystemBase
    {
        protected override void OnCreate()
        {
            Entity entity = EntityManager.CreateEntity();

            EntityManager.AddComponent<UnityObjectMap>(entity);
            EntityManager.SetComponentData(entity, new UnityObjectMap
            {
                Value = new System.Collections.Generic.Dictionary<UniqueId, UnityEngine.Object>(),
            });

            EntityManager.SetName(entity, "UnityObjectMap");

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }
    }
}