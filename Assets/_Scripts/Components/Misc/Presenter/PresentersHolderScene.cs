using Unity.Entities;
using UnityEngine.SceneManagement;

namespace Components.Misc.Presenter
{
    public struct PresentersHolderScene : IComponentData
    {
        public Scene Value;
    }

}
