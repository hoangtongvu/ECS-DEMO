using Unity.Entities;

namespace Components.Misc.Presenter
{
    public struct TransformAccessArrayIndex : IComponentData
    {
        public int Value;
        public static readonly TransformAccessArrayIndex Invalid = new()
        {
            Value = -1,
        };
    }

}
