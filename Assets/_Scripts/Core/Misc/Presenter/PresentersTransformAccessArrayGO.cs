using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

namespace Core.Misc.Presenter
{
    public class PresentersTransformAccessArrayGO : MonoBehaviour
    {
        private readonly Queue<int> indexPool = new();
        public TransformAccessArray TransformAccessArray;

        private void Awake()
        {
            this.TransformAccessArray = new(100);
        }

        private void OnDestroy()
        {
            this.TransformAccessArray.Dispose();
        }

        public void AddTransform(Transform transform, out int index)
        {
            if (this.indexPool.Count != 0)
            {
                index = this.indexPool.Dequeue();
                this.TransformAccessArray[index] = transform;
                return;
            }

            index = this.TransformAccessArray.length;
            this.TransformAccessArray.Add(transform);
        }

        public void RemoveTransformAt(int index)
        {
            this.TransformAccessArray[index] = null;
            this.indexPool.Enqueue(index);
        }

    }

}