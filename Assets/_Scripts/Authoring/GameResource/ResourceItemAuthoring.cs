using Authoring.Utilities.Extensions;
using Components.GameEntity.EntitySpawning;
using Components.GameResource;
using Components.GameResource.ItemPicking.Pickee;
using Components.GameResource.ItemPicking.Pickee.RePickUpCoolDown;
using Components.Misc.Presenter;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameResource
{
    public class ResourceItemAuthoring : MonoBehaviour
    {
        private class Baker : Baker<ResourceItemAuthoring>
        {
            public override void Bake(ResourceItemAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<ResourceItemICD>(entity);
                AddComponent<NewlySpawnedTag>(entity);
                AddComponent<NeedSpawnPresenterTag>(entity);

                AddComponent<PickableItem>(entity);
                AddComponent<PickerEntity>(entity);
                AddComponent<PickerPos>(entity);
                this.AddAndDisableComponent<NeedUpdatePickerPos>(entity);
                this.AddAndDisableComponent<IsCandidateItem>(entity);

                this.AddAndDisableComponent<IsInRePickUpCoolDown>(entity);
                AddComponent<PreviousPickerEntity>(entity);
                AddComponent<PreviousPickerPickupCoolDownSeconds>(entity);
            }
        }
    }
}
