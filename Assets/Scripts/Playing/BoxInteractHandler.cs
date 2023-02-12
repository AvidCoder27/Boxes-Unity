using UnityEngine;

public class BoxInteractHandler : Interactable
{
    [SerializeField] private Box ParentBox;

    public override void InteractedWith(Transform player)
    {
        if (player.TryGetComponent(out PlayerMovement _))
        {
            ParentBox.Interact();
        }
    }
}
