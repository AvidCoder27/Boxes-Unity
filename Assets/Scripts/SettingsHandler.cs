using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    public static float MasterVolume { get; private set; }
    public static float MusicVolume { get; private set; }
    public static float SoundVolume { get; private set; }

    private static readonly string _idMasterVolume = "MasterVolume";
    private static readonly string _idMusicVolume = "MusicVolume";
    private static readonly string _idSoundVolume = "SoundVolume";
    private static readonly float _defaultVolume = 20f;

    private static SettingsHandler Instance;
    [SerializeField] private Slider _uiMasterVolume;
    [SerializeField] private Slider _uiMusicVolume;
    [SerializeField] private Slider _uiSoundVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.
        }

        SceneManager.sceneLoaded += (Scene _, LoadSceneMode _) => LoadSettings();
    }

    private void UpdateInputUI()
    {
        _uiMasterVolume.value = MasterVolume;
        _uiMusicVolume.value = MusicVolume;
        _uiSoundVolume.value = SoundVolume;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(_idMasterVolume, MasterVolume);
        PlayerPrefs.SetFloat(_idMusicVolume, MusicVolume);
        PlayerPrefs.SetFloat(_idSoundVolume, SoundVolume);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat(_idMasterVolume, _defaultVolume);
        MusicVolume = PlayerPrefs.GetFloat(_idMusicVolume, _defaultVolume);
        SoundVolume = PlayerPrefs.GetFloat(_idSoundVolume, _defaultVolume);
        UpdateInputUI();
    }

    public static SettingsHandler GetInstance() => Instance;
    public void SetMasterVolume(float volume) => MasterVolume = volume;
    public void SetMusicVolume(float volume) => MusicVolume = volume;
    public void SetSoundVolume(float volume) => SoundVolume = volume;
}
