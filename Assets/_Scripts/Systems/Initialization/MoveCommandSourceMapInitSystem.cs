using Components;
using Components.GameEntity;
using Components.Unit;
using Components.Unit.MyMoveCommand;
using Core.Unit.MyMoveCommand;
using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Utilities;

namespace Systems.Initialization
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
            int enumLength = Enum.GetNames(typeof(MoveCommandSource)).Length;
            int initialCap = profileCount * enumLength;

            var commandSourceMap = new MoveCommandSourceMap
            {
                Value = new(initialCap, Allocator.Persistent),
            };

            foreach (var pair in profiles)
            {
                // Add all priorities exist in Dictionary first.
                byte maxPriority = 0;
                for (int j = 0; j < enumLength; j++)
                {
                    var commandSource = (MoveCommandSource)j;
                    if (!pair.Value.MoveCommandSourcePriorities.TryGetValue(commandSource, out byte priority)) continue;

                    var commandSourceId = new MoveCommandSourceId(pair.Key.UnitType, commandSource, pair.Key.VariantIndex);
                    if (commandSourceMap.Value.TryAdd(commandSourceId, priority))
                    {
                        Debug.Log($"Added [{commandSourceId}] with value: {priority}");
                        maxPriority = maxPriority < priority ? priority : maxPriority;
                        continue;
                    }

                    Debug.LogError($"MoveCommandSource already contains MoveCommandSourceId: {commandSourceId}");

                }

                maxPriority++;

                // Add remaining in MoveAffecter.
                for (int j = 0; j < enumLength; j++)
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

            su.AddOrSetComponentData(commandSourceMap);

        }

    }

}