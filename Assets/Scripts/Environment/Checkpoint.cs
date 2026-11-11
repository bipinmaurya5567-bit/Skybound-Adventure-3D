using UnityEngine;

namespace SkyboundAdventure
{
    public class Checkpoint : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private Renderer beaconRenderer;
        [SerializeField] private Color activeColor = new Color(0.2f, 0.95f, 0.4f);
        [SerializeField] private Color inactiveColor = new Color(0.95f, 0.55f, 0.15f);

        private bool isActivated;

        private void Start()
        {
            UpdateVisual(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isActivated) return;

            if (other.GetComponent<PlayerController>() != null || other.CompareTag("Player"))
            {
                ActivateCheckpoint();
            }
        }

        public void Configure(Renderer renderer)
        {
            beaconRenderer = renderer;
            UpdateVisual(isActivated);
        }

        public void ActivateCheckpoint()
        {
            if (isActivated) return;
            isActivated = true;
            UpdateVisual(true);

            if (GameManager.Instance != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
                GameManager.Instance.SetCheckpoint(spawnPos, transform.rotation);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCheckpoint();
            }
        }

        private MaterialPropertyBlock propBlock;

        private void UpdateVisual(bool active)
        {
            if (beaconRenderer != null)
            {
                if (propBlock == null) propBlock = new MaterialPropertyBlock();
                beaconRenderer.GetPropertyBlock(propBlock);

                Color col = active ? activeColor : inactiveColor;
                propBlock.SetColor("_BaseColor", col);
                propBlock.SetColor("_Color", col);
                propBlock.SetColor("_EmissionColor", col * 0.75f);
                beaconRenderer.SetPropertyBlock(propBlock);
            }
        }

        public void ResetCheckpoint()
        {
            isActivated = false;
            UpdateVisual(false);
        }
    }
}
