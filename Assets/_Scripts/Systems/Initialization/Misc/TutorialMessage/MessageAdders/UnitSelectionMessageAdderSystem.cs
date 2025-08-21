using Components.GameEntity.Misc;
using Components.Misc.TutorialMessage;
using Components.Player;
using Components.Unit.Misc;
using Systems.Initialization.GameEntity.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.Misc.TutorialMessage.MessageAdders
{
    [UpdateInGroup(typeof(NewlySpawnedTagProcessSystemGroup))]
    [UpdateAfter(typeof(SetSpawnerFactionToSpawnedEntitySystem))]
    [BurstCompile]
    public partial struct UnitSelectionMessageAdderSystem : ISystem
    {
        private EntityQuery playerQuery;
        private EntityQuery unitQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.playerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , FactionIndex>()
                .Build();

            this.unitQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitTag
                    , FactionIndex>()
                .Build();

            state.RequireForUpdate(this.playerQuery);
            state.RequireForUpdate(this.unitQuery);
            state.RequireForUpdate<TutorialMessageList>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            byte playerFactionIndex = this.playerQuery.GetSingleton<FactionIndex>().Value;
            var unitFactionIndice = this.unitQuery.ToComponentDataArray<FactionIndex>(Allocator.Temp);

            int length = unitFactionIndice.Length;

            for (int i = 0; i < length; i++)
            {
                if (playerFactionIndex != unitFactionIndice[i].Value) continue;

                state.Enabled = false;
                var tutorialMessageList = SystemAPI.GetSingleton<TutorialMessageList>().Value;

                tutorialMessageList.Add(new()
                {
                    String = "Press <b>[Right Mouse Button]</b> on the recruited unit to <b>select</b> him",
                    TextDuration = new(9f),
                });

                tutorialMessageList.Add(new()
                {
                    String = "Press <b>[Right Mouse Button]</b> on the ground to move the selected unit",
                    TextDuration = new(9f),
                });

                tutorialMessageList.Add(new()
                {
                    String = "Hold <b>[Right Mouse Button]</b> and drag to <b>select</b> multiple units",
                    TextDuration = new(9f),
                });

                tutorialMessageList.Add(new()
                {
                    String = "Press <b>[Q]</b> to <b>deselect</b> all units",
                    TextDuration = new(7f),
                });

                break;
            }
            
        }

    }

}