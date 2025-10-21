using Components.Misc;
using Unity.Entities;
using Components.Player;
using Components.GameEntity.Damage;
using UnityEngine.Rendering.Universal;
using LitMotion;
using Systems.Initialization.GameEntity.Damage.DeadResolve;

namespace Systems.Initialization.Player.Misc
{
    [UpdateInGroup(typeof(DeadResolveSystemGroup))]
    public partial class SetPostProcessingOnPlayerDeadSystem : SystemBase
    {
        private EntityQuery query0;

        protected override void OnCreate()
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag>()
                .WithAll<
                    DeadEvent>()
                .Build();

            this.RequireForUpdate(this.query0);
            this.RequireForUpdate<GlobalVolumeHolder>();
        }

        protected override void OnUpdate()
        {
            bool isPlayerDead = this.query0.CalculateEntityCount() != 0;
            if (!isPlayerDead) return;

            var globalVolume = SystemAPI.GetSingleton<GlobalVolumeHolder>().Value.Value;

            if (globalVolume.profile.TryGet(out ChromaticAberration chromaticAberration))
            {
                chromaticAberration.active = true;
                float presetIntensity = chromaticAberration.intensity.value;

                LMotion.Create(0, presetIntensity, 1f)
                    .Bind(tempValue => chromaticAberration.intensity.value = tempValue);
            }

            if (globalVolume.profile.TryGet(out Vignette vignette))
            {
                vignette.active = true;
                float presetIntensity = vignette.intensity.value;

                LMotion.Create(0, presetIntensity, 1f)
                    .Bind(tempValue => vignette.intensity.value = tempValue);
            }
        }

    }

}