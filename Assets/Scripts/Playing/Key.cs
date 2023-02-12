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
    [SerializeField] private List<Color> rgbaColors;
    [SerializeField] private float xrayEmissionIntensity;
    [SerializeField] private float standardEmissionIntensity;

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

    private void Start()
    {
        UpdateMatColors(xrayEmissionIntensity);
    }

    private protected override void AnimationStart()
    {
        playerMovement.GiveKey(color);
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
        Color c = rgbaColors[(int)Math.Log((double)color, 2)];
        keyRenderer.material.SetColor("_EmissionColor", c * emmisiveFactor);
    }

}
