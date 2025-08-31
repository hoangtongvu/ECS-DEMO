using Core.Misc;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel.BuildableObjectDisplay.Previews.CostStack
{
    public partial class CostStacksHolder : SaiMonoBehaviour
    {
        [SerializeField] private List<CostStackCtrl> value = new();

        public List<CostStackCtrl> Value => value;

    }

}
