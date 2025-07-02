using Core.GameEntity.Misc;
using Unity.Entities;

namespace Components.GameEntity.Misc
{
    public struct GeneralEntityDataSOHolder : IComponentData
    {
        public UnityObjectRef<GeneralEntityDataSO> Value;
    }

}
