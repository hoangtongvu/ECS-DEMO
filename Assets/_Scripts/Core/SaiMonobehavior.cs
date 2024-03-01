using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SaiMonoBehaviour : MonoBehaviour
    {

#if (UNITY_EDITOR)
        [ContextMenu("Load Components")]
        private void LoadComponentButton()
        {
            this.LoadComponents();
            Debug.Log("Loaded Components");
        }


#endif


        protected virtual void Awake()
        {

        }

        protected virtual void Reset()
        {
            this.LoadComponents();
            this.ResetValue();
        }

        /// <summary>
        /// For override
        /// </summary>
        protected virtual void LoadComponents()
        {
        }


        protected virtual void LoadComponentInChildren<TClass>(ref TClass @class) where TClass : UnityEngine.Object
        {
            @class = GetComponentInChildren<TClass>();
            if (@class != null) return;
            Debug.LogError("Can't find " + @class, this.gameObject);
        }

        protected virtual void LoadComponentInCtrl<TClass>(ref TClass @class) where TClass : UnityEngine.Object
        {
            @class = GetComponent<TClass>();
            if (@class != null) return;
            Debug.LogError("Can't find component in ", this.gameObject);
        }

        protected virtual void LoadCtrl<TClass>(ref TClass @class) where TClass : UnityEngine.Object
        {
            @class = GetComponentInParent<TClass>();
            if (@class != null) return;
            Debug.LogError("Can't find component in ", this.gameObject);
        }

        protected virtual void LoadTransformInChildrenByName(out Transform t, string name)
        {
            t = transform.Find(name);
            if (t != null) return;
            Debug.LogError("Can't find child Transform with name " + name, this.gameObject);
        }




        /// <summary>
        /// Use to Reset Value (Reset button)
        /// </summary>
        protected virtual void ResetValue()
        {
            //For override
        }


    }
}


