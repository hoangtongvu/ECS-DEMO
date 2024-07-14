using Components.Unit;
using Core.Unit;
using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Utilities;


namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class MoveAffecterMapInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.LoadUnitProfileSOs(out var unitProfiles);

            int length = unitProfiles.Length;
            int enumLength = Enum.GetNames(typeof(MoveAffecter)).Length;
            int initialCap = length * enumLength;

            var moveAffecterMap = new MoveAffecterMap
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
                    var moveAffecter = (MoveAffecter)j;
                    if (!profile.MoveAffecterPriorities.TryGetValue(moveAffecter, out byte priority)) continue;

                    var moveAffecterId = new MoveAffecterId(profile.UnitType, moveAffecter, profile.LocalIndex);
                    if (moveAffecterMap.Value.TryAdd(moveAffecterId, priority))
                    {
                        // Debug.Log($"Added [{moveAffecterId}] with value: {priority}");
                        maxPriority = maxPriority < priority ? priority : maxPriority;
                        continue;
                    }

                    Debug.LogError($"MoveAffecterMap already contains MoveAffecterId: {moveAffecterId}");

                }

                maxPriority++;

                // Add remaining in MoveAffecter.
                for (int j = 0; j < enumLength; j++)
                {
                    var moveAffecter = (MoveAffecter)j;
                    if (profile.MoveAffecterPriorities.ContainsKey(moveAffecter)) continue;

                    var moveAffecterId = new MoveAffecterId(profile.UnitType, moveAffecter, profile.LocalIndex);

                    if (moveAffecterMap.Value.TryAdd(moveAffecterId, maxPriority))
                    {
                        // Debug.Log($"Added [{moveAffecterId}] with value: {maxPriority}");
                        maxPriority++;
                        continue;
                    }

                    Debug.LogError($"MoveAffecterMap already contains MoveAffecterId: {moveAffecterId}");
                }


                
            }


            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(moveAffecterMap);


        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }

        private void LoadUnitProfileSOs(out UnitProfileSO[] unitProfiles) => unitProfiles = Resources.LoadAll<UnitProfileSO>("UnitProfiles");
    }
}