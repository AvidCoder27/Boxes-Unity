using System.Threading;
using UnityEngine;

public class XrayDuringPreparation : MonoBehaviour
{
    [SerializeField] Material xrayMaterial;
    [SerializeField] Material normalMaterial;
    [SerializeField] new Renderer renderer;

    [SerializeField] LayerMask defaultLayer;
    [SerializeField] LayerMask xRayLayer;

    private void OnEnable()
    {
        Actions.OnSceneSwitchSetup += HideXray;
    }
    private void OnDisable()
    {
        Actions.OnSceneSwitchSetup += HideXray;
    }

    private void Start()
    {
        int layerNum = (int)Mathf.Log(xRayLayer.value, 2);
        gameObject.layer = layerNum;

        if (transform.childCount > 0)
        {
            SetLayerAllChildren(transform, layerNum);
        }

        renderer.material = xrayMaterial;
    }

    private void HideXray()
    {
        int layerNum = (int)Mathf.Log(defaultLayer.value, 2);
        gameObject.layer = layerNum;

        if (transform.childCount > 0)
        {
            SetLayerAllChildren(transform, layerNum);
        }

        renderer.material = normalMaterial;
    }

    private void SetLayerAllChildren(Transform parent, int layer)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive: true);

        foreach (Transform child in children)
        {
            child.gameObject.layer = layer;
        }
    }
}
