using AYellowpaper.SerializedCollections;
using Components;
using Components.GameEntity;
using Components.Unit;
using Components.Unit.MyMoveCommand;
using Core.Unit;
using Core.Unit.MyMoveCommand;
using System;
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
                    , AfterBakedPrefabsElement>()
                .Build();

            this.RequireForUpdate(this.query);
            this.RequireForUpdate<EnumLength<MoveCommandSource>>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var profiles = this.query.GetSingleton<UnitProfilesSOHolder>().Value.Value.Profiles;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            int profileCount = profiles.Count;
            int commandSourceCount = Enum.GetNames(typeof(MoveCommandSource)).Length;
            int initialCap = profileCount * commandSourceCount;

            var commandSourceMap = new MoveCommandSourceMap
            {
                Value = new(initialCap, Allocator.Persistent),
            };

            this.AddingPriorities(in profiles, commandSourceCount, in commandSourceMap);

            su.AddOrSetComponentData(commandSourceMap);

        }

        private void AddingPriorities(
            in SerializedDictionary<Core.Unit.UnitId, UnitProfileElement> profiles
            , int commandSourceCount
            , in MoveCommandSourceMap commandSourceMap)
        {
            foreach (var pair in profiles)
            {
                byte maxPriority = 0;

                this.AddExistingPriorities(
                    in pair
                    , commandSourceCount
                    , ref maxPriority
                    , in commandSourceMap);

                maxPriority++;

                this.AddRemaningPriorities(
                    in pair
                    , commandSourceCount
                    , ref maxPriority
                    , in commandSourceMap);

            }

        }

        private void AddExistingPriorities(
            in KeyValuePair<Core.Unit.UnitId, UnitProfileElement> pair
            , int commandSourceCount
            , ref byte maxPriority
            , in MoveCommandSourceMap commandSourceMap)
        {
            for (int j = 0; j < commandSourceCount; j++)
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
            in KeyValuePair<Core.Unit.UnitId, UnitProfileElement> pair
            , int commandSourceCount
            , ref byte maxPriority
            , in MoveCommandSourceMap commandSourceMap)
        {
            for (int j = 0; j < commandSourceCount; j++)
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