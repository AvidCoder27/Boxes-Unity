using UnityEngine;

public class Star : Collectable
{
    private Light innerLight;
    private AudioSource levelCompleteSound;

    private protected override void LateAwake()
    {
        levelCompleteSound = GetComponentInChildren<AudioSource>();
        innerLight = GetComponentInChildren<Light>();
        innerLight.enabled = false;
        doSpin = true;
    }

    private protected override void AnimationStart()
    {
        innerLight.enabled = true;
        levelCompleteSound.Play();
    }
}
