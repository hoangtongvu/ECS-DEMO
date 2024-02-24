using Components.Player;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Camera
{
    public class Movement : MonoBehaviour
    {

        [SerializeField] private EntityManager entityManager;
        [SerializeField] private PlayerRefsTransform playerRefsTransform;
        [SerializeField] private Vector3 addPosition;

        private void Awake()
        {
            this.entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void LateUpdate()
        {
            this.SetPlayerRefsTransform();
            this.SetCamPos();
        }


        private void SetCamPos()
        {
            float3 playerPos = this.playerRefsTransform.transform.Position;

            Vector3 newPos = new Vector3(playerPos.x, playerPos.y, playerPos.z);

            newPos = newPos + this.addPosition;

            transform.position = newPos;
        }

        public void SetPlayerRefsTransform()
        {
            EntityQuery query = entityManager.CreateEntityQuery(typeof(PlayerRefsTransform));

            this.playerRefsTransform = query.GetSingleton<PlayerRefsTransform>();
        }


    }

}