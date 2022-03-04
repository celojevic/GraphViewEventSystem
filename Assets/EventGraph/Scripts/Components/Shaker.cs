using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventGraph.Components
{

    public class Shaker : MonoBehaviour
    {

        public bool shakeRotation = false;
        public float intensity = 1f;
        public float decay = 0f;
        public bool destroyOnDecay = true;

        // TODO decay/duration 

        private Vector2 _basePos;

        private void Awake()
        {
            _basePos = transform.position;
        }

        private void Update()
        {
            if (decay > 0f)
            {
                intensity -= decay;
                if (intensity<=0)
                {
                    intensity = 0;
                    if (destroyOnDecay)
                        Destroy(this);
                    return;
                }
            }

            transform.position = _basePos + Random.insideUnitCircle * intensity;
        }

        public void Shake(float intensity = 1f, 
            float decay=0f, bool destroyOnDecay = true)
        {
            // Stop shaking
            if (intensity == 0f)
            {
                Destroy(this);
                return;
            }

            this.intensity = intensity;
            this.decay = decay;
            this.destroyOnDecay = destroyOnDecay;
        }

    }

}
