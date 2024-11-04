using Unity.Entities;

namespace Components.Misc.FlowField
{
    public struct GridNodePresenterConfig : IComponentData
    {
        public bool ShowCost;
        public bool ShowBestCost;
        public bool ShowDirectionVector;
        public bool ShowGridLines;
    }

}
