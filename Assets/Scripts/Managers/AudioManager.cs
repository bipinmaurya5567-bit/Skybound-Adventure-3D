using UnityEngine;

namespace SkyboundAdventure
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        private AudioClip jumpClip;
        private AudioClip coinClip;
        private AudioClip deathClip;
        private AudioClip victoryClip;
        private AudioClip clickClip;
        private AudioClip bounceClip;
        private AudioClip disappearWarningClip;
        private AudioClip checkpointClip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            EnsureAudioSources();
            GenerateProceduralClips();
            StartBackgroundMusic();
        }

        private void EnsureAudioSources()
        {
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.spatialBlend = 0f;
            }

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.playOnAwake = false;
                musicSource.loop = true;
                musicSource.spatialBlend = 0f;
                musicSource.volume = 0.25f;
            }
        }

        private void GenerateProceduralClips()
        {
            jumpClip = CreateJumpClip();
            coinClip = CreateCoinClip();
            deathClip = CreateDeathClip();
            victoryClip = CreateVictoryClip();
            clickClip = CreateClickClip();
            bounceClip = CreateBounceClip();
            disappearWarningClip = CreateDisappearWarningClip();
            checkpointClip = CreateCheckpointClip();
        }

        private void StartBackgroundMusic()
        {
            if (musicSource == null) return;
            AudioClip musicClip = CreateAmbientMusicClip();
            if (musicClip != null)
            {
                musicSource.clip = musicClip;
                musicSource.Play();
            }
        }

        public void PlayJump()
        {
            PlaySFX(jumpClip, 0.6f);
        }

        public void PlayBounce()
        {
            PlaySFX(bounceClip, 0.75f);
        }

        public void PlayDisappearWarning()
        {
            PlaySFX(disappearWarningClip, 0.55f);
        }

        public void PlayCheckpoint()
        {
            PlaySFX(checkpointClip, 0.7f);
        }

        public void PlayCoin()
        {
            PlaySFX(coinClip, 0.7f);
        }

        public void PlayDeath()
        {
            PlaySFX(deathClip, 0.8f);
        }

        public void PlayVictory()
        {
            PlaySFX(victoryClip, 0.85f);
        }

        public void PlayClick()
        {
            PlaySFX(clickClip, 0.5f);
        }

        private void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip, volume);
            }
        }

        #region Procedural Audio Generation

        private AudioClip CreateJumpClip()
        {
            int sampleRate = 44100;
            float duration = 0.22f;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float freq = Mathf.Lerp(260f, 620f, Mathf.Sqrt(t));
                float wave = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)sampleRate));
                float env = 1f - t;
                data[i] = wave * env * 0.45f;
            }

            AudioClip clip = AudioClip.Create("SFX_Jump", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateCoinClip()
        {
            int sampleRate = 44100;
            float duration = 0.28f;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            int note1Samples = samples / 2;
            for (int i = 0; i < samples; i++)
            {
                float freq = (i < note1Samples) ? 987.77f : 1318.51f; // B5 then E6
                float noteT = (i < note1Samples) ? (float)i / note1Samples : (float)(i - note1Samples) / (samples - note1Samples);
                float wave = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)sampleRate));
                wave += 0.25f * Mathf.Sin(4f * Mathf.PI * freq * (i / (float)sampleRate));
                float env = Mathf.Exp(-noteT * 4.5f);
                data[i] = wave * env * 0.4f;
            }

            AudioClip clip = AudioClip.Create("SFX_Coin", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateDeathClip()
        {
            int sampleRate = 44100;
            float duration = 0.55f;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float freq = Mathf.Lerp(380f, 90f, t * t);
                float wave = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)sampleRate));
                wave += 0.3f * Mathf.Sin(2f * Mathf.PI * (freq * 0.5f) * (i / (float)sampleRate));
                float env = Mathf.Cos(t * Mathf.PI * 0.5f);
                data[i] = wave * env * 0.5f;
            }

            AudioClip clip = AudioClip.Create("SFX_Death", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateVictoryClip()
        {
            int sampleRate = 44100;
            float duration = 1.1f;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            float[] notes = new float[] { 523.25f, 659.25f, 783.99f, 1046.50f }; // C5, E5, G5, C6
            int noteLen = samples / notes.Length;

            for (int i = 0; i < samples; i++)
            {
                int noteIdx = Mathf.Clamp(i / noteLen, 0, notes.Length - 1);
                float freq = notes[noteIdx];
                int localI = i - noteIdx * noteLen;
                float localT = (float)localI / noteLen;

                float wave = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)sampleRate));
                wave += 0.3f * Mathf.Sin(4f * Mathf.PI * freq * (i / (float)sampleRate));
                float env = Mathf.Exp(-localT * 2.5f);

                data[i] = wave * env * 0.45f;
            }

            AudioClip clip = AudioClip.Create("SFX_Victory", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateClickClip()
        {
            int sampleRate = 44100;
            float duration = 0.05f;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float wave = Mathf.Sin(2f * Mathf.PI * 850f * (i / (float)sampleRate));
                float env = 1f - t;
                data[i] = wave * env * 0.3f;
            }

            AudioClip clip = AudioClip.Create("SFX_Click", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateBounceClip()
        {
            int sampleRate = 44100;
            float duration = 0.35f;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float freq = Mathf.Lerp(160f, 680f, Mathf.Pow(t, 0.6f));
                float wave = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)sampleRate));
                wave += 0.2f * Mathf.Sin(4f * Mathf.PI * freq * (i / (float)sampleRate));
                float env = Mathf.Sin(t * Mathf.PI);
                data[i] = wave * env * 0.5f;
            }

            AudioClip clip = AudioClip.Create("SFX_Bounce", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateDisappearWarningClip()
        {
            int sampleRate = 44100;
            float duration = 0.4f;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float stutter = Mathf.Sin(t * 50f * Mathf.PI);
                float noise = Random.Range(-0.3f, 0.3f);
                float wave = Mathf.Sin(2f * Mathf.PI * 320f * (i / (float)sampleRate)) * (stutter > 0f ? 1f : 0f);
                float env = 1f - t;
                data[i] = (wave * 0.7f + noise * 0.3f) * env * 0.35f;
            }

            AudioClip clip = AudioClip.Create("SFX_DisappearWarning", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateCheckpointClip()
        {
            int sampleRate = 44100;
            float duration = 0.65f;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            float[] notes = new float[] { 440f, 554.37f, 659.25f, 880f }; // A4, C#5, E5, A5
            int noteLen = samples / notes.Length;

            for (int i = 0; i < samples; i++)
            {
                int noteIdx = Mathf.Clamp(i / noteLen, 0, notes.Length - 1);
                float freq = notes[noteIdx];
                int localI = i - noteIdx * noteLen;
                float localT = (float)localI / noteLen;

                float wave = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)sampleRate));
                wave += 0.2f * Mathf.Sin(4f * Mathf.PI * freq * (i / (float)sampleRate));
                float env = Mathf.Exp(-localT * 3.5f);

                data[i] = wave * env * 0.4f;
            }

            AudioClip clip = AudioClip.Create("SFX_Checkpoint", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateAmbientMusicClip()
        {
            int sampleRate = 22050;
            float duration = 8.0f; // 8-second calm loop
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            // Peaceful ethereal chord progression (Fmaj7 -> G -> Am -> Em)
            float[] baseFreqs = new float[] { 174.61f, 196.00f, 220.00f, 164.81f };
            float sectionLen = duration / baseFreqs.Length;

            for (int i = 0; i < samples; i++)
            {
                float timeSec = (float)i / sampleRate;
                int section = Mathf.Clamp((int)(timeSec / sectionLen), 0, baseFreqs.Length - 1);
                float baseFreq = baseFreqs[section];

                float wave = 0f;
                wave += Mathf.Sin(2f * Mathf.PI * baseFreq * timeSec) * 0.15f;
                wave += Mathf.Sin(2f * Mathf.PI * (baseFreq * 1.5f) * timeSec) * 0.10f; // 5th
                wave += Mathf.Sin(2f * Mathf.PI * (baseFreq * 2.0f) * timeSec) * 0.06f; // octave
                wave += Mathf.Sin(2f * Mathf.PI * (baseFreq * 1.25f) * timeSec) * 0.08f; // major 3rd

                // Smooth loop fade at ends
                float edgeFade = 1f;
                if (i < 2000) edgeFade = (float)i / 2000f;
                else if (i > samples - 2000) edgeFade = (float)(samples - i) / 2000f;

                data[i] = wave * edgeFade * 0.25f;
            }

            AudioClip clip = AudioClip.Create("BGM_SkyboundAmbient", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        #endregion
    }
}
