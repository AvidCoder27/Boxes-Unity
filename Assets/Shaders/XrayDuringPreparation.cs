using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
    private Material _tempXrayMat;
    private Material _tempNormalMat;
    private bool _useTempMats;

    private void OnEnable()
    {
        Actions.OnSceneSwitchSetup += HideXray;
    }
    private void OnDisable()
    {
        Actions.OnSceneSwitchSetup -= HideXray;
    }

    public void SetMaterialColors(Color color, float xrayEmissionIntensity, float standardEmissionIntensity)
    {
        renderers[0].material = normalMaterial;
        _tempNormalMat = renderers[0].material;

        renderers[0].material = xrayMaterial;
        _tempXrayMat = renderers[0].material;

        _useTempMats = true;

        _tempXrayMat.EnableKeyword("_EMISSION");
        _tempNormalMat.EnableKeyword("_EMISSION");
        _tempXrayMat.SetColor("_EmissionColor", color * xrayEmissionIntensity);
        _tempNormalMat.SetColor("_EmissionColor", color * standardEmissionIntensity);
        _tempXrayMat.color = color;
        _tempNormalMat.color = color;
    }

    private void Start()
    {
        if (_useTempMats)
        {
            SetLayerAndMaterial(xRayLayer, _tempXrayMat);
        } else
        {
            SetLayerAndMaterial(xRayLayer, xrayMaterial);
        }
    }

    private void HideXray()
    {
        if (hideMeDuringPlay)
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (_useTempMats)
            {
                SetLayerAndMaterial(defaultLayer, _tempNormalMat);
            } else
            {
                SetLayerAndMaterial(defaultLayer, normalMaterial);
            }
        }
    }

    private void SetLayerAndMaterial(LayerMask layer, Material mat)
    {
        foreach (Renderer r in renderers)
        {
            r.material = mat;
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