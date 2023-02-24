using System;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public class Key : Collectable
{
    [Flags]
    public enum Colors
    {
        Undefined = 0, Red = 1, Green = 2, Purple = 4, Gold = 8
    }

    [SerializeField] private Renderer keyRenderer;
    [SerializeField] private float xrayEmissionIntensity;
    [SerializeField] private float standardEmissionIntensity;
    [SerializeField] private AudioSource rattleSound;
    private XrayDuringPreparation xrayHandler;

    private Colors color;
    private PlayerMovement playerMovement;

    public void SetColor(Colors color)
    {
        if (this.color == Colors.Undefined)
        {
            this.color = color;
        }
    }

    public void SetPlayerMovementRef(PlayerMovement playerMovement)
    {
        this.playerMovement = playerMovement;
    }

    private protected override void AwakeInherited()
    {
    }

    private void Start()
    {
        xrayHandler = GetComponent<XrayDuringPreparation>();
        xrayHandler.SetMaterialColors(GetColorOfKeyColor(color), xrayEmissionIntensity, standardEmissionIntensity);
    }

    private protected override void AnimationStart()
    {
        OnAnimationComplete += AnimationEnd;
        OnAnimationComplete += playerMovement.LockInputWithCallback();
    }

    private void AnimationEnd()
    {
        OnAnimationComplete -= AnimationEnd;
        playerMovement.GiveKey(color);
        transform.SetParent(playerMovement.transform);
        rattleSound.Play();
    }

    public static Color[] GetAllKeyColors()
    {
        return new Color[]
        {
            Color.red,
            Color.green,
            new Color(0.61176f, 0f, 1f),
            new Color(1f, 0.77255f, 0f)
        };
    }

    public static Color GetColorOfKeyColor(Colors color)
    {
        if (color == Colors.Undefined)
        {
            Debug.LogError("Cannot get the Unity.Color of undefined Key.Color!");
            return Color.blue;
        }
        return GetAllKeyColors()[(int)Math.Log((double)color, 2)];
    }
}
