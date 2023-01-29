using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingLadder : MonoBehaviour
{
    enum ClimbState { None, Up, Down }

    [SerializeField] ClimbState climbingState;
    Transform player;
    PlayerInput playerInput;

    Transform bottomEntrance;
    Transform topEntrance;
    Transform animatedParent;
    Animator animator;

    float allowFinishMovementDelay;

    private void Awake()
    {
        bottomEntrance = transform.Find("Bottom Entrance");
        topEntrance = transform.Find("Top Entrance");
        animatedParent = transform.Find("Animated Parent");
        animator = animatedParent.GetComponent<Animator>();
        climbingState = ClimbState.None;
    }

    public void InteractedWith(Transform player, bool climbingUp)
    {
        this.player = player;
        if (climbingState == ClimbState.None)
        {
            playerInput = player.GetComponent<PlayerInput>();
            playerInput.DeactivateInput();
            allowFinishMovementDelay = 0.05f;
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
            if (allowFinishMovementDelay >= 0) allowFinishMovementDelay -= Time.deltaTime;
            bool isAnimating = allowFinishMovementDelay > 0
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