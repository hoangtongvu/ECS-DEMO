using Components.Damage;
using Unity.Entities;

namespace Aspect
{
    //Note: Aspect must be put in namespace in order to shown in ECS inspector (this is a bug?) 
    public readonly partial struct DmgReceiverAspect : IAspect
    {
        public readonly RefRW<HpComponent> HpComponentRef;
        public readonly RefRW<HpChangeState> HpChangeStateRef;
        public readonly RefRW<AliveState> AliveStateRef;
    }
}