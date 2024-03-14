using Components;
using Components.CustomIdentification;
using Core.Animator;
using Unity.Entities;
using UnityEngine;


public class BaseAnimatorMapRegister : MonoBehaviour
{
    [SerializeField] private UniqueId Id;
    [SerializeField] private BaseAnimator Target;
    private EntityManager em;


    private void Awake()
    {
        this.em = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (this.Target == false) this.Target = this.gameObject.GetComponent<BaseAnimator>();


        BaseAnimatorMap objectMap = this.GetBaseAnimatorMap();

        if (objectMap == null)
        {
            Debug.LogError("ObjMap not found");
            return;
        }

        if (!objectMap.Value.TryAdd(this.Id, this.Target))
        {
            Debug.LogError($"Một Base Animator khác đã được đăng ký với Id={this.Id}", this.Target);
        }
    }

    private BaseAnimatorMap GetBaseAnimatorMap()
    {
        EntityQuery entityQuery = this.em.CreateEntityQuery(typeof(BaseAnimatorMap));
        return entityQuery.GetSingleton<BaseAnimatorMap>();
    }


}
