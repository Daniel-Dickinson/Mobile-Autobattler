using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Shake")]
        [SerializeField] private float shakeSpeed = 16;
        [SerializeField] private float shakeMagnitude = 0.5f;

        //Shake
        private float shakeTime = 0f;
        private float shakeDuration = 0f;
        private float speedMultiplier = 1f;
        private float magnitudeMultiplier = 1f;

        //Falloff
        private Camera cam;
        private float falloff;
        private Vector3 startPosition;
        //private Quaternion startRotation;

        //Mono
        private void Awake()
        {
            cam = GetComponent<Camera>();
            startPosition = cam.transform.position;
            //startRotation = cam.transform.rotation;
        }
        private void LateUpdate()
        {
            //Camera Shake
            Vector3 shakeVector = UpdateCameraShake();
            cam.transform.position = startPosition + shakeVector;
        }

        //Shake
        public void Shake(float duration, float speed, float strength)
        {
            if (duration > shakeTime * falloff)
            {
                shakeTime = duration;
                shakeDuration = duration;
            }

            if (speed > speedMultiplier * falloff) speedMultiplier = speed;
            if (strength > magnitudeMultiplier * falloff) magnitudeMultiplier = strength;
        }
        private Vector3 UpdateCameraShake()
        {
            if (shakeTime > 0)
            {
                //Increment shakeTime
                shakeTime = Mathf.MoveTowards(shakeTime, 0, Time.deltaTime);

                //Calculate falloff value
                falloff = (shakeTime / shakeDuration);

                //Reduce values as we exit the shake
                float speedFalloff = shakeSpeed * speedMultiplier * falloff;
                float magnitudeFalloff = shakeMagnitude * magnitudeMultiplier * falloff;

                //Generate X and Y values from perlin noise
                float x = Mathf.PerlinNoise(0, 1.59f + (Time.time * speedFalloff));
                float y = Mathf.PerlinNoise(0, 9.0f + (Time.time * speedFalloff));

                //Return random offset
                return new Vector3(x * magnitudeFalloff, y * magnitudeFalloff, 0);
            }
            else return Vector3.zero;
        }
    }
}