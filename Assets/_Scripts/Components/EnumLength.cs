using Components;
using Core.GameResource;
using System;
using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(EnumLength<ResourceType>))]

namespace Components
{
    public struct EnumLength<TEnum> : IComponentData where TEnum : Enum
    {
        public int Value;
    }
}
