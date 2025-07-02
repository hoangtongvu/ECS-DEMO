using Components.GameEntity.Misc;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.GameEntity.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class FlashOnTakeHitMaterialInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    GeneralEntityDataSOHolder>()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var generalEntityDataSO = this.query.GetSingleton<GeneralEntityDataSOHolder>().Value.Value;

            su.AddOrSetComponentData(new FlashOnTakeHitMaterial
            {
                Value = generalEntityDataSO.FlashOnTakeHitMaterial,
            });

        }

    }

}