using Components.GameEntity;
using Components.Misc.WorldMap.WorldBuilding;
using Components.Misc.WorldMap.WorldBuilding.BuildingConstruction;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding.BuildingConstruction
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BlueprintMaterialHolderInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    GameBuildingProfilesSOHolder
                    , BakedGameEntityProfileElement>()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            var gameBuildingGlobalConfigs = this.query.GetSingleton<GameBuildingProfilesSOHolder>().Value.Value.GameBuildingGlobalConfigs;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            su.AddOrSetComponentData(new BlueprintMaterialHolder
            {
                Value = gameBuildingGlobalConfigs.BlueprintMaterial,
            });

        }

    }

}