using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Box : MonoBehaviour
{
    public enum Contents { None, Star, Ladder, Inverter, Key }

    /// <summary>
    /// X: Floor that the box is on. 0 is the top, 1 is the floor directly below, etc.
    /// Y: Column that the box is in
    /// Z: Row that the box is in. 0 is bottom, 1 is top
    /// </summary>
    public int3 Index { get; private set; }
    [SerializeField] private bool isOpen;
    private Contents contents;
    private bool allowOpening;
    private Key.Colors lockColor = Key.Colors.Undefined;
    private Key.Colors keyColor = Key.Colors.Undefined;

    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject inverterPrefab;
    [SerializeField] private GameObject ladderPrefab;
    [SerializeField] private AudioSource BoxOpenSound;
    [SerializeField] private AudioSource UnlockSound;
    private Transform playingCharacter;
    private PlayerMovement playerMovement;
    private GameObject openedBox;
    private GameObject closedBox;
    private Collectable collectable;
    private PlayingLadder ladder;

    private void Awake()
    {
        openedBox = transform.Find("container/Box/opened").gameObject;
        closedBox = transform.Find("container/Box/closed").gameObject;
    }

    public void SetRefs(Transform playingCharacter)
    {
        if (this.playingCharacter == null)
        {
            this.playingCharacter = playingCharacter;
            playerMovement = playingCharacter.GetComponent<PlayerMovement>();
        }
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
                Key key = go.GetComponent<Key>();
                key.SetColor(keyColor);
                key.SetPlayerMovementRef(playerMovement);
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

    private void Update()
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
        {
            Interact();
        }
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
            if (playerMovement.HasKey(lockColor))
            {
                isOpen = true;
                if (lockColor != Key.Colors.Undefined)
                {
                    // remove lock from box after it is unlocked
                    lockColor = Key.Colors.Undefined;
                    StartCoroutine(UnlockAndInteractWithContents());
                } else
                {
                    OpenAndInteractWithContents();
                }
            }
            else
            {
                Debug.Log("lost game by failing to unlock box");
                TriggerGameLose();
            }
        }
    }

    private IEnumerator UnlockAndInteractWithContents()
    {
        UnlockSound.Play();
        Action unlockCallback = playerMovement.LockInputWithCallback();
        yield return new WaitForSeconds(0.5f);
        unlockCallback?.Invoke();
        BoxOpenSound.Play();
        InteractWithContents();
    }

    private void OpenAndInteractWithContents()
    {
        BoxOpenSound.Play();
        InteractWithContents();
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
                if (collectable.Done)
                {
                    TriggerGameLose();
                } else
                {
                    collectable.StartCollectAnimation(this, KeyAnimationCallback);
                }
                break;

            case Contents.Ladder:
                ladder.StartClimbing(playingCharacter, false);
                break;
            case Contents.Inverter:
                Actions.OnInverterActivated?.Invoke();
                break;
            case Contents.None:
                TriggerGameLose();
                break;
        }
    }

    private void TriggerGameLose()
    {
        Actions.OnGameEnd?.Invoke(Actions.GameEndState.Lose);
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