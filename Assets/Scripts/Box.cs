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
    GameHandler gameHandler;
    GameObject starGO;
    Star star;
    AudioSource BoxOpenSound;
    GameObject openedBox;
    GameObject closedBox;

    private void Awake()
    {
        openedBox = transform.Find("container/Box/opened").gameObject;
        closedBox = transform.Find("container/Box/closed").gameObject;
        BoxOpenSound = GetComponent<AudioSource>();
        gameHandler = GameHandler.GetInstance();
    }

    private void Start()
    {
        if (contents == Contents.Star)
        {
            starGO = Instantiate(starPrefab, transform.position, transform.rotation);
            star = starGO.GetComponent<Star>();
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
        Actions.OnTryOpenBox += HandleTryOpenBoxAction;
    }
    private void OnDisable()
    {
        Actions.OnTryOpenBox -= HandleTryOpenBoxAction;
    }

    private void HandleTryOpenBoxAction(int3 attemptedBoxIndex)
    {
        if (attemptedBoxIndex.x == boxIndex.x && attemptedBoxIndex.y == boxIndex.y && attemptedBoxIndex.z == boxIndex.z)
            TryOpenBox();
    }

    public void TryOpenBox()
    {
        if (gameHandler.Stage == GameHandler.GameStage.Playing && !isOpen)
        {
            BoxOpenSound.Play();
            isOpen = true;
            if (contents == Contents.Star) WinLevel();
            else FailLevel();
        }
    }

    void WinLevel()
    {
        star.StartWinAnimation(this, null);

        Actions.OnGameEnd?.Invoke(Actions.GameEndState.Win);
    }

    void FailLevel()
    {
        Actions.OnGameEnd?.Invoke(Actions.GameEndState.Lose);
    }
}