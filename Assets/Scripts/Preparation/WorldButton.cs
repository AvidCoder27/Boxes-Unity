using UnityEngine;
using UnityEngine.Events;

public class WorldButton : Interactable
{
    [SerializeField] private UnityEvent interactEvent;
    public override void InteractedWith(Transform player)
    {
        interactEvent?.Invoke();
    }
}