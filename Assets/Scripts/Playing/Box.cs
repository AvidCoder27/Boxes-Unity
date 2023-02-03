using UnityEngine;
using Unity.Mathematics;

public class Box : MonoBehaviour
{
    public enum Contents { None, Star, Ladder, Inverter, Key }

    /// <summary>
    /// X: Floor that the box is on. 0 is the top, 1 is the floor directly below, etc.
    /// Y: Column that the box is in
    /// Z: Row that the box is in. 0 is bottom, 1 is top
    /// </summary>
    public int3 boxIndex;
    public bool isOpen;
    public Contents contents;
    public bool allowOpening;

    [SerializeField] GameObject starPrefab;
    [SerializeField] GameObject ladderPrefab;
    Transform playingCharacter;
    AudioSource BoxOpenSound;
    GameObject openedBox;
    GameObject closedBox;

    Star star;
    PlayingLadder ladder;

    private void Awake()
    {
        openedBox = transform.Find("container/Box/opened").gameObject;
        closedBox = transform.Find("container/Box/closed").gameObject;
        BoxOpenSound = GetComponent<AudioSource>();
    }

    public void SetRefs(Transform playingCharacter)
    {
        if (this.playingCharacter == null) this.playingCharacter = playingCharacter;
    }

    private void Start()
    {
        switch (contents)
        {
            case Contents.Star:
                star = Instantiate(starPrefab, transform.position, transform.rotation).GetComponent<Star>();
                break;

            case Contents.Ladder:
                ladder = Instantiate(ladderPrefab, transform.position, transform.rotation, transform).GetComponentInChildren<PlayingLadder>();
                ladder.SetTopFloorAndColumn(boxIndex.x, boxIndex.y);
                break;
        }
    }

    void Update()
    {
        // Set active box based on isOpen
        openedBox.SetActive(isOpen);
        closedBox.SetActive(!isOpen);
    }

    private void OnEnable()
    {
        Actions.OnTryInteractBox += TryInteract;
        Actions.OnInverterActivated += InvertIsOpen;
    }
    private void OnDisable()
    {
        Actions.OnTryInteractBox -= TryInteract;
        Actions.OnInverterActivated -= InvertIsOpen;
    }

    private void InvertIsOpen()
    {
        isOpen = !isOpen;
    }

    private void TryInteract(int3 attemptedBoxIndex)
    {
        if (attemptedBoxIndex.x == boxIndex.x && attemptedBoxIndex.y == boxIndex.y && attemptedBoxIndex.z == boxIndex.z)
            Interact();
    }

    public void Interact()
    {
        // if the box is already open, just interact with contents
        if (isOpen)
        {
            InteractWithContents();
            return;
        }
        // if it's not already open and opening is allowed, open the box and interact
        if (allowOpening)
        {
            isOpen = true;
            BoxOpenSound.Play();
            InteractWithContents();
        }
    }

    private void InteractWithContents()
    {
        switch (contents)
        {
            case Contents.Star:
                star.StartWinAnimation(this, WinAnimationCallback);
                Actions.OnGameEnd?.Invoke(Actions.GameEndState.Win);
                break;
            case Contents.Ladder:
                ladder.StartClimbing(playingCharacter, false);
                break;
            case Contents.Inverter:
                Actions.OnInverterActivated?.Invoke();
                break;
            case Contents.None:
                Actions.OnGameEnd?.Invoke(Actions.GameEndState.Lose);
                break;
        }
    }

    private void WinAnimationCallback()
    {
        Debug.Log("finished animation");
    }
}