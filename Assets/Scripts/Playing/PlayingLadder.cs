using UnityEngine;
using UnityEngine.Assertions;
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
            Debug.Log("parented player to animparent");
        }
    }

    private void Update()
    {
        if (
            climbingState != ClimbState.None &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 &&
            !animator.IsInTransition(0)
            ) FinishedMovement();
    }

    private void FinishedMovement()
    {
        Debug.Log("move finished");
        climbingState = ClimbState.None;
        animator.SetTrigger("StopClimbing");
        player.SetParent(null);
        playerInput.ActivateInput();
    }
}