using System.Collections;
using UnityEngine;

namespace SkyboundAdventure
{
    public class BouncePad : MonoBehaviour
    {
        [Header("Bounce Settings")]
        [SerializeField] private float bounceHeight = 5.2f;

        private Transform padVisual;
        private Vector3 initialScale;
        private bool isBouncing = false;

        private void Awake()
        {
            padVisual = transform.Find("PadMound") ?? transform;
            initialScale = padVisual.localScale;
        }

        public void Configure(float height)
        {
            bounceHeight = height;
        }

        public void OnPlayerBounce(PlayerController player)
        {
            if (player == null || isBouncing) return;
            StartCoroutine(BounceSequence(player));
        }

        private void OnTriggerEnter(Collider other)
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                OnPlayerBounce(pc);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var pc = collision.collider.GetComponent<PlayerController>();
            if (pc != null)
            {
                OnPlayerBounce(pc);
            }
        }

        private IEnumerator BounceSequence(PlayerController player)
        {
            isBouncing = true;

            // Compress
            padVisual.localScale = new Vector3(initialScale.x * 1.3f, initialScale.y * 0.4f, initialScale.z * 1.3f);

            yield return new WaitForSeconds(0.04f);

            // Launch player
            player.Bounce(bounceHeight);

            // Rebound stretch
            padVisual.localScale = new Vector3(initialScale.x * 0.85f, initialScale.y * 1.4f, initialScale.z * 0.85f);

            float t = 0f;
            while (t < 0.25f)
            {
                t += Time.deltaTime;
                padVisual.localScale = Vector3.Lerp(padVisual.localScale, initialScale, t * 8f);
                yield return null;
            }

            padVisual.localScale = initialScale;
            isBouncing = false;
        }
    }
}
