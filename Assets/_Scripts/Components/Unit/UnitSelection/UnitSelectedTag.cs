using Unity.Entities;

namespace Components.Unit.UnitSelection
{
    public struct UnitSelectedTag : IComponentData, IEnableableComponent
    {
    }

    public struct NewlySelectedUnitTag : IComponentData, IEnableableComponent
    {
    }

    public struct NewlyDeselectedUnitTag : IComponentData, IEnableableComponent
    {
    }

}
