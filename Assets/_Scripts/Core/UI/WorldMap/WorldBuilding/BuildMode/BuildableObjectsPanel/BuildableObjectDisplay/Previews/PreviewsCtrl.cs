using Core.Misc;
using Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel.BuildableObjectDisplay.Previews.CostStack;
using Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel.BuildableObjectDisplay.Previews.PreviewImage;
using DSPool;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel.BuildableObjectDisplay.Previews
{
    public partial class PreviewsCtrl : SaiMonoBehaviour, IPoolElement, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private PreviewImageCtrl previewImage;
        [SerializeField] private CostStacksHolder costStacksHolder;

        public PreviewImageCtrl PreviewImage => previewImage;
        public CostStacksHolder CostStacksHolder => costStacksHolder;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(out this.previewImage);
            this.LoadComponentInChildren(out this.costStacksHolder);
        }

        public void OnRent()
        {
        }

        public void OnReturn()
        {
            foreach (var costStack in costStacksHolder.Value)
            {
                costStack.ReturnSelfToPool();
            }

            this.costStacksHolder.Value.Clear();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            foreach (var costStack in this.costStacksHolder.Value)
            {
                costStack.TriggerTweenOnExpanded();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (var costStack in this.costStacksHolder.Value)
            {
                costStack.TriggerTweenOnCollapsed();
            }
        }

    }

}
