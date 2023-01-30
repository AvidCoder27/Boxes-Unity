using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LadderAccessPoint : Interactable
{
    enum PlayerState
    {
        None, Aligning, Climbing
    }

    [SerializeField] PlayerState playerState;
    [SerializeField] float alignmentTime;
    float alignmentStartTime;
    Vector3 translationStart;
    Quaternion rotationStart;
    Quaternion camRotStart;
    Transform animatedParent;
    Transform entryPoint;
    Animator animator;

    Transform player;
    PlayerInput playerInput;
    Rigidbody playerRigidBody;
    CharacterLook characterLook;
    Transform cam;

    private void Awake()
    {
        entryPoint = transform.Find("Entry Point");
        animatedParent = entryPoint.Find("Animated Parent");
        animator = animatedParent.GetComponent<Animator>();
        cam = Camera.main.transform;
    }

    public override void InteractedWith(Transform player)
    {
        if (playerState == PlayerState.None)
        {
            this.player = player;
            playerInput = player.GetComponent<PlayerInput>();
            characterLook = player.GetComponent<CharacterLook>();
            playerRigidBody = player.GetComponent<Rigidbody>();
            StartMovement();
        }
    }

    private void StartMovement()
    {
        alignmentStartTime = Time.time;
        translationStart = player.position;
        rotationStart = player.rotation;
        camRotStart = cam.localRotation;

        playerInput.DeactivateInput();
        playerState = PlayerState.Aligning;
        playerRigidBody.constraints = RigidbodyConstraints.FreezeAll;
        characterLook.AllowLooking = false;
    }

    private void Update()
    {
        switch (playerState)
        {
            case PlayerState.Aligning:
                float timeRatio = (Time.time - alignmentStartTime) / alignmentTime;

                player.SetPositionAndRotation(
                    Vector3.Lerp(translationStart, entryPoint.position, timeRatio),
                    Quaternion.Slerp(rotationStart, entryPoint.rotation, timeRatio)
                    //Quaternion.Euler(Vector3.Slerp(rotationStart.eulerAngles, entryPoint.eulerAngles + Vector3.right * 360, timeRatio))
                    );

                cam.localRotation = Quaternion.Slerp(camRotStart, Quaternion.identity, timeRatio);

                if (timeRatio >= 1)
                {
                    playerState = PlayerState.Climbing;
                    player.SetParent(animatedParent);
                    animator.SetTrigger("StartClimbing");
                }
                break;
            case PlayerState.Climbing:
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                    FinishedMovement();
                break;
        }
    }

    private void FinishedMovement()
    {
        animator.SetTrigger("StopClimbing");
        player.SetParent(null);
        animatedParent.position = entryPoint.position;

        playerInput.ActivateInput();
        playerState = PlayerState.None;
        playerRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        characterLook.AllowLooking = true;
    }
}
