using Core.Misc.Presenter;
using UnityEngine;

namespace Core.Unit.Presenter
{
    public class UnitPresenter : BasePresenter
    {
        [SerializeField] private RendererFlasherOnTakeHit flasher;

        [SerializeField] public RendererFlasherOnTakeHit Flasher => flasher;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(ref this.flasher);
        }

    }

}