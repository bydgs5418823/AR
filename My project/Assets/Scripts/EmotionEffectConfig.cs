using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "EmotionEffectConfig", menuName = "Scriptable Objects/Emotion Effect Config")]
public class EmotionEffectConfig : ScriptableObject
{
    [System.Serializable]
    public class EmotionSettings
    {
        [Header("环境设置")]
        public Material skybox;
        public Color ambientColor = Color.white;
        public Color lightColor = Color.white;
        public float lightIntensity = 1f;
        public VolumeProfile volumeProfile;

        [Header("音频设置")]
        public AudioClip backgroundMusic;
        public AudioClip transitionSound;

        [Header("粒子效果")]
        public GameObject ambientParticlesPrefab;
        public GameObject transitionParticlesPrefab;
        public float particleDuration = 2f;

        [Header("屏幕闪烁")]
        public Color screenFlashColor = new Color(1, 1, 1, 0.2f);
        public float flashDuration = 0.3f;

        [Header("过渡设置")]
        public float transitionDuration = 2f;
    }

    public EmotionSettings happySettings;
    public EmotionSettings sadSettings;
    public EmotionSettings normalSettings;
}