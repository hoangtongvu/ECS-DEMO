using Authoring.Utilities.Extensions;
using Components.GameResource;
using Components.GameResource.ItemPicking;
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
                AddComponent<NeedSpawnPresenterTag>(entity);

                AddComponent<UnitCannotPickUpTag>(entity);
                AddComponent<UnitCannotPickUpTimeCounter>(entity);

                AddComponent<PickableItem>(entity);
                AddComponent<PickerEntity>(entity);
                AddComponent<PickerPos>(entity);
                this.AddAndDisableComponent<NeedUpdatePickerPos>(entity);
                this.AddAndDisableComponent<IsCandidateItem>(entity);
            }
        }
    }
}
