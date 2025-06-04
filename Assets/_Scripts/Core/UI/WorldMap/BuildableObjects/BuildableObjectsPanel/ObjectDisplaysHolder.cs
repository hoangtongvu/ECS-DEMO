using Core.Misc;
using Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel.BuildableObjectDisplay;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel
{
    public class ObjectDisplaysHolder : SaiMonoBehaviour
    {
        [SerializeField] private List<BuildableObjectDisplayCtrl> displays;

        public List<BuildableObjectDisplayCtrl> Displays => displays;

    }

}
