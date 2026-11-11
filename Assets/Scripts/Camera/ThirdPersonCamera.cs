using UnityEngine;

namespace SkyboundAdventure
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Target & Fixed Framing")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 4.0f, -6.5f);
        [SerializeField] private float fixedPitch = 22f;
        [SerializeField] private float followSpeed = 8.0f;
        [SerializeField] private float verticalFollowSpeed = 5.0f; // Soft damping to prevent jump bouncing

        private float currentY;

        public Transform Target => target;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                currentY = target.position.y;
            }
        }

        private void Start()
        {
            if (target == null)
            {
                var player = FindFirstObjectByType<PlayerController>();
                if (player != null) SetTarget(player.transform);
            }

            if (target != null)
            {
                currentY = target.position.y;
                transform.position = new Vector3(target.position.x + offset.x, currentY + offset.y, target.position.z + offset.z);
                transform.rotation = Quaternion.Euler(fixedPitch, 0f, 0f);
            }

            // Ensure mouse cursor is visible and free (no cursor locking)
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Damped vertical follow to eliminate jump jitter
            currentY = Mathf.Lerp(currentY, target.position.y, verticalFollowSpeed * Time.deltaTime);

            Vector3 desiredPosition = new Vector3(
                target.position.x + offset.x,
                currentY + offset.y,
                target.position.z + offset.z
            );

            // Smooth follow along horizontal and vertical
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // Fixed camera orientation (no mouse rotation)
            transform.rotation = Quaternion.Euler(fixedPitch, 0f, 0f);
        }

        // Stub kept for compatibility with GameManager
        public void SetCursorLocked(bool locked)
        {
            // Mouse camera rotation is permanently disabled; cursor remains free for UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
