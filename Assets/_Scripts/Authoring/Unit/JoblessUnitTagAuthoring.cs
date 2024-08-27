using Components.Unit;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit
{
    public class JoblessUnitTagAuthoring : MonoBehaviour
    {

        private class Baker : Baker<JoblessUnitTagAuthoring>
        {
            public override void Bake(JoblessUnitTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<JoblessUnitTag>(entity);

            }
        }
    }
}
