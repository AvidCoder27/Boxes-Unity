
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterSwitcher : MonoBehaviour
{
    [SerializeField] private Transform PlayCharacter;
    [SerializeField] private Transform PrepCharacter;
    [SerializeField] private UnityEvent OnPauseInput;
    private PlayerInput _playerInput;
    private IrisScreenwipeController _irisScreenwipeController;
    private Transform _playCamParent;
    private Transform _prepCamParent;

    private void Awake()
    {
        _playCamParent = PlayCharacter.Find("CameraParent");
        _prepCamParent = PrepCharacter.Find("CameraParent");

        _playerInput = GetComponent<PlayerInput>();
        _irisScreenwipeController = GetComponent<IrisScreenwipeController>();
    }

    private void Start()
    {
        PrepCharacter.gameObject.SetActive(true);
        PlayCharacter.gameObject.SetActive(false);
        transform.SetParent(_prepCamParent);
        _playerInput.SwitchCurrentActionMap("Preparation Phase");
        _playerInput.ActivateInput();
    }

    private void OnEnable()
    {
        _playerInput.actions["Pause"].performed += Pause_performed;
        Actions.OnGamePause += HandleGamePause;
        Actions.OnGameResume += HandleGameResume;
    }

    private void OnDisable()
    {
        _playerInput.actions["Pause"].performed -= Pause_performed;
        Actions.OnGamePause -= HandleGamePause;
        Actions.OnGameResume -= HandleGameResume;
    }

    public void StartStageSwitch()
    {
        _playerInput.DeactivateInput();
        _irisScreenwipeController.StartTransition(1, 0.5f, 1,
            SetupPlayingStage, StartPlayingStage);
        Actions.OnSceneSwitchStart?.Invoke();
    }

    private void SetupPlayingStage()
    {
        transform.SetParent(_playCamParent, false);
        PrepCharacter.gameObject.SetActive(false);
        PlayCharacter.gameObject.SetActive(true);
        _playerInput.SwitchCurrentActionMap("Playing Phase");
        _playerInput.DeactivateInput();
        transform.localEulerAngles = Vector3.zero;

        Actions.OnSceneSwitchSetup?.Invoke();
    }

    private void StartPlayingStage()
    {
        _playerInput.ActivateInput();
        _playerInput.actions["Pause"].performed += Pause_performed;
        Actions.OnSceneSwitchEnd?.Invoke();
    }

    private void Pause_performed(InputAction.CallbackContext ctx)
    {
        OnPauseInput?.Invoke();
    }

    private void HandleGamePause()
    {
        _playerInput.DeactivateInput();
    }
    private void HandleGameResume()
    {
        _playerInput.ActivateInput();
    }
}
