using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Core.GameEntity
{
    public class GameEntityProfilesSO<IdType, ProfileType> : ScriptableObject
        where IdType : unmanaged
        where ProfileType : GameEntityProfileElement
    {
        [SerializedDictionary("Id", "Profile")]
        public SerializedDictionary<IdType, ProfileType> Profiles;

    }

}