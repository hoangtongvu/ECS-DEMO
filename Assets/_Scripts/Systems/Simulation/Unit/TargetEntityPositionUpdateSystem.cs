//using Unity.Entities;
//using Unity.Burst;
//using Components.MyEntity;
//using Components;
//using Unity.Transforms;

//namespace Systems.Simulation.Unit
//{

//    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//    [BurstCompile]
//    public partial struct TargetEntityPositionUpdateSystem : ISystem
//    {
//        private float timeCounterSecond; // Can make this become singleton component
//        private float updateTimeIntervalSecond; // Can make become this singleton component


//        [BurstCompile]
//        public void OnCreate(ref SystemState state)
//        {
//            this.timeCounterSecond = 0f;
//            this.updateTimeIntervalSecond = 0.5f;

//            EntityQuery query = SystemAPI.QueryBuilder()
//                .WithAll<
//                    TargetEntity
//                    , TargetPosition
//                    , TargetPosChangedTag>()
//                .Build();

//            state.RequireForUpdate(query);
//        }

//        [BurstCompile]
//        public void OnUpdate(ref SystemState state)
//        {
//            this.timeCounterSecond += SystemAPI.Time.DeltaTime;
//            if (this.timeCounterSecond < this.updateTimeIntervalSecond) return;
//            this.timeCounterSecond = 0;

//            foreach (var (targetEntityRef, targetPosRef, targetPosChangedTag) in
//                SystemAPI.Query<
//                    RefRO<TargetEntity>
//                    , RefRW<TargetPosition>
//                    , EnabledRefRW<TargetPosChangedTag>>()
//                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
//            {

//                var targetEntity = targetEntityRef.ValueRO.Value;
//                if (!SystemAPI.Exists(targetEntity)) continue;

//                var transformRef = SystemAPI.GetComponentRO<LocalTransform>(targetEntity);

//                targetPosRef.ValueRW.Value = transformRef.ValueRO.Position;
//                targetPosChangedTag.ValueRW = true;


//            }

//        }


//    }

//}