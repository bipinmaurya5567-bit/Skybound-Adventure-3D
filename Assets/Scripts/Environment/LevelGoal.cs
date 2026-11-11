using UnityEngine;

namespace SkyboundAdventure
{
    public class LevelGoal : MonoBehaviour
    {
        [Header("Visual Effects")]
        [SerializeField] private Transform rotatingRing;
        [SerializeField] private float rotationSpeed = 60f;
        [SerializeField] private ParticleSystem victoryParticles;

        private bool isCompleted;

        private void Update()
        {
            if (rotatingRing != null)
            {
                rotatingRing.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
                rotatingRing.Rotate(Vector3.forward, rotationSpeed * 0.4f * Time.deltaTime, Space.Self);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCompleted) return;

            if (other.GetComponent<PlayerController>() != null || other.CompareTag("Player"))
            {
                TriggerGoal();
            }
        }

        public void TriggerGoal()
        {
            if (isCompleted) return;
            isCompleted = true;

            if (victoryParticles != null)
            {
                victoryParticles.Play();
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayVictory();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnLevelComplete();
            }
        }

        public void ResetGoal()
        {
            isCompleted = false;
        }
    }
}
