using TypeWrap;
using Unity.Entities;

namespace Components.Tool.Picker;

[WrapType(typeof(Entity))]
public partial struct ToolToPick : IComponentData
{
}