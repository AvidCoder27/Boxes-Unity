using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _settingsMenu;
    
    private void Awake()
    {
        _settingsMenu.SetActive(false);
        _pauseMenu.SetActive(false);
    }

    public void Pause()
    {
        _pauseMenu.SetActive(true);
        Actions.OnGamePause?.Invoke();
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        _settingsMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        Actions.OnGameResume?.Invoke();
        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        _settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        _settingsMenu.SetActive(false);
    }
}
