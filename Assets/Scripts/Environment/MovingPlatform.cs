using UnityEngine;

namespace SkyboundAdventure
{
    public class MovingPlatform : MonoBehaviour
    {
        [Header("Movement Path")]
        [SerializeField] private Vector3 moveDelta = new Vector3(0f, 0f, 4.5f);
        [SerializeField] private float travelDuration = 3.2f;
        [SerializeField] private float dockPauseDuration = 0.8f;

        private Vector3 startPos;
        private Vector3 targetPos;
        private CharacterController playerOnPlatform;

        public void Configure(Vector3 delta, float travelTime = 3.0f, float pauseTime = 0.8f)
        {
            moveDelta = delta;
            travelDuration = Mathf.Max(0.5f, travelTime);
            dockPauseDuration = Mathf.Max(0f, pauseTime);
        }

        private void Start()
        {
            startPos = transform.position;
            targetPos = startPos + moveDelta;
        }

        private void Update()
        {
            float cycleDuration = (travelDuration + dockPauseDuration) * 2f;
            float t = Time.time % cycleDuration;
            float smoothT = 0f;

            if (t < dockPauseDuration)
            {
                // 1. Paused at start dock
                smoothT = 0f;
            }
            else if (t < dockPauseDuration + travelDuration)
            {
                // 2. Smoothly cruising from start to target
                float u = (t - dockPauseDuration) / travelDuration;
                smoothT = Mathf.SmoothStep(0f, 1f, u);
            }
            else if (t < (dockPauseDuration * 2f) + travelDuration)
            {
                // 3. Paused at target dock
                smoothT = 1f;
            }
            else
            {
                // 4. Smoothly cruising back from target to start
                float u = (t - (dockPauseDuration * 2f) - travelDuration) / travelDuration;
                smoothT = Mathf.SmoothStep(1f, 0f, u);
            }

            Vector3 newPos = Vector3.Lerp(startPos, targetPos, smoothT);
            Vector3 delta = newPos - transform.position;
            transform.position = newPos;

            // Carry player along smoothly without sliding
            if (playerOnPlatform != null && playerOnPlatform.enabled)
            {
                playerOnPlatform.Move(delta);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                playerOnPlatform = other.GetComponent<CharacterController>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                if (playerOnPlatform == other.GetComponent<CharacterController>())
                {
                    playerOnPlatform = null;
                }
            }
        }
    }
}
