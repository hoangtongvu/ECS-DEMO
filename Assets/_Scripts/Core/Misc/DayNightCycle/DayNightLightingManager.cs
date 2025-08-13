using UnityEngine;

namespace Core.Misc.DayNightCycle
{
    [ExecuteAlways]
    public class DayNightLightingManager : SaiMonoBehaviour
    {
        [SerializeField] private Light directionalLight;
        [SerializeField] private DayNightPresetSO preset;

        [SerializeField]
        [Range(0, 24)]
        private float hourInDay = 7f;

        public DayNightPresetSO Preset
        {
            get => preset;
            set
            {
                preset = value;
                hourInDay = preset.StartHourInTheDay;
            }
        }

        private void OnValidate()
        {
            if (this.directionalLight != null) return;

            if (RenderSettings.sun != null)
            {
                this.directionalLight = RenderSettings.sun;
            }
            else
            {
                var lights = GameObject.FindObjectsOfType<Light>();
                foreach (var light in lights)
                {
                    if (light.type != LightType.Directional) continue;

                    this.directionalLight = light;
                    break;
                }
            }

        }

        private void Update()
        {
            if (this.preset == null) return;

            const float maxHourInADay = 24f;

            if (Application.isPlaying)
            {
                this.hourInDay += Time.deltaTime / (this.preset.MinutesPerDayCycle * 60) * maxHourInADay;
                this.hourInDay %= maxHourInADay;
                this.UpdateLighting(this.hourInDay / maxHourInADay);
            }
            else
            {
                this.UpdateLighting(this.hourInDay / maxHourInADay);
            }
        }

        private void UpdateLighting(float t)
        {
            RenderSettings.ambientLight = this.preset.AmbientColor.Evaluate(t);
            RenderSettings.fogColor = this.preset.FogColor.Evaluate(t);

            if (this.directionalLight != null)
            {
                this.directionalLight.color = this.preset.DirectionalColor.Evaluate(t);
                this.directionalLight.transform.localRotation = Quaternion.Euler(t * 360f - 90f, -170f, 0);
            }
        }

    }

}