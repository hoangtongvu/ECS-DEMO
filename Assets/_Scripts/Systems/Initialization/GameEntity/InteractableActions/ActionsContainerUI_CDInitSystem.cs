using Components.GameEntity.InteractableActions;
using Unity.Burst;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct ActionsContainerUI_CDInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var su = SingletonUtilities.GetInstance(state.EntityManager);

            su.AddComponent<ActionsContainerUI_CD.Holder>();
            su.AddComponent<ActionsContainerUI_CD.CanShow>();
            su.SetComponentEnabled<ActionsContainerUI_CD.CanShow>(false);
            su.AddComponent<ActionsContainerUI_CD.CanUpdate>();
            su.SetComponentEnabled<ActionsContainerUI_CD.CanUpdate>(false);
            su.AddComponent<ActionsContainerUI_CD.IsActive>();
            su.SetComponentEnabled<ActionsContainerUI_CD.IsActive>(false);

            su.AddComponent<NearestInteractableEntity>();
        }

    }

}
