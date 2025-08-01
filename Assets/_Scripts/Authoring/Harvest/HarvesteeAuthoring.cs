using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Interaction;
using Components.Harvest;
using Components.Harvest.HarvesteeHp;
using Components.Misc.Presenter;
using Unity.Entities;
using UnityEngine;
using TweenLib.StandardTweeners.ShakePositionTweeners;
using Authoring.Utilities.Extensions;
using Components.GameEntity.Damage;

namespace Authoring.Harvest
{
    public class HarvesteeAuthoring : MonoBehaviour
    {
        private class Baker : Baker<HarvesteeAuthoring>
        {
            public override void Bake(HarvesteeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<NewlySpawnedTag>(entity);
                AddComponent<NeedSpawnPresenterTag>(entity);
                AddComponent<InteractableEntityTag>(entity);
                this.AddAndDisableComponent<NewlyTakeHitTag>(entity);

                AddComponent<HarvesteeTag>(entity);

                AddComponent<DropResourceHpThreshold>(entity);

                ShakePositionXZTweener.AddTweenComponents(this, entity);

            }

        }

    }

}
