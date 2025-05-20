using AYellowpaper.SerializedCollections;
using Components.GameEntity;
using Components.Unit;
using Components.Unit.MyMoveCommand;
using Core.Unit;
using Core.Unit.MyMoveCommand;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Utilities;

namespace Systems.Initialization.Unit.MyMoveCommand
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class MoveCommandSourceMapInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfilesSOHolder
                    , BakedGameEntityProfileElement>()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var profiles = this.query.GetSingleton<UnitProfilesSOHolder>().Value.Value.Profiles;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            int profileCount = profiles.Count;
            int initialCap = profileCount * MoveCommandSource_Length.Value;

            var commandSourceMap = new MoveCommandSourceMap
            {
                Value = new(initialCap, Allocator.Persistent),
            };

            this.AddingPriorities(in profiles, in commandSourceMap);

            su.AddOrSetComponentData(commandSourceMap);

        }

        private void AddingPriorities(
            in SerializedDictionary<UnitProfileId, UnitProfileElement> profiles
            , in MoveCommandSourceMap commandSourceMap)
        {
            foreach (var pair in profiles)
            {
                byte maxPriority = 0;

                this.AddExistingPriorities(
                    in pair
                    , ref maxPriority
                    , in commandSourceMap);

                maxPriority++;

                this.AddRemaningPriorities(
                    in pair
                    , ref maxPriority
                    , in commandSourceMap);

            }

        }

        private void AddExistingPriorities(
            in KeyValuePair<UnitProfileId, UnitProfileElement> pair
            , ref byte maxPriority
            , in MoveCommandSourceMap commandSourceMap)
        {
            for (int j = 0; j < MoveCommandSource_Length.Value; j++)
            {
                var commandSource = (MoveCommandSource)j;
                if (!pair.Value.MoveCommandSourcePriorities.TryGetValue(commandSource, out byte priority)) continue;

                var commandSourceId = new MoveCommandSourceId(pair.Key.UnitType, commandSource, pair.Key.VariantIndex);
                if (commandSourceMap.Value.TryAdd(commandSourceId, priority))
                {
                    //Debug.Log($"Added [{commandSourceId}] with value: {priority}");
                    maxPriority = maxPriority < priority ? priority : maxPriority;
                    continue;
                }

                Debug.LogError($"MoveCommandSource already contains MoveCommandSourceId: {commandSourceId}");

            }

        }

        private void AddRemaningPriorities(
            in KeyValuePair<UnitProfileId, UnitProfileElement> pair
            , ref byte maxPriority
            , in MoveCommandSourceMap commandSourceMap)
        {
            for (int j = 0; j < MoveCommandSource_Length.Value; j++)
            {
                var commandSource = (MoveCommandSource)j;
                if (pair.Value.MoveCommandSourcePriorities.ContainsKey(commandSource)) continue;
                var commandSourceId = new MoveCommandSourceId(pair.Key.UnitType, commandSource, pair.Key.VariantIndex);

                if (commandSourceMap.Value.TryAdd(commandSourceId, maxPriority))
                {
                    //Debug.Log($"Added [{commandSourceId}] with value: {maxPriority}");
                    maxPriority++;
                    continue;
                }

                Debug.LogError($"MoveCommandSource already contains MoveCommandSourceId: {commandSourceId}");

            }

        }

    }

}