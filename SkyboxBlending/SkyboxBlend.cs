using System;
using System.Collections.Generic;
using System.Text;

namespace SagesUtilsRelease
{
    using BepInEx.Configuration;
    using UnityEngine;
    using UnityEngine.Rendering;

    namespace SagesUtilsRelease
    {
        public class SkyboxBlend : MonoBehaviour
        {
            public bool activateOnEnable;
            [Tooltip("Lerp will move from the skyboxes current blend to the goal value over X seconds. Constant value will add X% (out of 100%) to the blend value every second.")]
            public enum lerpType
            {
                ConstantValue,
                Lerp
            }
            [Tooltip("Lerp will move from the skyboxes current blend to the goal value over X seconds. Constant value will add X% (out of 100%) to the blend value every second.")]
            public lerpType blendType;
            [Tooltip("The blend value you are attempting to reach. Maximum being one and minimum being 0.")]
            public float goalValue;
            [Tooltip("Time in seconds for the value to shift from A to B for lerp. Percentage out of 100 added or subtracted to or from the blend value every second for constant speed.")]
            public float durationOrSpeed;
            [Tooltip("Light shift controllers that will be activated upon calling SkyboxBlend.Activate();")]
            public BlendLights[] blendLights;

            public UltrakillEvent onFinish;

            [HideInInspector]
            public float startValue;

            private bool shifting = false;
            private float lerpCurrent;
            private float t = 0f;

            void Start()
            {
                if (activateOnEnable)
                    Activate();
            }

            public void Activate()
            {
                startValue = RenderSettings.skybox.GetFloat("_Blend");

                foreach (BlendLights light in blendLights)
                {
                    if (light.inheritSkyboxDuration)
                    {
                        light.duration = durationOrSpeed;

                        if (blendType == lerpType.ConstantValue)
                        {
                            var val = 0f;

                            if (startValue > goalValue)
                                val = (startValue - goalValue);
                            else
                                val = (goalValue - startValue);

                            light.duration = (val / (durationOrSpeed / 100));
                        }
                    }

                    light.Activate();
                }

                t = 0f;
                shifting = true;
            }

            public void ChangeDuration(float dur)
            {
                durationOrSpeed = dur;
            }

            public void ChangeBlendValue(float blend)
            {
                startValue = RenderSettings.skybox.GetFloat("_Blend");
                goalValue = blend;
            }

            void Update()
            {
                if (shifting)
                {
                    if (blendType == lerpType.ConstantValue)
                    {
                        if (startValue < goalValue)
                            lerpCurrent += (durationOrSpeed * Time.deltaTime / 100);
                        else
                            lerpCurrent -= (durationOrSpeed * Time.deltaTime / 100);
                    }
                    else
                    {
                        t += Time.deltaTime / durationOrSpeed;

                        lerpCurrent = Mathf.Lerp(startValue, goalValue, t);
                    }

                    RenderSettings.skybox.SetFloat("_Blend", lerpCurrent);

                    if (lerpCurrent > goalValue && goalValue > startValue)
                    {
                        lerpCurrent = goalValue;
                    }
                    else if (goalValue > lerpCurrent && startValue > goalValue)
                    {
                        lerpCurrent = goalValue;
                    }

                    if (lerpCurrent == goalValue)
                    {
                        onFinish.Invoke();
                        startValue = lerpCurrent;
                        shifting = false;
                    }
                }
            }
        }
    }
}
