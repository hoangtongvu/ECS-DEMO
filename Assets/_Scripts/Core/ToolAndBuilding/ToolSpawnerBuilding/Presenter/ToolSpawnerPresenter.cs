using Core.Misc.Presenter;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.ToolAndBuilding.ToolSpawnerBuilding.Presenter
{
    public class ToolSpawnerPresenter : BasePresenter
    {
        [SerializeField] private List<ToolHolderSlotMarker> toolHolderSlotMarkers;

        public List<ToolHolderSlotMarker> ToolHolderSlotMarkers => toolHolderSlotMarkers;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            LoadToolHolderSlotMarkers();
        }

        protected override void LoadMeshRenderer(out MeshRenderer meshRenderer)
        {
            meshRenderer = transform.Find("SM_WorkBench").GetComponent<MeshRenderer>();
        }

        private void LoadToolHolderSlotMarkers()
        {
            toolHolderSlotMarkers = GetComponentsInChildren<ToolHolderSlotMarker>().ToList();
        }

    }

}