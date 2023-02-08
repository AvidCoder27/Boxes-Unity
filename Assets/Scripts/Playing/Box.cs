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
    public int3 Index { get; private set; }
    private bool isOpen;
    private Contents contents;
    private bool allowOpening;
    private Key.Color lockColor = Key.Color.Undefined;
    private Key.Color keyColor = Key.Color.Undefined;

    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject inverterPrefab;
    [SerializeField] private GameObject ladderPrefab;
    private Transform playingCharacter;
    private AudioSource BoxOpenSound;
    private GameObject openedBox;
    private GameObject closedBox;
    private Collectable collectable;
    private PlayingLadder ladder;

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

    public void CopyInAttributes(BoxStruct boxStruct, int3 boxIndex, bool allowOpening)
    {
        Index = boxIndex;
        isOpen = boxStruct.isOpen;
        contents = boxStruct.contents;
        keyColor = boxStruct.keyColor;
        lockColor = boxStruct.lockColor;
        this.allowOpening = allowOpening;
    }

    private void Start()
    {
        switch (contents)
        {
            case Contents.Star:
                collectable = Instantiate(starPrefab, transform.position, transform.rotation, transform)
                    .GetComponent<Collectable>();
                break;

            case Contents.Key:
                GameObject go = Instantiate(keyPrefab, transform.position, transform.rotation, transform);
                collectable = go.GetComponent<Collectable>();
                go.GetComponent<Key>().SetColor(keyColor);
                break;

            case Contents.Ladder:
                ladder = Instantiate(ladderPrefab, transform.position, transform.rotation, transform)
                    .GetComponentInChildren<PlayingLadder>();
                ladder.SetTopFloorAndColumn(Index.x, Index.y);
                break;

            case Contents.Inverter:
                Instantiate(inverterPrefab, transform.position, transform.rotation, transform);
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
        if (attemptedBoxIndex.x == Index.x && attemptedBoxIndex.y == Index.y && attemptedBoxIndex.z == Index.z)
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
                collectable.StartCollectAnimation(this, StarAnimationCallback);
                Actions.OnGameEnd?.Invoke(Actions.GameEndState.Win);
                break;
            case Contents.Key:
                collectable.StartCollectAnimation(this, KeyAnimationCallback);
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

    private void StarAnimationCallback()
    {
        Debug.Log("finished star win anim");
    }

    private void KeyAnimationCallback()
    {
        Debug.Log("finished key animation");
    }
}