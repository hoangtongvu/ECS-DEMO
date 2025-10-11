using Core.Misc.Presenter.TransformSync;
using TypeWrap;
using Unity.Entities;

namespace Components.Misc.Presenter.TransformSync;

[WrapType(typeof(TransformSyncType))]
public partial struct TransformSyncTypeHolder : IComponentData
{
}