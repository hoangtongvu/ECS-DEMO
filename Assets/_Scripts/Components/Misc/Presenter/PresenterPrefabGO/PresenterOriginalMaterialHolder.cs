using Unity.Entities;

namespace Components.Misc.Presenter.PresenterPrefabGO
{
    public struct PresenterOriginalMaterialHolder : ISharedComponentData
    {
        public UnityObjectRef<UnityEngine.Material> Value;
    }

}
