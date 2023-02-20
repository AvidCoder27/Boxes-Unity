using UnityEngine;
using UnityEngine.InputSystem;

public class LadderAccessPoint : Interactable
{
    private enum PlayerState
    {
        None, Aligning, StartClimbing, Climbing
    }

    [SerializeField] private PlayerState playerState;
    [SerializeField] private float alignmentTime;
    private float alignmentStartTime;
    private Vector3 translationStart;
    private Quaternion rotationStart;
    private Quaternion camRotStart;
    private Transform animatedParent;
    private Transform entryPoint;
    private Animator animator;
    private Transform player;
    private PlayerInput playerInput;
    private Rigidbody playerRigidBody;
    private CharacterLook characterLook;
    private Transform cam;

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
            playerInput = player.GetComponentInChildren<PlayerInput>();
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
                    playerState = PlayerState.StartClimbing;
                    player.SetParent(animatedParent);
                    animator.SetTrigger("StartClimbing");
                }
                break;
            case PlayerState.StartClimbing:
                StartCoroutine(AnimatorWatcher.WaitForAnimatorFinished(animator, 0, FinishedMovement));
                playerState = PlayerState.Climbing;
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
