using Components.GameResource;
using Core.GameResource;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameResource
{

    public class GameResourceAuthoring : MonoBehaviour
    {

        private class Baker : Baker<GameResourceAuthoring>
        {
            public override void Bake(GameResourceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                int length = Enum.GetNames(typeof(ResourceType)).Length;

                //var gameResourceElements = AddBuffer<GameResourceElement>(entity);

                //for (int i = 0; i < length; i++)
                //{
                //    gameResourceElements.Add(new GameResourceElement
                //    {
                //        Value = new()
                //        {
                //            Quantity = 0,
                //            ResourceType = (ResourceType) i,
                //        }
                //    });
                //}



                Dictionary<ResourceType, uint> resourceMap = new();

                for (int i = 0; i < length; i++)
                {
                    var type = (ResourceType) i;
                    if (resourceMap.TryAdd(type, 0)) continue;
                    Debug.LogError($"GameResourceMap already contained {type}");
                }

                AddComponentObject(entity, new GameResourceMap
                {
                    Value = resourceMap,
                });
            }
        }
    }
}
