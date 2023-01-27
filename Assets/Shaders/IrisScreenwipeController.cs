using System;
using UnityEngine;

public class IrisScreenwipeController : MonoBehaviour
{
    enum TransitionState { GoingDown, Waiting, GoingUp, Finished }

    [SerializeField] float maxRadius;

    [SerializeField] Material IrisScreenwipeMaterial;
    Action FadeDownDoneCallback;
    Action FadeUpDoneCallback;
    float radius;
    
    float upTime;
    float intermediateTime;
    float downTime;
    float elapsedTime;
    TransitionState state;

    private void Start()
    {
        state = TransitionState.Finished;
        radius = maxRadius;
        SetShaderRadius(radius);
    }

    public void StartTransition(float downTime, float intermediateTime, float upTime, Action FadeDownDoneCallback, Action FadeUpDoneCallback)
    {
        elapsedTime = 0;
        state = TransitionState.GoingDown;
        this.downTime = downTime;
        this.intermediateTime = intermediateTime;
        this.upTime = upTime;
        this.FadeDownDoneCallback = FadeDownDoneCallback;
        this.FadeUpDoneCallback = FadeUpDoneCallback;
    }

    private void Update()
    {
        if (state != TransitionState.Finished)
        {
            elapsedTime += Time.deltaTime;
            switch (state)
            {
                case TransitionState.GoingDown:
                    UpdateGoingDown();
                    break;
                case TransitionState.Waiting:
                    UpdateWaiting();
                    break;
                case TransitionState.GoingUp:
                    UpdateGoingUp();
                    break;
            }
            SetShaderRadius(radius);
        }
    }

    private void UpdateGoingDown()
    {
        // lerp down
        radius = Mathf.Lerp(maxRadius, 0, elapsedTime / downTime);

        if (elapsedTime >= downTime)
        {
            state = TransitionState.Waiting;
            elapsedTime = 0;
            radius = 0;
            FadeDownDoneCallback?.Invoke();
        }
    }
    private void UpdateWaiting()
    {
        // just wait
        if (elapsedTime >= intermediateTime)
        {
            state = TransitionState.GoingUp;
            elapsedTime = 0;
        }
    }
    private void UpdateGoingUp()
    {
        // lerp up
        radius = Mathf.Lerp(0, maxRadius, elapsedTime / upTime);

        if (elapsedTime >= upTime)
        {
            state = TransitionState.Finished;
            FadeUpDoneCallback?.Invoke();
        }
    }

    private void SetShaderRadius(float r)
    {
        IrisScreenwipeMaterial.SetFloat("_Radius", r);
    }
}
