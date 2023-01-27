using UnityEngine;
using Unity.Mathematics;
using System;

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

    [SerializeField] GameObject starPrefab;
    [SerializeField] GameObject ladderPrefab;
    GameHandler gameHandler;
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
        gameHandler = GameHandler.GetInstance();
    }

    private void Start()
    {
        switch (contents)
        {
            case Contents.Star:
                star = Instantiate(starPrefab, transform.position, transform.rotation).GetComponent<Star>();
                break;

            case Contents.Ladder:
                ladder = Instantiate(ladderPrefab,
                    transform.position + Vector3.down * (Level.DistanceBetweenFloors - 1),
                    transform.rotation).GetComponentInChildren<PlayingLadder>();
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
    }
    private void OnDisable()
    {
        Actions.OnTryInteractBox -= HandleTryInteractBoxAction;
    }

    private void HandleTryInteractBoxAction(int3 attemptedBoxIndex)
    {
        if (attemptedBoxIndex.x == boxIndex.x && attemptedBoxIndex.y == boxIndex.y && attemptedBoxIndex.z == boxIndex.z)
            TryInteractBox();
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
            case Contents.Ladder:
                ladder.InteractedWith(gameHandler.playingCharacter, false);
                break;
        }
    }

    private void InteractedWhenClosed()
    {
        BoxOpenSound.Play();
        isOpen = true;
        switch (contents)
        {
            case Contents.Star:
                star.StartWinAnimation(this, null);
                Actions.OnGameEnd?.Invoke(Actions.GameEndState.Win);
                break;
            case Contents.Ladder:
                // don't lose :)
                break;
            default:
                Actions.OnGameEnd?.Invoke(Actions.GameEndState.Lose);
                break;
        }
    }
}