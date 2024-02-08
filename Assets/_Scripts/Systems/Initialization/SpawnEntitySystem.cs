using Components;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Initialization
{
    // Systems that create sync points should update in initialization group
    // so they won't force job scheduled in simulation group to complete.
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct SpawnEntitySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitySpawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // make sure this Update only run once.
            state.Enabled = false;

            var entitySpawner = SystemAPI.GetSingleton<EntitySpawner>();

            var em = state.EntityManager;
            var allocator = state.WorldUpdateAllocator;

            var entities = em.Instantiate(entitySpawner.prefab, entitySpawner.spawnCount, allocator);

            // after the sync point caused by em.Instantiate
            // previous state.Dependency is no longer valid
            // we have to reset it.
            state.Dependency = default;

            var rows = (int)math.ceil(math.sqrt(entitySpawner.spawnCount));
            var offset = entitySpawner.spacing * 0.5f;

            if (SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<EntityRefElement> refBuffer))
            {
                refBuffer.AddRange(entities.Reinterpret<EntityRefElement>());
            }

            var job = new SetComponentJob
            {
                entities = entities,
                lookupTransform = SystemAPI.GetComponentLookup<LocalTransform>(),
                lookupMoveDirection = SystemAPI.GetComponentLookup<MoveDirection>(),
                rows = rows,
                offset = offset,
                spacing = entitySpawner.spacing,
            };

            state.Dependency = job.ScheduleParallelByRef(entities.Length, 32, state.Dependency);
        }

        [BurstCompile]
        private partial struct SetComponentJob : IJobParallelForBatch
        {
            [ReadOnly] public NativeArray<Entity> entities;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<LocalTransform> lookupTransform;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<MoveDirection> lookupMoveDirection;

            public int rows;
            public float offset;
            public float spacing;

            public void Execute(int startIndex, int count)
            {
                var length = startIndex + count;
                var random = new Random((uint)(startIndex + 1));

                for (var i = startIndex; i < length; i++)
                {
                    var entity = entities[i];
                    var transformRef = lookupTransform.GetRefRWOptional(entity);
                    var directionRef = lookupMoveDirection.GetRefRWOptional(entity);

                    if (Hint.Likely(transformRef.IsValid))
                    {
                        int row = i / rows;
                        int col = i % rows;

                        transformRef.ValueRW.Position = new float3
                        {
                            x = (col * spacing) - offset,
                            y = 0f,
                            z = (row * spacing) - offset,
                        };
                    }

                    if (Hint.Likely(directionRef.IsValid))
                    {
                        directionRef.ValueRW.value = new float3
                        {
                            x = random.NextFloat(-1f, 1f),
                            y = 0f,
                            z = random.NextFloat(-1f, 1f),
                        };
                    }
                }
            }
        }
    }
}
