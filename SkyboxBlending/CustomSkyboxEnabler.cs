using UnityEngine;

namespace SagesUtilsRelease
{
    public class CustomSkyboxEnabler : MonoSingleton<CustomSkyboxEnabler>
    {
        public float startBlend;

        void Start()
        {
            RenderSettings.skybox.SetFloat("_Blend", startBlend);
        }
    }
}
