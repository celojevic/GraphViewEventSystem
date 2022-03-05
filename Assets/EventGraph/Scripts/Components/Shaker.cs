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
        public float duration = -1f;

        private Vector2 _basePos;
        private float _endTime;

        private void Awake()
        {
            _basePos = transform.position;
        }

        private void Update()
        {
            if (duration > 0f)
            {
                if (Time.time >= _endTime)
                {
                    transform.position = _basePos;
                    Destroy(this);
                    return;
                }
            }

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

        public void Shake(float intensity = 1f, float duration = -1f,
            float decay=0f, bool destroyOnDecay = true)
        {
            // Stop shaking
            if (intensity == 0f)
            {
                Destroy(this);
                return;
            }

            if (duration > 0f)
            {
                _endTime = Time.time + duration;
            }

            this.duration = duration;
            this.intensity = intensity;
            this.decay = decay;
            this.destroyOnDecay = destroyOnDecay;
        }

    }

}
