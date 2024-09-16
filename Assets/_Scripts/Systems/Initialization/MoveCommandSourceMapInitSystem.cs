using Components.Unit.MyMoveCommand;
using Core.Unit;
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
        protected override void OnCreate()
        {
            this.LoadUnitProfileSOs(out var unitProfiles);

            int length = unitProfiles.Length;
            int enumLength = Enum.GetNames(typeof(MoveCommandSource)).Length;
            int initialCap = length * enumLength;

            var commandSourceMap = new MoveCommandSourceMap
            {
                Value = new(initialCap, Allocator.Persistent),
            };

            for (int i = 0; i < length; i++)
            {
                var profile = unitProfiles[i];

                // Add all exist in Dictionary first.
                byte maxPriority = 0;
                for (int j = 0; j < enumLength; j++)
                {
                    var commandSource = (MoveCommandSource)j;
                    if (!profile.MoveCommandSourcePriorities.TryGetValue(commandSource, out byte priority)) continue;

                    var commandSourceId = new MoveCommandSourceId(profile.UnitType, commandSource, profile.LocalIndex);
                    if (commandSourceMap.Value.TryAdd(commandSourceId, priority))
                    {
                        //Debug.Log($"Added [{commandSourceId}] with value: {priority}");
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
                    if (profile.MoveCommandSourcePriorities.ContainsKey(commandSource)) continue;

                    var commandSourceId = new MoveCommandSourceId(profile.UnitType, commandSource, profile.LocalIndex);

                    if (commandSourceMap.Value.TryAdd(commandSourceId, maxPriority))
                    {
                        //Debug.Log($"Added [{commandSourceId}] with value: {maxPriority}");
                        maxPriority++;
                        continue;
                    }

                    Debug.LogError($"MoveCommandSource already contains MoveCommandSourceId: {commandSourceId}");
                }


                
            }


            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(commandSourceMap);


        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }

        private void LoadUnitProfileSOs(out UnitProfileSO[] unitProfiles) => unitProfiles = Resources.LoadAll<UnitProfileSO>("UnitProfiles");
    }
}