using UnityEngine;

namespace SkyboundAdventure
{
    public class RotatingSweeper : MonoBehaviour
    {
        [Header("Sweeper Configuration")]
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] private bool rotateClockwise = true;

        private Transform armTransform;

        private void Start()
        {
            armTransform = transform.Find("SweeperArm") ?? transform;
        }

        private void Update()
        {
            float direction = rotateClockwise ? 1f : -1f;
            transform.Rotate(Vector3.up, direction * rotationSpeed * Time.deltaTime, Space.World);
        }

        public void Configure(float speed, bool clockwise = true)
        {
            rotationSpeed = speed;
            rotateClockwise = clockwise;
        }
    }
}
