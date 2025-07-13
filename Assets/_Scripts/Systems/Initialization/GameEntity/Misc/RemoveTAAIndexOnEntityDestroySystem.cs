using Components.GameEntity.Misc;
using Components.Misc.Presenter;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(DestroyEntityWithTagSystem))]
    public partial class RemoveTAAIndexOnEntityDestroySystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedDestroyBasePresenterTag
                    , NeedDestroyEntityTag
                    , TransformAccessArrayIndex>()
                .Build();

            this.RequireForUpdate(this.query);
        }

        protected override void OnUpdate()
        {
            var presentersTransformAccessArrayGO = SystemAPI.GetSingleton<PresentersTransformAccessArrayGOHolder>().Value.Value;

            foreach (var transformAccessIndexRef in SystemAPI
                .Query<
                    RefRW<TransformAccessArrayIndex>>()
                .WithAll<
                    NeedDestroyBasePresenterTag
                    , NeedDestroyEntityTag>())
            {
                presentersTransformAccessArrayGO.RemoveTransformAt(transformAccessIndexRef.ValueRO.Value);
                transformAccessIndexRef.ValueRW = TransformAccessArrayIndex.Invalid;
            }

        }

    }

}