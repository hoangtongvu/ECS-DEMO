using Components.MyEntity.EntitySpawning;
using Core.Tool;
using Core.Unit;
using System;
using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(SpawnTypeTag<UnitType>))]
[assembly: RegisterGenericComponentType(typeof(SpawnTypeTag<ToolType>))]

namespace Components.MyEntity.EntitySpawning
{
    public struct SpawnTypeTag<TEnum> : IComponentData where TEnum : Enum
    {
    }

}
