using Core.Animator;
using Core.Misc.Presenter;
using Core.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Core.ToolAndBuilding.ToolSpawnerBuilding.Presenter
{
    public class ToolSpawnerPresenter : BasePresenter
    {
        [SerializeField] private List<ToolHolderSlotMarker> toolHolderSlotMarkers;
        [SerializeField] private BaseAnimator benchWorkerAnimator;
        private float originalbenchWorkerEulerY;

        public List<ToolHolderSlotMarker> ToolHolderSlotMarkers => toolHolderSlotMarkers;
        public BaseAnimator BenchWorkerAnimator => benchWorkerAnimator;

        protected override void Awake()
        {
            this.originalbenchWorkerEulerY = this.benchWorkerAnimator.transform.rotation.eulerAngles.y;
        }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadToolHolderSlotMarkers();
            this.LoadBenchWorkerAnimator();
        }

        protected override void LoadMeshRenderer(out MeshRenderer meshRenderer)
        {
            meshRenderer = transform.Find("SM_WorkBench").GetComponent<MeshRenderer>();
        }

        private void LoadToolHolderSlotMarkers()
        {
            this.toolHolderSlotMarkers = GetComponentsInChildren<ToolHolderSlotMarker>().ToList();
        }

        private void LoadBenchWorkerAnimator()
        {
            this.benchWorkerAnimator = transform.Find("BenchWorker").GetComponent<BaseAnimator>();
        }

        public void Work()
        {
            this.RotateBenchWorker(true);
            this.PlayWorkAnim();
        }

        public void EndWorking()
        {
            this.PlayIdleAnim();
            this.RotateBenchWorker(false);
        }

        private void RotateBenchWorker(bool isWorking)
        {
            float3 tempEulerAngles = this.benchWorkerAnimator.transform.rotation.eulerAngles;
            float targetEulerY = isWorking
                ? 180 - this.originalbenchWorkerEulerY
                : this.originalbenchWorkerEulerY;

            this.benchWorkerAnimator.transform.rotation = Quaternion.Euler(tempEulerAngles.With(y: targetEulerY));
        }

        private void PlayWorkAnim()
        {
            this.benchWorkerAnimator.WaitPlay("UnarmedWorkingAnimation", 0.2f);
        }

        void PlayIdleAnim()
        {
            this.benchWorkerAnimator.WaitPlay("Sit_Floor_Idle", 0.2f);
        }

    }

}