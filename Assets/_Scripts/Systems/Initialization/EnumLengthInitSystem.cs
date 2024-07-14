using Components;
using Core.GameResource;
using System;
using Unity.Entities;
using Utilities;

// [assembly: RegisterGenericSystemType(typeof(EnumLengthInitSystem<ResourceType>))] // This only for ISystem generic

namespace Systems.Initialization
{
    public partial class ResourceTypeLengthInitSystem : EnumLengthInitSystem<ResourceType> { }




    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class EnumLengthInitSystem<TEnum> : SystemBase where TEnum : Enum
    {

        protected override void OnCreate()
        {
            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new EnumLength<TEnum>
                {
                    Value = Enum.GetNames(typeof(TEnum)).Length,
                });
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }

    }

}