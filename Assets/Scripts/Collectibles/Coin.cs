using UnityEngine;

namespace SkyboundAdventure
{
    [RequireComponent(typeof(Collider))]
    public class Coin : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private float rotationSpeed = 135f;
        [SerializeField] private float bobHeight = 0.12f;
        [SerializeField] private float bobSpeed = 3.0f;

        [Header("Visual References")]
        [SerializeField] private ParticleSystem ambientSparkles;

        private Vector3 startPosition;
        private Quaternion startRotation;
        private float randomOffset;
        private bool isCollected;

        private Renderer[] allRenderers;
        private Collider[] allColliders;

        private static ParticleSystem sharedSparkleSystem;

        public bool IsCollected => isCollected;

        private void Awake()
        {
            CacheComponents();
            startPosition = transform.position;
            startRotation = transform.rotation;
            randomOffset = Random.Range(0f, Mathf.PI * 2f);
        }

        private void CacheComponents()
        {
            if (allColliders == null || allColliders.Length == 0)
            {
                allColliders = GetComponentsInChildren<Collider>(true);
            }
            if (allRenderers == null || allRenderers.Length == 0)
            {
                allRenderers = GetComponentsInChildren<Renderer>(true);
            }
            if (ambientSparkles == null)
            {
                ambientSparkles = GetComponentInChildren<ParticleSystem>();
            }

            for (int i = 0; i < allColliders.Length; i++)
            {
                if (allColliders[i] != null)
                {
                    allColliders[i].isTrigger = true;
                }
            }
        }

        private void Update()
        {
            if (isCollected) return;

            // Continuous smooth rotation around Y axis
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            // Gentle floating vertical bobbing
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed + randomOffset) * bobHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return;

            if (other.GetComponent<PlayerController>() != null || other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)
            {
                Collect();
            }
        }

        public void Collect()
        {
            if (isCollected) return;
            isCollected = true;

            CacheComponents();

            // Deactivate all coin colliders
            for (int i = 0; i < allColliders.Length; i++)
            {
                if (allColliders[i] != null) allColliders[i].enabled = false;
            }

            // Deactivate all coin renderers (body, rim, 3D dollar signs)
            for (int i = 0; i < allRenderers.Length; i++)
            {
                if (allRenderers[i] != null) allRenderers[i].enabled = false;
            }

            // Stop ambient sparkles
            if (ambientSparkles != null)
            {
                ambientSparkles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            // Play audio
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCoin();
            }

            // Centralized registration in GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoin(this);
            }

            // Play collection sparkle burst
            PlaySparkleEffect(transform.position);
        }

        public void ResetCoin()
        {
            isCollected = false;
            CacheComponents();

            // Restore original position and rotation
            transform.position = startPosition;
            transform.rotation = startRotation;

            // Reactivate all colliders
            for (int i = 0; i < allColliders.Length; i++)
            {
                if (allColliders[i] != null) allColliders[i].enabled = true;
            }

            // Reactivate all renderers
            for (int i = 0; i < allRenderers.Length; i++)
            {
                if (allRenderers[i] != null) allRenderers[i].enabled = true;
            }

            // Reactivate ambient sparkles
            if (ambientSparkles != null)
            {
                ambientSparkles.Play();
            }
        }

        private void PlaySparkleEffect(Vector3 pos)
        {
            if (sharedSparkleSystem == null)
            {
                GameObject fxGo = new GameObject("SharedCoinSparkleEmitter");
                sharedSparkleSystem = fxGo.AddComponent<ParticleSystem>();
                sharedSparkleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                var psRenderer = sharedSparkleSystem.GetComponent<ParticleSystemRenderer>();
                if (psRenderer != null)
                {
                    Shader pShader = Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? Shader.Find("Particles/Standard Unlit");
                    if (pShader != null)
                    {
                        Material pMat = new Material(pShader);
                        pMat.name = "M_CoinSparkleParticle";
                        psRenderer.sharedMaterial = pMat;
                    }
                }

                var main = sharedSparkleSystem.main;
                main.playOnAwake = false;
                main.duration = 0.45f;
                main.loop = false;
                main.startLifetime = 0.4f;
                main.startSpeed = 3.2f;
                main.startSize = 0.18f;
                main.startColor = new Color(1f, 0.88f, 0.15f, 1f);

                var emission = sharedSparkleSystem.emission;
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 16) });

                var shape = sharedSparkleSystem.shape;
                shape.shapeType = ParticleSystemShapeType.Sphere;
                shape.radius = 0.25f;

                var colorLife = sharedSparkleSystem.colorOverLifetime;
                colorLife.enabled = true;
                Gradient grad = new Gradient();
                grad.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.95f, 0.3f), 0f), new GradientColorKey(new Color(1f, 0.65f, 0.1f), 1f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
                );
                colorLife.color = grad;
            }

            sharedSparkleSystem.transform.position = pos;
            sharedSparkleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            sharedSparkleSystem.Play();
        }
    }
}
