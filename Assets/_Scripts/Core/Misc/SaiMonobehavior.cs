using UnityEngine;

namespace Core.Misc;

public abstract class SaiMonoBehaviour : MonoBehaviour
{

#if (UNITY_EDITOR)

    [ContextMenu("Load Components")]
    private void LoadComponentButton()
    {
        this.LoadComponents();
        Debug.Log("Loaded Components");
    }

#endif

    protected virtual void Reset()
    {
        this.LoadComponents();
        this.ResetValues();
    }

    /// <summary>
    /// For overriding
    /// </summary>
    protected virtual void LoadComponents() { }

    /// <summary>
    /// Use to Reset Value (Reset button).
    /// For overriding
    /// </summary>
    protected virtual void ResetValues() { }

    protected void LoadComponentInChildren<TComponent>(out TComponent component)
        where TComponent : Component
    {
        component = GetComponentInChildren<TComponent>();
        if (component != null) return;

        Debug.LogError($"Can't load component of type {nameof(TComponent)}", gameObject);
    }

    protected void LoadComponentInCtrl<TComponent>(out TComponent component)
        where TComponent : Component
    {
        component = GetComponent<TComponent>();
        if (component != null) return;

        Debug.LogError($"Can't load component of type {nameof(TComponent)}", gameObject);
    }

    protected void LoadCtrl<TComponent>(out TComponent component)
        where TComponent : Component
    {
        component = GetComponentInParent<TComponent>();
        if (component != null) return;

        Debug.LogError($"Can't load component of type {nameof(TComponent)}", gameObject);
    }

    protected void LoadTransformInChildrenByName(out Transform t, string name)
    {
        t = transform.Find(name);
        if (t != null) return;

        Debug.LogError($"Can't find child Transform with name {name}", gameObject);
    }

}