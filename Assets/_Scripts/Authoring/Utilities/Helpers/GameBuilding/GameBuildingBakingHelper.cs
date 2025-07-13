using Authoring.Utilities.Extensions;
using Authoring.Utilities.Helpers.GameEntity.InteractableActions;
using Components.GameBuilding;
using Components.GameEntity.Damage;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Misc;
using Components.Misc;
using Components.Misc.Presenter;
using Components.Misc.Presenter.PresenterPrefabGO;
using Unity.Entities;

namespace Authoring.Utilities.Helpers.GameBuilding
{
    public static class GameBuildingBakingHelper
    {
        public static void AddComponents(IBaker baker, in Entity entity)
        {
            baker.AddComponent<GameBuildingTag>(entity);
            baker.AddComponent<NewlySpawnedTag>(entity);
            baker.AddAndDisableComponent<NewlyDeadTag>(entity);
            baker.AddComponent<NeedSpawnPresenterTag>(entity);
            baker.AddAndDisableComponent<WithinPlayerAutoInteractRadiusTag>(entity);

            baker.AddComponent(entity, FactionIndex.Neutral);
            baker.AddComponent<SpawnerEntityHolder>(entity);

            baker.AddSharedComponent<PresenterOriginalMaterialHolder>(entity, default);
            InteractableActionsBakingHelper.AddComponents(baker, entity);

        }

    }

}
