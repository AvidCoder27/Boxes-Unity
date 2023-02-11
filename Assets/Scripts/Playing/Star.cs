using UnityEngine;

public class Star : Collectable
{
    private Light innerLight;
    private AudioSource levelCompleteSound;

    private protected override void AwakeInherited()
    {
        levelCompleteSound = GetComponentInChildren<AudioSource>();
        innerLight = GetComponentInChildren<Light>();
        innerLight.enabled = false;
    }

    private protected override void AnimationStart()
    {
        innerLight.enabled = true;
        levelCompleteSound.Play();
    }
}
