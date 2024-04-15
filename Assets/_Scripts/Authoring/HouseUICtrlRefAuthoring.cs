using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class HouseUICtrlRefAuthoring : MonoBehaviour
    {

        private class Baker : Baker<HouseUICtrlRefAuthoring>
        {
            public override void Bake(HouseUICtrlRefAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<HouseUICtrlRef>(entity);

            }
        }
    }
}
