using Components;
using Components.Tag;
using Unity.Entities;
using UnityEngine;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class TestSystem : SystemBase
    {
        private int index = 0;
        private int amountPerTime = 1000;
        private DynamicBuffer<EntityRefElement> entities;

        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingletonBuffer(out this.entities)) return;
            if (this.index >= 10) return;
            if (Input.GetKeyDown(KeyCode.Return))
            {
                //Disable Tag for {amountPerTime} entities.
                UnityEngine.Debug.Log($"Disabled {(this.index + 1) * this.amountPerTime} Entities.");
                for (int i = this.index; i < (this.index + 1) * this.amountPerTime; i++)
                {
                    Entity e = entities[i].entity;
                    SystemAPI.SetComponentEnabled<EnableableTag>(e, false);
                }
                this.index++;
            }

        }
    }
}
