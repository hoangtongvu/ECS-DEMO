using Components.UnitAndTool.Misc;
using Core.Tool;
using Unity.Entities;
using UnityEngine;

namespace Authoring.UnitAndTool
{
    public class NeedCreateAndAssignToolAuthoring : MonoBehaviour
    {
        public ToolProfileId ToolProfileId;

        private class Baker : Baker<NeedCreateAndAssignToolAuthoring>
        {
            public override void Bake(NeedCreateAndAssignToolAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new NeedCreateAndAssignTool
                {
                    Value = authoring.ToolProfileId,
                });
            }

        }

    }

}
