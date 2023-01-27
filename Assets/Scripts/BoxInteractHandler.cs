using UnityEngine;

public class BoxInteractHandler : MonoBehaviour
{
    [SerializeField] Box ParentBox;

    public void OnMouseDown()
    {
        ParentBox.TryInteractBox();
    }
}
