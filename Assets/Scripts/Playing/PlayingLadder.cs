using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingLadder : Interactable
{
    private enum ClimbState { None, Up, Down }

    [SerializeField] private ClimbState climbingState;
    private Transform player;
    private PlayerInput playerInput;
    private Transform bottomEntrance;
    private Transform topEntrance;
    private Transform animatedParent;
    private Animator animator;
    private int floorOfTop;
    private int column;
    private Box containingBox;

    private void Awake()
    {
        // container, Ladder, Box
        containingBox = transform.parent.parent.parent.GetComponent<Box>();

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
        {
            StartClimbing(player, true);
        }
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
        // first, check if player is climbing up because its easiest
        if (climbingUp)
        {
            // if the player can't open the box, then they fail!
            if (!containingBox.CanPlayerOpenMe())
            {
                Debug.Log("I will not let you climb a ladder into a box you cannot open");
                return;
            }
        }

        this.player = player;
        if (climbingState == ClimbState.None)
        {
            playerInput = player.GetComponentInChildren<PlayerInput>();
            playerInput.DeactivateInput();
            if (climbingUp)
            {
                climbingState = ClimbState.Up;
                animatedParent.position = bottomEntrance.position;
                player.position = bottomEntrance.position;
                animator.SetTrigger("ClimbUp");
            }
            else
            {
                climbingState = ClimbState.Down;
                animatedParent.position = topEntrance.position;
                player.position = topEntrance.position;
                animator.SetTrigger("ClimbDown");
            }
            player.SetParent(animatedParent);
            StartCoroutine(AnimatorWatcher.WaitForAnimatorFinished(animator, 0, FinishMovement));
        }
    }

    private void FinishMovement()
    {
        // change player floor: -1 if climbing up, else +1
        player.GetComponent<PlayerMovement>().ChangeFloor(climbingState == ClimbState.Up ? -1 : +1);
        player.SetParent(null);
        playerInput.ActivateInput();
        animator.SetTrigger("StopClimbing");
        climbingState = ClimbState.None;
    }
}