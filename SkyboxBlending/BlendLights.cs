using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SagesUtilsRelease
{
    public class BlendLights : MonoBehaviour
    {
        public bool activateOnEnable;
        [Tooltip("Will inheriet the duration or speed of any instance of a SkyboxBlend that references this object in its 'Blend Lights' array upon SkyboxBlend.Activate();.")]
        public bool inheritSkyboxDuration;
        [Tooltip("The color you are trying to reach.")]
        public Color color;
        [Tooltip("The light intensity you are trying to reach.")]
        public float intensity;
        [Tooltip("The time in seconds it will take to reach 'Color' from your lights start color.")]
        public float duration;
        [Tooltip("Lights affected by this script when Activate(); is called. If this is left empty, it will automatically search for a light component on this game object.")]
        public Light[] lights;
        [Tooltip("Will disactivate events upon finishing BlendLights.Reverse(); if called.")]
        public bool revertEventsOnReverse;
        public UltrakillEvent onFinish;

        [HideInInspector]
        public float speedToLerp;

        private List<Color> originalColors;
        private List<Color> currentColors;
        private List<float> originalIntensity;
        private List<float> currentIntensity;

        private bool shifting;
        private int length = 0;
        private float t = 0f;
        private bool lightsCompleted;
        private bool reversing;

        void Start()
        {
            if (1 > lights.Length || lights.Length == -1)
            {
                var ls = new List<Light>();
                ls.Add(gameObject.GetComponent<Light>());
                lights = ls.ToArray();
            }

            originalColors = new List<Color>();
            currentColors = new List<Color>();
            originalIntensity = new List<float>();
            currentIntensity = new List<float>();

            foreach (Light light in lights)
            {
                originalColors.Add(light.color);
                originalIntensity.Add(light.intensity);
                currentColors.Add(light.color);
                currentIntensity.Add(light.intensity);
                length += 1;
            }

            if (activateOnEnable)
                Activate();
        }

        public void Activate()
        {
            t = 0f;
            lightsCompleted = false;
            reversing = false;
            shifting = true;
        }

        public void ActivateWithNewDuration(float dur)
        {
            duration = dur;
            t = 0f;
            lightsCompleted = false;
            reversing = false;
            shifting = true;
        }

        public void Reverse(float dur)
        {
            duration = dur;
            t = 0f;
            lightsCompleted = false;
            reversing = true;
            shifting = true;
        }

        public void ChangeDuration(float dur)
        {
            duration = dur;
        }

        public void ChangeColor(Color col)
        {
            color = col;
        }

        public void ChangeIntensity(float inte)
        {
            intensity = inte;
        }

        void Update()
        {
            if (shifting && !reversing && Time.deltaTime != 0f)
            {
                t += Time.deltaTime / duration;

                for (int i = 0; i < length; i++)
                {
                    currentColors[i] = Color.Lerp(originalColors[i], color, t);
                    currentIntensity[i] = Mathf.Lerp(originalIntensity[i], intensity, t);

                    lights[i].color = currentColors.ToArray()[i];
                    lights[i].intensity = currentIntensity.ToArray()[i];

                    if (intensity > originalIntensity[i])
                    {
                        if (currentIntensity[i] > intensity)
                            currentIntensity[i] = intensity;
                    }
                    else if (originalIntensity[i] > intensity)
                    {
                        if (intensity > currentIntensity[i])
                            currentIntensity[i] = intensity;
                    }
                }

                lightsCompleted = true;

                for (int i = 0; i < length;i++)
                {
                    if (currentColors[i] != color && currentIntensity[i] != intensity)
                        lightsCompleted = false;
                }

                if (lightsCompleted)
                    Complete();
            }

            if (shifting && reversing && Time.deltaTime != 0f)
            {
                t += Time.deltaTime / duration;

                for (int i = 0; i < length; i++)
                {
                    currentColors[i] = Color.Lerp(color, originalColors[i], t);
                    currentIntensity[i] = Mathf.Lerp(intensity, originalIntensity[i], t);

                    lights[i].color = currentColors.ToArray()[i];
                    lights[i].intensity = currentIntensity.ToArray()[i];

                    if (intensity > originalIntensity[i])
                    {
                        if (originalIntensity[i] > currentIntensity[i])
                            currentIntensity[i] = originalIntensity[i];
                    }
                    else if (originalIntensity[i] > intensity)
                    {
                        if (currentIntensity[i] > originalIntensity[i])
                            currentIntensity[i] = originalIntensity[i];
                    }
                }

                lightsCompleted = true;

                for (int i = 0; i < length; i++)
                {
                    if (currentColors[i] != originalColors[i] && currentIntensity[i] != originalIntensity[i])
                        lightsCompleted = false;
                }

                if (lightsCompleted)
                    ReverseComplete();
            }

        }

        void Complete()
        {
            shifting = false;
            lightsCompleted = false;
            t = 0f;

            foreach (Light light in lights)
            {
                light.color = color;
                light.intensity = intensity;
            }

            onFinish.Invoke();
        }

        void ReverseComplete()
        {
            shifting = false;
            lightsCompleted = false;
            reversing = false;
            t = 0f;

            for (int i = 0; i < length; i++)
            {
                lights[i].color = originalColors[i];
                lights[i].intensity = originalIntensity[i];
            }

            if (revertEventsOnReverse)
                onFinish.Revert();
        }
    }
}
