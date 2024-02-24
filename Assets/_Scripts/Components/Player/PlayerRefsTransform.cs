using Unity.Entities;
using Unity.Transforms;

namespace Components.Player
{
    //This component is used for storing LocalTransform from all the players(currently only one player exists.)
    public struct PlayerRefsTransform : IComponentData 
    {
        public LocalTransform transform;
    }

}
