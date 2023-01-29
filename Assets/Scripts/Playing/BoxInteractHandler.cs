using UnityEngine;

public class BoxInteractHandler : Interactable
{
    [SerializeField] Box ParentBox;

    public override void InteractedWith(Transform player)
    {
        ParentBox.TryInteractBox();
    }
}
