using AYellowpaper.SerializedCollections;
using Components.Unit.Misc;
using Core.Unit.Misc;
using Core.Unit.MyMoveCommand;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Utilities;
using Utilities.Helpers.Misc;

namespace Systems.Initialization.Unit.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class MoveCommandPrioritiesMapInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    MoveCommandPrioritiesSOHolder>()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var basePriorities = this.query.GetSingleton<MoveCommandPrioritiesSOHolder>().Value.Value.MoveCommandSourcePriorities;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            int initialCap = ArmedState_Length.Value * MoveCommandSource_Length.Value;

            var commandPrioritiesMap = new MoveCommandPrioritiesMap
            {
                Value = new(initialCap, Allocator.Persistent),
            };

            this.AddingPriorities(in basePriorities, ref commandPrioritiesMap);

            su.AddOrSetComponentData(commandPrioritiesMap);

        }

        private void AddingPriorities(
            in SerializedDictionary<ArmedState, PrioritiesElement> basePriorities
            , ref MoveCommandPrioritiesMap commandPrioritiesMap)
        {
            foreach (var pair in basePriorities)
            {
                this.AddPriorities(
                    in pair
                    , ref commandPrioritiesMap);

            }

        }

        private void AddPriorities(
            in KeyValuePair<ArmedState, PrioritiesElement> pair
            , ref MoveCommandPrioritiesMap commandPrioritiesMap)
        {
            int startIndex = MoveCommandPrioritiesHelper.GetStartIndexInMap(pair.Key);
            int tempIndex = 0;
            var addedCommandSources = new NativeHashSet<byte>(pair.Value.Value.Count, Allocator.Temp);

            // Add Dev-defined sources
            foreach (var item in pair.Value.Value)
            {
                byte priority = item.Key.Priority;
                var source = item.Key.MoveCommandSource;

                if (priority != tempIndex)
                    throw new System.Exception($"Priorities for {nameof(ArmedState)} = {pair.Key} are not in continuous order, Fix it.");

                commandPrioritiesMap.Value[startIndex + tempIndex] = source;
                addedCommandSources.Add((byte)source);
                tempIndex++;
            }

            // Add remaining sources except the "None"
            for (byte i = 0; i < MoveCommandSource_Length.Value; i++)
            {
                if (addedCommandSources.Contains(i)) continue;
                var source = (MoveCommandSource)i;

                if (source == MoveCommandSource.None) continue;

                commandPrioritiesMap.Value[startIndex + tempIndex] = (MoveCommandSource)i;
                tempIndex++;
            }

            // Explicitly Add "None" at the end
            commandPrioritiesMap.Value[startIndex + tempIndex] = MoveCommandSource.None;

            addedCommandSources.Dispose();

        }

    }

}