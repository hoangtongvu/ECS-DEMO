using UnityEngine;
using UnityEngine.Jobs;

namespace Core.Misc.Presenter
{
    public class PresentersTransformAccessArrayGO : MonoBehaviour
    {
        public TransformAccessArray TransformAccessArray;

        private void Awake()
        {
            this.TransformAccessArray = new(100);
        }

        private void OnDestroy()
        {
            this.TransformAccessArray.Dispose();
        }

    }

}