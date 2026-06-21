using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using static UnityEngine.UIElements.UIR.BestFitAllocator;

namespace SagesUtilsRelease
{
    public class MaterialCubemap : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Material that will be given a cubemap.")]
        public Material material;
        [Tooltip("Option to apply cubemap to all materials in renderer.")]
        public bool applyToAllMaterials = false;

        [Header("Reflection Shader Settings")]
        [Tooltip("Master Shader reflection mask.")]
        public Texture2D reflectionMask;
        [Tooltip("Cubemap texture that will be applied to the choosen material.")]
        public Cubemap cubemap;
        [Tooltip("Master Shader cubemap mode.")]
        public CubemapMode cubemapMode;
        public float reflectionStrength = 1f;
        public float lightingContribution = 1f;
        public float colorContribution = 1f;

        private Renderer renderer;
        private MaterialPropertyBlock block;

        void Start()
        {
            block = new MaterialPropertyBlock();
            renderer = GetComponent<Renderer>();
            renderer.GetPropertyBlock(block);
            var mats = renderer.materials;

            if (!applyToAllMaterials)
            {
                block.SetTexture("_CubeTex", cubemap);
                block.SetFloat("_ReflectionStrength", reflectionStrength);
                block.SetFloat("_LightingContribution", lightingContribution);
                block.SetFloat("_ColorContribution", colorContribution);

                if (cubemapMode == CubemapMode.Replace)
                    block.SetFloat("_CubeMode", 0);
                else if (cubemapMode == CubemapMode.Add)
                    block.SetFloat("_CubeMode", 1);

                var materials = renderer.sharedMaterials;

                foreach (Material mat in materials)
                {
                    if (mat.name.Replace(" (Instance)", "") == material.name && mat.mainTexture == material.mainTexture)
                    {
                        var location = (materials.IndexOf(mat));
                        renderer.SetPropertyBlock(block, location);
                        renderer.materials[location].EnableKeyword("REFLECTION");
                    }
                }
            }
            else if (applyToAllMaterials && cubemap != null && renderer != null)
            {
                block.SetTexture("_CubeTex", cubemap);
                block.SetFloat("_ReflectionStrength", reflectionStrength);
                block.SetFloat("_LightingContribution", lightingContribution);
                block.SetFloat("_ColorContribution", colorContribution);

                if (cubemapMode == CubemapMode.Replace)
                    block.SetFloat("_CubeMode", 0);
                else if (cubemapMode == CubemapMode.Add)
                    block.SetFloat("_CubeMode", 1);

                renderer.SetPropertyBlock(block);

                foreach (Material mat in renderer.materials)
                    renderer.materials[renderer.materials.IndexOf(mat)].EnableKeyword("REFLECTION");
            }
            else
                return;
        }
    }
}
