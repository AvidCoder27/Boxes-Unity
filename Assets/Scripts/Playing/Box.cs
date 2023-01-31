using UnityEngine;
using Unity.Mathematics;
using System;
using System.Collections;

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
    GameHandler gameHandler;
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

    public void SetGameHandlerRef(GameHandler g)
    {
        if (gameHandler == null) gameHandler = g;
    }
    public void SetPlayingCharacterRef(Transform t)
    {
        if (playingCharacter == null) playingCharacter = t;
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
        Actions.OnTryInteractBox += HandleTryInteractBoxAction;
        Actions.OnInverterActivated += InvertIsOpen;
    }
    private void OnDisable()
    {
        Actions.OnTryInteractBox -= HandleTryInteractBoxAction;
        Actions.OnInverterActivated -= InvertIsOpen;
    }

    private void HandleTryInteractBoxAction(int3 attemptedBoxIndex)
    {
        if (attemptedBoxIndex.x == boxIndex.x && attemptedBoxIndex.y == boxIndex.y && attemptedBoxIndex.z == boxIndex.z)
            TryInteractBox();
    }

    private void InvertIsOpen()
    {
        if (isOpen)
        {
            isOpen = false;
        } else
        {
            isOpen = true;
            //float t = boxIndex.x * 3 + boxIndex.y * 2 + boxIndex.z;
            //Invoke(nameof(PlayOpenSound), t/100);
        }
    }

    public void TryInteractBox()
    {
        if (gameHandler.Stage == GameHandler.GameStage.Playing)
        {
            if (isOpen) InteractedWhenOpen();
            else InteractedWhenClosed();
        }
    }

    private void InteractedWhenOpen()
    {
        switch (contents)
        {
            case Contents.Star:
                star.StartWinAnimation(this, null);
                Actions.OnGameEnd?.Invoke(Actions.GameEndState.Win);
                break;
            case Contents.Ladder:
                ladder.StartClimbing(playingCharacter, false);
                break;
            case Contents.Inverter:
                PlayOpenSound();
                Actions.OnInverterActivated?.Invoke();
                break;
        }
    }

    private void InteractedWhenClosed()
    {
        if (allowOpening)
        {
            PlayOpenSound();
            isOpen = true;
            switch (contents)
            {
                case Contents.Star:
                    star.StartWinAnimation(this, null);
                    Actions.OnGameEnd?.Invoke(Actions.GameEndState.Win);
                    break;
                case Contents.None:
                    Actions.OnGameEnd?.Invoke(Actions.GameEndState.Lose);
                    break;
                default:
                    // Don't lose
                    break;
            }
        }
    }

    private void PlayOpenSound()
    {
        BoxOpenSound.Play();
    }
}