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
    [SerializeField]
    private bool doSpin;

    private protected Action OnAnimationComplete;
    private Transform animationParent;
    private Animator animator;
    private bool isMoving;
    private float spinTimeElapsed;
    private float finishSpinDelay;

    private void Awake()
    {
        finishSpinDelay = 0.05f;
        animationParent = transform.Find("Animation Parent");
        animator = animationParent.GetComponent<Animator>();
        AwakeInherited();
    }

    private protected abstract void AwakeInherited();

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
        if (parentBox.Index.z == 0)
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
