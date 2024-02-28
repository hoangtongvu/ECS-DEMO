using Components;
using Components.CustomIdentification;
using Unity.Entities;
using UnityEngine;


public class UnityObjectMapRegister : MonoBehaviour
{
    public UniqueId Id;
    public UnityEngine.Object Target;
    private EntityManager em;


    private void Awake()
    {
        this.em = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (this.Target == false) this.Target = this.gameObject;


        UnityObjectMap objectMap = this.GetUnityObjectMap();

        if (objectMap == null)
        {
            Debug.LogError("ObjMap not found");
            return;
        }

        if (!objectMap.Value.TryAdd(this.Id, this.Target))
        {
            Debug.LogError($"Một Unity Object khác đã được đăng ký với Id={this.Id}", this.Target);
        }
    }

    private UnityObjectMap GetUnityObjectMap()
    {
        EntityQuery entityQuery = this.em.CreateEntityQuery(typeof(UnityObjectMap));
        return entityQuery.GetSingleton<UnityObjectMap>();
    }


}
