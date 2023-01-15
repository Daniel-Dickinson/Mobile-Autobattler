using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Cameras
{
    public class CameraShaker : MonoBehaviour
    {
        public bool playOnAwake = false;

        public float duration = 2.0f;
        public float strength = 2.0f;
        public float speed = 2.0f;

        private CameraController[] controllers;

        //Mono
        private void OnEnable()
        {
            if (playOnAwake) Trigger();
        }

        //Core
        public void Trigger(float multiplier = 1.0f)
        {
            //Grab all camera controllers
            if (controllers == null || controllers.Length == 0)
            {
                controllers = FindObjectsOfType<CameraController>();
            }

            //Trigger shake on each camera in range
            foreach (CameraController controller in controllers)
            {
                float localDuration = duration * multiplier;
                float localStrength = strength * multiplier;
                float localSpeed = speed * multiplier;

                Debug.Log("Shake!");

                controller.Shake(localDuration, localSpeed, localStrength);
            }
        }
    }
}