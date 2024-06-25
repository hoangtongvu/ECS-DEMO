using Core.CustomIdentification;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core
{

    // TODO: This can be turned into Generic class.
    public class UnityTransformMapRegister : MonoBehaviour
    {
        [SerializeField] private UniqueId Id;
        [SerializeField] private Transform Target;


        private void Awake()
        {
            //this.em = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (this.Target == false) this.Target = this.transform;


            //UnityTransformMap transformMap = this.GetUnityTransformMap();

            //if (transformMap == null)
            //{
            //    Debug.LogError("ObjMap not found");
            //    return;
            //}

            //if (!transformMap.Value.TryAdd(this.Id, this.Target))
            //{
            //    Debug.LogError($"Một Unity Transform khác đã được đăng ký với Id={this.Id}", this.Target);
            //}

            // Send a RegisterMessage.
            MapRegisterMessenger.MessagePublisher.Publish(new RegisterMessage<UniqueId, UnityEngine.Transform>
            {
                ID = this.Id,
                TargetRef = this.Target,
            });
        }

        //private UnityTransformMap GetUnityTransformMap()
        //{
        //    EntityQuery entityQuery = this.em.CreateEntityQuery(typeof(UnityTransformMap));
        //    return entityQuery.GetSingleton<UnityTransformMap>();
        //}


    }
}