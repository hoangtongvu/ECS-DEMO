using Core.Misc.GameView;
using Unity.Entities;

namespace Components.Misc.GameView
{
    public struct CurrentGameView : IComponentData
    {
        public GameViewType Value;
    }

}
