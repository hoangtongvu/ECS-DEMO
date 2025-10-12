using Components.Misc.TerrainBaking;
using Components.Tag;
using Core.Misc;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace Authoring.Misc
{
    public class TerrainPrefabAndColliderInjector : MonoBehaviour
    {
        public Terrain Terrain;
        public GameObject TerainPresenter;
        public CollisionLayer BelongsTo;
        public CollisionLayer CollidesWith;
        public int GroupIndex;
        public Unity.Physics.TerrainCollider.CollisionMethod CollisionMethod;

        private class Baker : Baker<TerrainPrefabAndColliderInjector>
        {
            public override void Bake(TerrainPrefabAndColliderInjector authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new TerrainPresenterPrefabHolder
                {
                    Value = authoring.TerainPresenter,
                });

                AddComponent(entity, new TerrainPosition(authoring.transform.position));

                this.AddColliderComponents(authoring, in entity);
            }

            private void AddColliderComponents(TerrainPrefabAndColliderInjector authoring, in Entity entity)
            {
                var filter = new CollisionFilter()
                {
                    BelongsTo = (uint)authoring.BelongsTo,
                    CollidesWith = (uint)authoring.CollidesWith,
                    GroupIndex = authoring.GroupIndex,
                };

                AddComponent(entity, this.CreateTerrainCollider(authoring.Terrain.terrainData, filter, authoring.CollisionMethod));
                AddComponent<GroundTag>(entity);

                // This is needed for the collider to be registered properly in the physics world
                AddSharedComponent(entity, new PhysicsWorldIndex());
            }

            private PhysicsCollider CreateTerrainCollider(
                TerrainData terrainData
                , CollisionFilter filter
                , Unity.Physics.TerrainCollider.CollisionMethod collisionMethod)
            {
                var physicsCollider = new PhysicsCollider();

                var heightMapRes = terrainData.heightmapResolution;
                var size = new int2(heightMapRes);
                var scale = terrainData.heightmapScale;

                var colliderHeights = new NativeArray<float>(heightMapRes * heightMapRes,
                    Allocator.Temp);

                var terrainHeights = terrainData.GetHeights(0, 0, heightMapRes, heightMapRes);

                for (int j = 0; j < size.y; j++)
                {
                    for (int i = 0; i < size.x; i++)
                    {
                        var h = terrainHeights[i, j];
                        colliderHeights[j + i * size.x] = h;
                    }
                }

                physicsCollider.Value = Unity.Physics.TerrainCollider.Create(
                    colliderHeights
                    , size
                    , scale
                    , collisionMethod
                    , filter);

                return physicsCollider;
            }
        }
    }
}
