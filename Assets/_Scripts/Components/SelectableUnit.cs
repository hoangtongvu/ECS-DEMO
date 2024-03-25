using Unity.Entities;

namespace Components
{

    public struct SelectableUnitTag : IComponentData
    {
    }

    public struct SelectedState : IComponentData
    {
        public bool Value;
    }


}
