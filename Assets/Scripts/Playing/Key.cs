using System;
using System.Collections.Generic;
using UnityEngine;

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

    private Colors color;
    private PlayerMovement playerMovement;

    public void SetColor(Colors color)
    {
        if (this.color == Colors.Undefined)
        {
            this.color = color;
        }
    }

    public Colors GetColor()
    {
        return color;
    }

    public void SetPlayerMovementRef(PlayerMovement playerMovement)
    {
        this.playerMovement = playerMovement;
    }

    private protected override void AwakeInherited()
    {
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
        return GetAllKeyColors()[(int)Math.Log((double)color, 2)];
    }

    private void Start()
    {
        SetColor(Colors.Red); // default to red for testing
        UpdateMatColors(xrayEmissionIntensity);
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

    private void OnEnable()
    {
        Actions.OnSceneSwitchSetup += UpdateMatColors;
    }
    private void OnDisable()
    {
        Actions.OnSceneSwitchSetup -= UpdateMatColors;
    }

    private void UpdateMatColors()
    {
        UpdateMatColors(standardEmissionIntensity);
    }
    private void UpdateMatColors(float emmisiveFactor)
    {
        keyRenderer.material.EnableKeyword("_EMISSION");
        keyRenderer.material.SetColor("_EmissionColor", GetColorOfKeyColor(color) * emmisiveFactor);
    }

}
