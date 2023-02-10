using System;

public class Key : Collectable
{
    [Flags] public enum Color
    {
        Undefined = 0, Green = 1, Red = 2, Purple = 4, Gold = 8
    }

    private Color color;
    private PlayerMovement playerMovement;

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

    public void SetPlayerMovementRef(PlayerMovement playerMovement)
    {
        this.playerMovement = playerMovement;
    }

    private protected override void LateAwake()
    {
        doSpin = false;
    }

    private protected override void AnimationStart()
    {
        playerMovement.GiveKey(color);
    }
}
