using Unity.Mathematics;

namespace Core.Unit.Misc
{
    [System.Serializable]
	public struct AttackConfigsFloat
    {
        public float AutoAttackDetectionRadius;
        public float MinAttackDistance;
        public float MaxAttackDistance;

        public AttackConfigsFloat()
        {
            this.AutoAttackDetectionRadius = 12f;
            this.MinAttackDistance = 0f;
            this.MaxAttackDistance = 2f;
        }

        public AttackConfigs ToHalfVersion()
        {
            return new AttackConfigs
            {
                AutoAttackDetectionRadius = new(this.AutoAttackDetectionRadius),
                MinAttackDistance = new(this.MinAttackDistance),
                MaxAttackDistance = new(this.MaxAttackDistance),
            };
        }

    }

    [System.Serializable]
	public struct AttackConfigs
    {
        public half AutoAttackDetectionRadius;
        public half MinAttackDistance;
        public half MaxAttackDistance;

    }

}