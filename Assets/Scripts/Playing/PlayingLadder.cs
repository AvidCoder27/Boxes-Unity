using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingLadder : Interactable
{
    enum ClimbState { None, Up, Down }

    [SerializeField] ClimbState climbingState;
    Transform player;
    PlayerInput playerInput;

    Transform bottomEntrance;
    Transform topEntrance;
    Transform animatedParent;
    Animator animator;

    float finishMovementDelay;
    int floorOfTop;
    int column;

    private void Awake()
    {
        bottomEntrance = transform.Find("Bottom Entrance");
        topEntrance = transform.Find("Top Entrance");
        animatedParent = transform.Find("Animated Parent");
        animator = animatedParent.GetComponent<Animator>();
        climbingState = ClimbState.None;
        floorOfTop = -1;
        column = -1;
    }

    private void OnEnable()
    {
        Actions.OnTryClimbLadder += HandleTryClimbLadder;
    }

    private void OnDisable()
    {
        Actions.OnTryClimbLadder -= HandleTryClimbLadder;
    }

    private void HandleTryClimbLadder(int floor, int column, Transform player)
    {
        if (floor == floorOfTop + 1 && column == this.column)
        {
            StartClimbing(player, true);
        }
    }

    public override void InteractedWith(Transform player)
    {
        if (player.TryGetComponent(out PlayerMovement _))
            StartClimbing(player, true);
    }

    public void SetTopFloorAndColumn(int floorOfTop, int column)
    {
        if (this.floorOfTop == -1 || this.column == -1)
        {
            this.floorOfTop = floorOfTop;
            this.column = column;
        }
    }

    public void StartClimbing(Transform player, bool climbingUp)
    {
        this.player = player;
        if (climbingState == ClimbState.None)
        {
            playerInput = player.GetComponentInChildren<PlayerInput>();
            playerInput.DeactivateInput();
            finishMovementDelay = 0.05f;
            if (climbingUp)
            {
                climbingState = ClimbState.Up;
                animatedParent.position = bottomEntrance.position;
                player.position = bottomEntrance.position;
                animator.SetTrigger("ClimbUp");
            } else
            {
                climbingState = ClimbState.Down;
                animatedParent.position = topEntrance.position;
                player.position = topEntrance.position;
                animator.SetTrigger("ClimbDown");
            }
            player.SetParent(animatedParent);
        }
    }

    private void Update()
    {
        if (climbingState != ClimbState.None)
        {
            if (finishMovementDelay >= 0) finishMovementDelay -= Time.deltaTime;
            bool isAnimating = finishMovementDelay > 0
                || animator.IsInTransition(0)
                || animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
            if (!isAnimating) FinishedMovement();
        }
    }

    private void FinishedMovement()
    {
        // change player floor: -1 if climbing up, else +1
        player.GetComponent<PlayerMovement>().ChangeFloor(climbingState == ClimbState.Up ? -1 : +1);
        player.SetParent(null);
        playerInput.ActivateInput();

        animator.SetTrigger("StopClimbing");
        climbingState = ClimbState.None;
    }
}