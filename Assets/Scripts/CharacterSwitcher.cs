using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwitcher : MonoBehaviour
{
    [SerializeField] Transform PlayCharacter;
    [SerializeField] Transform PrepCharacter;

    PlayerInput playInput;
    PlayerInput prepInput;
    IrisScreenwipeController irisScreenwipeController;

    Transform playCamParent;
    Transform prepCamParent;

    private void Awake()
    {
        playCamParent = PlayCharacter.Find("CameraParent");
        prepCamParent = PrepCharacter.Find("CameraParent");

        playInput = PlayCharacter.GetComponent<PlayerInput>();
        prepInput = PrepCharacter.GetComponent<PlayerInput>();

        irisScreenwipeController = GetComponent<IrisScreenwipeController>();
    }

    private void Start()
    {
        PrepCharacter.gameObject.SetActive(true);
        PlayCharacter.gameObject.SetActive(false);
        transform.SetParent(prepCamParent);
    }

    public void StartStageSwitch()
    {
        prepInput.DeactivateInput();
        irisScreenwipeController.StartTransition(1, 0.5f, 1,
            SetupPlayingStage, StartPlayingStage);
        Actions.OnSceneSwitchStart?.Invoke();
    }

    private void SetupPlayingStage()
    {
        transform.SetParent(playCamParent, false);
        PrepCharacter.gameObject.SetActive(false);
        PlayCharacter.gameObject.SetActive(true);
        transform.localEulerAngles = Vector3.zero;

        playInput.DeactivateInput();
        Actions.OnSceneSwitchSetup?.Invoke();
    }

    private void StartPlayingStage()
    {
        playInput.ActivateInput();
        Actions.OnSceneSwitchEnd?.Invoke();
    }
}
