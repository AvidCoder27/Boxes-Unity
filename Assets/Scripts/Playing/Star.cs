using UnityEngine;

public class Star : Collectable
{
    private Light innerLight;

    private protected override void AwakeInherited()
    {
        innerLight = GetComponentInChildren<Light>();
        innerLight.enabled = false;
    }

    private protected override void AnimationStart()
    {
        innerLight.enabled = true;
    }
}
