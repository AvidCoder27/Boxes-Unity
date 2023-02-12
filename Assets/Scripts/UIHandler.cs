using UnityEngine;
using UnityEngine.InputSystem;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    private void Awake()
    {      
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
        _pauseMenu.SetActive(false);
        Actions.OnGameResume?.Invoke();
        Time.timeScale = 1f;
    }
}
