using UnityEngine;

namespace SkyboundAdventure
{
    public class DeathZone : MonoBehaviour
    {
        [SerializeField] private float fallThresholdY = -12.0f;
        private Transform playerTransform;

        private void Start()
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null) playerTransform = player.transform;
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                var player = FindFirstObjectByType<PlayerController>();
                if (player != null) playerTransform = player.transform;
                return;
            }

            if (playerTransform.position.y < fallThresholdY)
            {
                if (GameManager.Instance != null && GameManager.Instance.IsPlaying)
                {
                    GameManager.Instance.OnPlayerDeath();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>() != null || other.CompareTag("Player"))
            {
                if (GameManager.Instance != null && GameManager.Instance.IsPlaying)
                {
                    GameManager.Instance.OnPlayerDeath();
                }
            }
        }
    }
}
