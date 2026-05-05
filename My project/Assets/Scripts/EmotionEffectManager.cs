using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class EmotionEffectManager : MonoBehaviour
{
    [Header("配置文件")]
    public EmotionEffectConfig effectConfig;

    [Header("引用")]
    public Camera mainCamera;
    public Light directionalLight;
    public Transform particleSpawnPoint;
    public Volume postProcessVolume;

    [Header("调试")]
    public string currentEmotion = "normal";

    private ScreenFlash screenFlash;
    private AudioSource audioSource;
    private Dictionary<string, EmotionEffectConfig.EmotionSettings> emotionSettingsMap;
    
    private Material targetSkybox;
    private Color targetAmbientColor;
    private Color targetLightColor;
    private float targetLightIntensity;
    private VolumeProfile targetVolumeProfile;
    
    private Material currentSkybox;
    private Color currentAmbientColor;
    private Color currentLightColor;
    private float currentLightIntensity;
    
    private float transitionTimer;
    private float transitionDuration;
    private bool isTransitioning;
    
    private GameObject currentAmbientParticles;

    private void Awake()
    {
        InitializeComponents();
        InitializeEmotionMap();
    }

    private void Start()
    {
        if (effectConfig != null)
        {
            ApplyEmotionEffect("normal", immediate: true);
        }
    }

    private void InitializeComponents()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            screenFlash = mainCamera.GetComponent<ScreenFlash>();
            if (screenFlash == null)
            {
                screenFlash = mainCamera.gameObject.AddComponent<ScreenFlash>();
            }
        }

        if (directionalLight == null)
        {
            directionalLight = FindObjectOfType<Light>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (particleSpawnPoint == null)
        {
            particleSpawnPoint = transform;
        }
    }

    private void InitializeEmotionMap()
    {
        emotionSettingsMap = new Dictionary<string, EmotionEffectConfig.EmotionSettings>();
        if (effectConfig != null)
        {
            emotionSettingsMap["happy"] = effectConfig.happySettings;
            emotionSettingsMap["sad"] = effectConfig.sadSettings;
            emotionSettingsMap["normal"] = effectConfig.normalSettings;
        }
    }

    public void TransitionToEmotion(string newEmotion)
    {
        if (newEmotion == currentEmotion) return;

        string oldEmotion = currentEmotion;
        currentEmotion = newEmotion;

        PlayShortWindowEffects(oldEmotion, newEmotion);
        ApplyLongWindowEnvironment(newEmotion, immediate: false);
    }

    private void PlayShortWindowEffects(string fromEmotion, string toEmotion)
    {
        if (effectConfig == null || !emotionSettingsMap.TryGetValue(toEmotion, out var settings)) return;

        if (settings.transitionParticlesPrefab != null)
        {
            SpawnTransitionParticles(settings.transitionParticlesPrefab, settings.particleDuration);
        }

        if (settings.transitionSound != null)
        {
            audioSource.PlayOneShot(settings.transitionSound);
        }

        if (screenFlash != null)
        {
            screenFlash.Flash(settings.screenFlashColor, settings.flashDuration);
        }
    }

    private void SpawnTransitionParticles(GameObject prefab, float duration)    
    {
        if (prefab == null || particleSpawnPoint == null) return;

        GameObject instance = Instantiate(prefab, particleSpawnPoint.position, particleSpawnPoint.rotation);
        Destroy(instance, duration);
    }

    private void ApplyLongWindowEnvironment(string emotion, bool immediate)
    {
        if (effectConfig == null || !emotionSettingsMap.TryGetValue(emotion, out var settings)) return;

        targetSkybox = settings.skybox;
        targetAmbientColor = settings.ambientColor;
        targetLightColor = settings.lightColor;
        targetLightIntensity = settings.lightIntensity;
        targetVolumeProfile = settings.volumeProfile;
        transitionDuration = settings.transitionDuration;

        if (immediate)
        {
            currentSkybox = targetSkybox;
            currentAmbientColor = targetAmbientColor;
            currentLightColor = targetLightColor;
            currentLightIntensity = targetLightIntensity;

            RenderSettings.skybox = currentSkybox;
            RenderSettings.ambientLight = currentAmbientColor;
            if (directionalLight != null)
            {
                directionalLight.color = currentLightColor;
                directionalLight.intensity = currentLightIntensity;
            }

            if (postProcessVolume != null && targetVolumeProfile != null)
            {
                postProcessVolume.profile = targetVolumeProfile;
            }

            UpdateAmbientParticles(settings.ambientParticlesPrefab);
        }
        else
        {
            transitionTimer = 0f;
            isTransitioning = true;
        }
    }

    public void ApplyEmotionEffect(string emotion, bool immediate = false)
    {
        if (effectConfig == null || !emotionSettingsMap.TryGetValue(emotion, out var settings)) return;

        currentEmotion = emotion;
        ApplyLongWindowEnvironment(emotion, immediate);

        if (settings.backgroundMusic != null && audioSource != null)
        {
            audioSource.clip = settings.backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void UpdateAmbientParticles(GameObject prefab)
    {
        if (currentAmbientParticles != null)
        {
            Destroy(currentAmbientParticles);
        }

        if (prefab != null && particleSpawnPoint != null)
        {
            currentAmbientParticles = Instantiate(prefab, particleSpawnPoint.position, particleSpawnPoint.rotation, particleSpawnPoint);
        }
    }

    private void Update()
    {
        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);

            if (currentSkybox != targetSkybox)
            {
                Material blended = new Material(currentSkybox);
                blended.Lerp(currentSkybox, targetSkybox, t);
                RenderSettings.skybox = blended;
            }

            RenderSettings.ambientLight = Color.Lerp(currentAmbientColor, targetAmbientColor, t);
            
            if (directionalLight != null)
            {
                directionalLight.color = Color.Lerp(currentLightColor, targetLightColor, t);
                directionalLight.intensity = Mathf.Lerp(currentLightIntensity, targetLightIntensity, t);
            }

            if (postProcessVolume != null && targetVolumeProfile != null && t >= 0.5f)
            {
                postProcessVolume.profile = targetVolumeProfile;
            }

            if (t >= 1f)
            {
                currentSkybox = targetSkybox;
                currentAmbientColor = targetAmbientColor;
                currentLightColor = targetLightColor;
                currentLightIntensity = targetLightIntensity;
                isTransitioning = false;

                if (emotionSettingsMap.TryGetValue(currentEmotion, out var settings))
                {
                    UpdateAmbientParticles(settings.ambientParticlesPrefab);
                }
            }
        }
    }
}