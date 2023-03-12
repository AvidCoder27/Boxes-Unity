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
    private bool open;
    private Contents contents;
    private bool allowOpening;
    private Key.Colors lockColor = Key.Colors.Undefined;
    private Key.Colors keyColor = Key.Colors.Undefined;

    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject lockPrefab;
    [SerializeField] private float lockXrayEmissionIntensity;
    [SerializeField] private float lockStandardEmissionIntensity;
    [SerializeField] private GameObject inverterPrefab;
    [SerializeField] private GameObject ladderPrefab;
    [SerializeField] private AudioSource BoxOpenSound;
    [SerializeField] private AudioSource UnlockSuccess;
    [SerializeField] private AudioSource UnlockFail;
    private Animator animator;
    private Transform playingCharacter;
    private PlayerMovement playerMovement;
    private Collectable collectable;
    private PlayingLadder ladder;
    private Action movementUnlockCallback;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void SetOpen(bool open, Action callback = null, bool instantly = false)
    {
        this.open = open;
        float speed = instantly ? 100f : 1f;
        string trigger = open ? "Open" : "Close";
        animator.SetFloat("Speed", speed);
        animator.SetTrigger(trigger);
        StartCoroutine(AnimatorWatcher.WaitForAnimatorFinished(animator, 0, callback, 0.3f));
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
        SetOpen(boxStruct.IsOpen, null, true);
        contents = boxStruct.Contents;
        keyColor = boxStruct.KeyColor;
        lockColor = boxStruct.LockColor;
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
                collectable = Instantiate(keyPrefab, transform.position, transform.rotation, transform)
                    .GetComponent<Collectable>();
                Key key = collectable.GetComponent<Key>();
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

        if (lockColor != Key.Colors.Undefined)
        {
            XrayDuringPreparation l = Instantiate(lockPrefab, transform.position, transform.rotation, transform)
                .GetComponent<XrayDuringPreparation>();
            l.SetMaterialColors(Key.GetColorOfKeyColor(lockColor), lockXrayEmissionIntensity, lockStandardEmissionIntensity);
        }
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
        SetOpen(!open);
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
        movementUnlockCallback = playerMovement.LockInputWithCallback();
        // if the box is already open, just interact with contents
        if (open)
        {
            InteractWithContents();
            return;
        }

        if (allowOpening)
        {
            // if it's not already open and opening is allowed, open the box and interact
            // if the player has the key for the box, open it
            if (playerMovement.HasKey(lockColor))
            {
                if (lockColor != Key.Colors.Undefined)
                {
                    // remove lock from box after it is unlocked
                    lockColor = Key.Colors.Undefined;
                    StartCoroutine(Unlock());
                }
                else
                {
                    BoxOpenSound.Play();
                    SetOpen(true, InteractWithContents);
                }
            }
            else
            {
                // if the player lacks the key
                UnlockFail.Play();
                TriggerGameLose();
            }
        }
        else
        {
            // unlock the movement if opening is not allowed
            movementUnlockCallback?.Invoke();
        }
    }

    private IEnumerator Unlock()
    {
        UnlockSuccess.Play();
        yield return new WaitForSeconds(UnlockSuccess.clip.length);

        // open the box and then wait for it to finish
        BoxOpenSound.Play();
        SetOpen(true, InteractWithContents);
        yield break;
    }

    private void InteractWithContents()
    {
        movementUnlockCallback?.Invoke();
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
                }
                else
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

    public static Contents ContentsFromString(string str) => str switch
    {
        "star" => Contents.Star,
        "key" => Contents.Key,
        "ladder" => Contents.Ladder,
        "inverter" => Contents.Inverter,
        _ => Contents.None
    };

    public static Key.Colors ColorFromString(string str) => str switch
    {
        "red" => Key.Colors.Red,
        "green" => Key.Colors.Green,
        "purple" => Key.Colors.Purple,
        "gold" => Key.Colors.Gold,
        _ => Key.Colors.Undefined
    };
}