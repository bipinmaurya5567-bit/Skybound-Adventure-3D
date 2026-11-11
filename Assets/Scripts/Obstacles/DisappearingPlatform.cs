using System.Collections;
using UnityEngine;

namespace SkyboundAdventure
{
    [RequireComponent(typeof(BoxCollider))]
    public class DisappearingPlatform : MonoBehaviour
    {
        [Header("Timing")]
        [SerializeField] private float shakeDuration = 1.0f;
        [SerializeField] private float disappearDuration = 2.5f;

        [Header("Visuals")]
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material warningMaterial;

        private BoxCollider boxCollider;
        private Renderer meshRenderer;
        private Vector3 initialLocalPos;
        private bool isTriggered = false;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            meshRenderer = GetComponent<Renderer>();
            initialLocalPos = transform.localPosition;
            if (normalMaterial == null && meshRenderer != null)
            {
                normalMaterial = meshRenderer.sharedMaterial;
            }
        }

        public void SetupMaterials(Material normal, Material warning)
        {
            normalMaterial = normal;
            warningMaterial = warning;
            if (meshRenderer != null && normalMaterial != null)
            {
                meshRenderer.sharedMaterial = normalMaterial;
            }
        }

        public void OnPlayerStepped()
        {
            if (isTriggered) return;
            StartCoroutine(CrumbleSequence());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>() != null || other.CompareTag("Player"))
            {
                OnPlayerStepped();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.GetComponent<PlayerController>() != null || collision.collider.CompareTag("Player"))
            {
                OnPlayerStepped();
            }
        }

        private IEnumerator CrumbleSequence()
        {
            isTriggered = true;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDisappearWarning();
            }

            if (meshRenderer != null && warningMaterial != null)
            {
                meshRenderer.sharedMaterial = warningMaterial;
            }

            // Warning shake
            float elapsed = 0f;
            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                float shakeX = Mathf.Sin(elapsed * 40f) * 0.08f;
                float shakeZ = Mathf.Cos(elapsed * 40f) * 0.08f;
                transform.localPosition = initialLocalPos + new Vector3(shakeX, 0f, shakeZ);
                yield return null;
            }

            transform.localPosition = initialLocalPos;

            // Disappear
            if (boxCollider != null) boxCollider.enabled = false;
            if (meshRenderer != null) meshRenderer.enabled = false;

            yield return new WaitForSeconds(disappearDuration);

            // Reappear
            if (meshRenderer != null && normalMaterial != null)
            {
                meshRenderer.sharedMaterial = normalMaterial;
            }
            if (meshRenderer != null) meshRenderer.enabled = true;
            if (boxCollider != null) boxCollider.enabled = true;

            isTriggered = false;
        }
    }
}
