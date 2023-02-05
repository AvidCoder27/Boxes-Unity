public class Key : Collectable
{
    private protected override void LateAwake()
    {
        doSpin = false;
    }
    private protected override void AnimationStart() { }
}
