using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private Selectable _settingsExit;
    [SerializeField] private Selectable _settingsEntry;
    private InputSystemUIInputModule _inputModule;
    private Action OnCancel;
    private Actions.GameEndState _gameEndState;
    
    private void Awake()
    {
        OnCancel = null;
        _inputModule = GetComponent<InputSystemUIInputModule>();

        _endScreen.SetActive(false);
        _settingsMenu.SetActive(false);
        _pauseMenu.SetActive(false);
    }

    private void OnEnable()
    {
        _inputModule.cancel.action.performed += HandleCancelInput;
        Actions.OnGameEnd += HandleGameEnd;
    }

    private void OnDisable()
    {
        _inputModule.cancel.action.performed -= HandleCancelInput;
        Actions.OnGameEnd -= HandleGameEnd;
    }

    private void HandleGameEnd(Actions.GameEndState gameEndState)
    {
        _endScreen.SetActive(true);
        SetEndScreenText(gameEndState == Actions.GameEndState.Win);

        _inputModule.submit.action.performed += LoadNextLevelOnSubmit;
        _gameEndState = gameEndState;
    }

    private void LoadNextLevelOnSubmit(InputAction.CallbackContext ctx)
    {
        _inputModule.submit.action.performed -= LoadNextLevelOnSubmit;
        Actions.OnNextLevel?.Invoke(_gameEndState);
    }

    private void SetEndScreenText(bool won)
    {
        _endScreen.transform.Find("bottom text/win message").gameObject.SetActive(won);
        _endScreen.transform.Find("bottom text/lose message").gameObject.SetActive(!won);
    }

    private void HandleCancelInput(InputAction.CallbackContext ctx)
    {
        OnCancel?.Invoke();
    }

    public void Pause()
    {
        OnCancel = Resume;
        _pauseMenu.SetActive(true);
        _settingsMenu.SetActive(false);
        Actions.OnGamePause?.Invoke();
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        OnCancel = null;
        _settingsMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        Actions.OnGameResume?.Invoke();
        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        OnCancel = CloseSettings;
        _settingsMenu.SetActive(true);
        _settingsEntry.Select();
    }

    public void CloseSettings()
    {
        OnCancel = Resume;
        _settingsMenu.SetActive(false);
        _settingsExit.Select();
    }
}
