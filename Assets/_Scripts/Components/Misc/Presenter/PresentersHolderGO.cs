using Unity.Entities;
using UnityEngine;

namespace Components.Misc.Presenter
{
    public struct PresentersHolderGO : IComponentData
    {
        public UnityObjectRef<Transform> Value;
    }

}
