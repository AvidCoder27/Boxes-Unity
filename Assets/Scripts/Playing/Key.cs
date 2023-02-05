public class Key : Collectable
{
    public enum Color
    {
        Green, Red, Purple, Gold, Undefined
    }

    private Color color;

    public void SetColor(Color color)
    {
        if (this.color == Color.Undefined)
        {
            this.color = color;
        }
    }
    public Color GetColor()
    {
        return color;
    }

    private protected override void LateAwake()
    {
        doSpin = false;
    }

    private protected override void AnimationStart()
    {

    }
}
