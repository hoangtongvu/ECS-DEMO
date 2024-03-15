using Components;
using Components.CustomIdentification;
using Unity.Entities;
using UnityEngine;


public class UnityTransformMapRegister : MonoBehaviour
{
    public UniqueId Id;
    public UnityEngine.Transform Target;
    private EntityManager em;


    private void Awake()
    {
        this.em = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (this.Target == false) this.Target = this.transform;


        UnityTransformMap transformMap = this.GetUnityTransformMap();

        if (transformMap == null)
        {
            Debug.LogError("ObjMap not found");
            return;
        }

        if (!transformMap.Value.TryAdd(this.Id, this.Target))
        {
            Debug.LogError($"Một Unity Transform khác đã được đăng ký với Id={this.Id}", this.Target);
        }
    }

    private UnityTransformMap GetUnityTransformMap()
    {
        EntityQuery entityQuery = this.em.CreateEntityQuery(typeof(UnityTransformMap));
        return entityQuery.GetSingleton<UnityTransformMap>();
    }


}
