using System;
using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] float rotationTime;
    [SerializeField] float rotationSpeedFactor;
    [SerializeField] AnimationCurve rotationCurve;
    Transform animationParent;
    Light innerLight;
    AudioSource levelCompleteSound;
    Animator animator;
    float animTimeElapsed;
    bool isMoving;
    Action OnAnimationComplete;

    private void Awake()
    {
        levelCompleteSound = GetComponent<AudioSource>();
        animationParent = transform.Find("Animation Parent");
        innerLight = animationParent.GetComponent<Light>();
        animator = animationParent.GetComponent<Animator>();
        innerLight.enabled = false;
    }

    void Update()
    {
        if (isMoving)
        {
            animTimeElapsed += Time.deltaTime;
            if (animTimeElapsed < rotationTime)
            {
                float timeRatio = animTimeElapsed / rotationTime;
                float angleDelta = rotationCurve.Evaluate(timeRatio) * rotationSpeedFactor * Time.deltaTime * 1000f;
                animationParent.rotation = Quaternion.Euler(0f, angleDelta, 0f) * animationParent.rotation;
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                isMoving = false;
                OnAnimationComplete?.Invoke();
            }
        }
    }

    public void StartWinAnimation(Box parentBox, Action OnAnimationComplete)
    {
        this.OnAnimationComplete = OnAnimationComplete;
        innerLight.enabled = true;
        levelCompleteSound.Play();
        isMoving = true;
        animTimeElapsed = 0;
        if (parentBox.boxIndex.z == 0) {
            animator.SetTrigger("BottomMotion");
        } else {
            animator.SetTrigger("TopMotion");
        }
    }
}
