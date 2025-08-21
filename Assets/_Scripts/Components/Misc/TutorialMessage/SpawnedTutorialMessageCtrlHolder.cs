using Core.UI.TutorialMessage;
using Unity.Entities;

namespace Components.Misc.TutorialMessage
{
    public struct SpawnedTutorialMessageCtrlHolder : IComponentData
    {
        public UnityObjectRef<TutorialMessageCtrl> Value;
    }
}
