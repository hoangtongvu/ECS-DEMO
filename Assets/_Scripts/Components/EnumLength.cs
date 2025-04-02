using Components;
using Core.GameResource;
using Core.Unit.MyMoveCommand;
using System;
using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(EnumLength<ResourceType>))]
[assembly: RegisterGenericComponentType(typeof(EnumLength<MoveCommandSource>))]

namespace Components
{
    public struct EnumLength<TEnum> : IComponentData where TEnum : Enum
    {
        public int Value;
    }

}
