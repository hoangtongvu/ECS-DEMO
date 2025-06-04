using UnityEngine;

namespace Core.Misc
{
    public class SaiMonoBehaviour : MonoBehaviour
    {
#if (UNITY_EDITOR)
        [ContextMenu("Load Components")]
        private void LoadComponentButton()
        {
            LoadComponents();
            Debug.Log("Loaded Components");
        }

#endif

        protected virtual void Awake()
        {
        }

        protected virtual void Reset()
        {
            LoadComponents();
            ResetValue();
        }

        /// <summary>
        /// For overriding
        /// </summary>
        protected virtual void LoadComponents()
        {
        }

        protected virtual void LoadComponentInChildren<TClass>(ref TClass @class) where TClass : Object
        {
            @class = GetComponentInChildren<TClass>();
            if (@class != null) return;
            Debug.LogError("Can't find " + @class, gameObject);
        }

        protected virtual void LoadComponentInCtrl<TClass>(ref TClass @class) where TClass : Object
        {
            @class = GetComponent<TClass>();
            if (@class != null) return;
            Debug.LogError("Can't find component in ", gameObject);
        }

        protected virtual void LoadCtrl<TClass>(ref TClass @class) where TClass : Object
        {
            @class = GetComponentInParent<TClass>();
            if (@class != null) return;
            Debug.LogError("Can't find component in ", gameObject);
        }

        protected virtual void LoadTransformInChildrenByName(out Transform t, string name)
        {
            t = transform.Find(name);
            if (t != null) return;
            Debug.LogError("Can't find child Transform with name " + name, gameObject);
        }

        /// <summary>
        /// Use to Reset Value (Reset button).
        /// For overriding
        /// </summary>
        protected virtual void ResetValue()
        {
        }

    }

}