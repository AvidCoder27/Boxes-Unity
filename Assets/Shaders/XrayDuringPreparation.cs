using System.Collections.Generic;
using UnityEngine;

public class XrayDuringPreparation : MonoBehaviour
{
    [SerializeField] private Material xrayMaterial;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private List<Renderer> renderers;

    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private LayerMask xRayLayer;
    /// <summary>
    /// If enabled, then instead of changing the material when the game starts, gameObject.SetActive(true).
    /// If enabled, then Normal Material need not be set.
    /// </summary>
    [SerializeField] private bool hideMeDuringPlay;

    private void OnEnable()
    {
        Actions.OnSceneSwitchSetup += HideXray;
    }
    private void OnDisable()
    {
        Actions.OnSceneSwitchSetup -= HideXray;
    }

    private void Awake()
    {
        SetLayerAndMaterial(xRayLayer, xrayMaterial);
    }

    private void HideXray()
    {
        if (hideMeDuringPlay)
        {
            gameObject.SetActive(false);
        }
        else
        {
            SetLayerAndMaterial(defaultLayer, normalMaterial);
        }
    }

    private void SetLayerAndMaterial(LayerMask layer, Material material)
    {
        foreach (Renderer r in renderers)
        {
            r.material = material;
        }

        int layerNum = (int)Mathf.Log(layer.value, 2);
        gameObject.layer = layerNum;

        if (transform.childCount > 0)
        {
            Transform[] children = transform.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (Transform child in children)
            {
                child.gameObject.layer = layerNum;
            }
        }
    }
}
