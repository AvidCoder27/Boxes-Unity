using System;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    [SerializeField]
    private float spinTime;
    [SerializeField]
    private float spinSpeedFactor;
    [SerializeField]
    private AnimationCurve spinCurve;

    private protected bool doSpin;
    private bool isMoving;
    private float spinTimeElapsed;
    private float finishSpinDelay;
    private Transform animationParent;
    private Animator animator;
    private Action OnAnimationComplete;

    private void Awake()
    {
        finishSpinDelay = 0.05f;
        animationParent = transform.Find("Animation Parent");
        animator = animationParent.GetComponent<Animator>();
        LateAwake();
    }

    private protected abstract void LateAwake();

    private void Update()
    {
        if (!isMoving) return;
        if (doSpin) Spin();
        if (finishSpinDelay >= 0) finishSpinDelay -= Time.deltaTime;
        CheckIfFinished();
    }

    private void Spin()
    {
        spinTimeElapsed += Time.deltaTime;
        if (spinTimeElapsed < spinTime)
        {
            float timeRatio = spinTimeElapsed / spinTime;
            float angleDelta = spinCurve.Evaluate(timeRatio) * spinSpeedFactor * Time.deltaTime * 1000f;
            animationParent.rotation = Quaternion.Euler(0f, angleDelta, 0f) * animationParent.rotation;
        }
    }

    private void CheckIfFinished()
    {
        bool isAnimating = finishSpinDelay > 0
            || (spinTimeElapsed < spinTime && doSpin)
            || animator.IsInTransition(0)
            || animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
        if (!isAnimating)
        {
            isMoving = false;
            OnAnimationComplete?.Invoke();
        }
    }

    public void StartCollectAnimation(Box parentBox, Action OnAnimationComplete)
    {
        this.OnAnimationComplete = OnAnimationComplete;
        isMoving = true;
        spinTimeElapsed = 0;
        if (parentBox.boxIndex.z == 0)
        {
            animator.SetTrigger("BottomMotion");
        }
        else
        {
            animator.SetTrigger("TopMotion");
        }
        AnimationStart();
    }

    private protected abstract void AnimationStart();
}