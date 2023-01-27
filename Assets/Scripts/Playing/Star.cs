using System;
using UnityEngine;

public class Star : XrayDuringPreparation
{
    [SerializeField] float moveTime;
    [SerializeField] float rotationTime;
    [SerializeField] float rotationSpeedFactor;
    [SerializeField] AnimationCurve rotationCurve;
    Light innerLight;
    AudioSource levelCompleteSound;
    float animTimeElapsed;
    bool isMoving;
    Vector3 posSetpoint, posStart;
    Action OnAnimationComplete;

    private void Awake()
    {
        innerLight = GetComponent<Light>();
        innerLight.enabled = false;
        levelCompleteSound = GetComponent<AudioSource>();

        // move the star to a better spot in the box
        Vector3 translation = new(0, 0.45f, -0.4f);
        translation = transform.rotation * translation;
        transform.position = transform.position + translation;
    }

    void Update()
    {
        if (isMoving)
        {
            animTimeElapsed += Time.deltaTime;
            if (animTimeElapsed < moveTime)
            {
                UpdateMove();
            }
            if (animTimeElapsed < rotationTime)
            {
                UpdateRotation();
            }
            else {
                isMoving = false;
                OnAnimationComplete?.Invoke();
            }
        }
    }

    void UpdateRotation()
    {
        float timeRatio = animTimeElapsed / rotationTime;
        float angleDelta = rotationCurve.Evaluate(timeRatio) * rotationSpeedFactor * Time.deltaTime * 1000f;
        transform.rotation = Quaternion.Euler(0f, angleDelta, 0f) * transform.rotation;
    }

    void UpdateMove()
    {
        float timeRatio = animTimeElapsed / moveTime;
        timeRatio = Mathf.Pow(timeRatio, 2) * (3 - 2 * timeRatio);
        transform.position = Vector3.Slerp(posStart, posSetpoint, timeRatio);
    }

    public void StartWinAnimation(Box parentBox, Action OnAnimationComplete)
    {
        this.OnAnimationComplete = OnAnimationComplete;
        innerLight.enabled = true;
        levelCompleteSound.Play();
        isMoving = true;
        posStart = transform.position;
        float upFactor;
        float backFactor;
        if (parentBox.boxIndex.z == 0) {
            upFactor = 1.8f;
            backFactor = 1f;
        } else {
            upFactor = 0f;
            backFactor = 2.6f;
        }
        posSetpoint = transform.position + Vector3.up * upFactor + transform.rotation * Vector3.back * backFactor;
    }
}
