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

    [Header("scenes MENU")]
    public string[] menuSceneNames =
    {
        "MainMenuScene",
        "CharacterSelectorScene",
        "DifficultySelectorScene",
        "ShopScene"
    };

    [Header("scenes GAME")]
    public string[] gameSceneNames =
    {
        "SampleScene"
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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
        string sceneName = scene.name;

        if (IsInList(sceneName, menuSceneNames))
        {
            PlayMusic(menuMusic);
        }
        else if (IsInList(sceneName, gameSceneNames))
        {
            PlayMusic(gameMusic);
        }
    }

    private bool IsInList(string sceneName, string[] list)
    {
        if (list == null) return false;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == sceneName) return true;
        }
        return false;
    }

    public void PlayMusic(AudioClip clip, bool restartIfSame = false)
    {
        if (clip == null || musicSource == null) return;

        if (!restartIfSame && musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
        musicSource.clip = null;
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
