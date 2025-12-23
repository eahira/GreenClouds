using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 0.6f;
    [Range(0f, 1f)] public float sfxVolume = 1.0f;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Optional Overrides (exact scene names)")]
    public string[] menuSceneNames =
    {
        "MainMenuScene",
        "CharacterSelectorScene",
        "DifficultySelectorScene",
        "ShopScene",
        "SettingsScene",
        "TutorialScene",
        "PauseScene",
        "LevelCompleteScene",
        "DefeatScene",
        "FinalWinScene"
    };

    public string[] gameSceneNames =
    {
        "SampleScene"
    };

    [Header("Auto-detect (keywords in scene name)")]
    public string[] menuKeywords =
    {
        "Menu","Shop","Selector","Settings","Tutorial","Pause","Win","Defeat","Complete"
    };

    [Header("UI")]
    public AudioClip uiClick;

    [Header("Combat")]
    public AudioClip playerHit;
    public AudioClip enemyHit;
    public AudioClip ultimate;

    [Header("World")]
    public AudioClip roomTeleport;
    public AudioClip coinPickup;
    public AudioClip healPickup;
    public AudioClip artifactPickup;

    private bool _subscribed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureSources();
        ApplyVolumes();

        Subscribe();

        DecideMusicForScene(SceneManager.GetActiveScene().name, forceRestart: false);
    }

    private void OnEnable() => Subscribe();

    private void OnDisable()
    {
        if (!_subscribed) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        _subscribed = false;
    }

    private void OnDestroy()
    {
        if (Instance == this && _subscribed)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            _subscribed = false;
        }
    }

    private void Subscribe()
    {
        if (_subscribed) return;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        _subscribed = true;
    }

    private void EnsureSources()
    {
        if (musicSource == null)
        {
            var m = new GameObject("MusicSource");
            m.transform.SetParent(transform);
            musicSource = m.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            var s = new GameObject("SFXSource");
            s.transform.SetParent(transform);
            sfxSource = s.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    public void ApplyVolumes()
    {
        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DecideMusicForScene(SceneManager.GetActiveScene().name, forceRestart: false);
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        DecideMusicForScene(newScene.name, forceRestart: false);
    }

    private void DecideMusicForScene(string sceneName, bool forceRestart)
    {
        if (IsInList(sceneName, menuSceneNames))
        {
            PlayMusic(menuMusic, restartIfSame: forceRestart);
            return;
        }

        if (IsInList(sceneName, gameSceneNames))
        {
            PlayMusic(gameMusic, restartIfSame: forceRestart);
            return;
        }

        if (ContainsAny(sceneName, menuKeywords))
        {
            PlayMusic(menuMusic, restartIfSame: forceRestart);
            return;
        }

        PlayMusic(gameMusic, restartIfSame: forceRestart);
    }

    private bool IsInList(string sceneName, string[] list)
    {
        if (list == null) return false;
        for (int i = 0; i < list.Length; i++)
            if (list[i] == sceneName) return true;
        return false;
    }

    private bool ContainsAny(string text, string[] keywords)
    {
        if (keywords == null) return false;
        for (int i = 0; i < keywords.Length; i++)
        {
            var k = keywords[i];
            if (string.IsNullOrEmpty(k)) continue;
            if (text.Contains(k)) return true;
        }
        return false;
    }

    public void PlayMusic(AudioClip clip, bool restartIfSame = false)
    {
        if (clip == null || musicSource == null) return;

        if (!restartIfSame && musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    private void PlaySFX(AudioClip clip, float volumeMul = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(sfxVolume * volumeMul));
    }

    public void PlayUIClick() => PlaySFX(uiClick, 0.9f);
    public void PlayPlayerHit() => PlaySFX(playerHit, 1.0f);
    public void PlayEnemyHit() => PlaySFX(enemyHit, 0.9f);
    public void PlayUltimate() => PlaySFX(ultimate, 1.0f);
    public void PlayRoomTeleport() => PlaySFX(roomTeleport, 0.9f);
    public void PlayCoinPickup() => PlaySFX(coinPickup, 0.85f);
    public void PlayHealPickup() => PlaySFX(healPickup, 0.9f);
    public void PlayArtifactPickup() => PlaySFX(artifactPickup, 1.0f);
}
