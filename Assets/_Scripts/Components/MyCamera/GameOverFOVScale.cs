using TypeWrap;
using Unity.Entities;

namespace Components.MyCamera;

[WrapType(typeof(float))]
public partial struct GameOverFOVScale : IComponentData
{
}