using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwitcher : MonoBehaviour
{
    [SerializeField] Transform PlayingCharacter;
    [SerializeField] Transform PreparationCharacter;

    [SerializeField] GameObject hideDuringPlay;

    PlayerInput playingInput;
    PlayerInput preparationInput;
    IrisScreenwipeController irisScreenwipeController;

    Transform playCamParent;

    private void Awake()
    {
        playCamParent = PlayingCharacter.Find("CameraParent");

        playingInput = PlayingCharacter.GetComponent<PlayerInput>();
        preparationInput = PreparationCharacter.GetComponent<PlayerInput>();

        irisScreenwipeController = GetComponent<IrisScreenwipeController>();
    }

    public void StartStageSwitch()
    {
        preparationInput.DeactivateInput();
        irisScreenwipeController.StartTransition(1, 0.5f, 1,
            SetupPlayingStage, StartPlayingStage);
        Actions.OnSceneSwitchStart?.Invoke();
    }

    private void SetupPlayingStage()
    {
        hideDuringPlay.SetActive(false);

        transform.SetParent(playCamParent, false);
        PreparationCharacter.gameObject.SetActive(false);
        PlayingCharacter.gameObject.SetActive(true);
        transform.localEulerAngles = Vector3.zero;

        playingInput.DeactivateInput();
        Actions.OnSceneSwitchSetup?.Invoke();
    }

    private void StartPlayingStage()
    {
        playingInput.ActivateInput();
        Actions.OnSceneSwitchEnd?.Invoke();
    }
}
