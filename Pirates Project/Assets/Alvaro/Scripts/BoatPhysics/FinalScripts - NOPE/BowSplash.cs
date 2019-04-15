using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NopeScript
{
    public class BowSplash : MonoBehaviour
    {
        public Transform sphere;
        public Transform splashTop;
        public Transform splashBottom;
        public ParticleSystem splashParticleSystem;

        private Vector3 lastPos;

        void Update()
        {
            Debug.DrawLine(splashBottom.position, splashTop.position, Color.blue);

            float bottomDistToWater = WaterController.current.DistanceToWater(splashBottom.position, Time.time);

            float topDistToWater = WaterController.current.DistanceToWater(splashTop.position, Time.time);

            if(topDistToWater > 0f && bottomDistToWater < 0f)
            {
                Vector3 H = splashTop.position;
                Vector3 M = splashBottom.position;

                float h_M = bottomDistToWater;
                float h_H = topDistToWater;

                Vector3 MH = H - M;

                float t_M = -h_M / (h_H - h_M);

                Vector3 MI_M = t_M * MH;

                Vector3 I_M = MI_M + M;

                sphere.position = I_M;

                if(I_M.y < lastPos.y)
                {
                    splashParticleSystem.transform.LookAt(splashTop.position);

                    if(!splashParticleSystem.isPlaying)
                    {
                        splashParticleSystem.Play();
                    }
                }
                else
                {
                    splashParticleSystem.Stop();
                }

                lastPos = I_M;
            }
            else
            {
                splashParticleSystem.Stop();
            }
        }
    }
}
