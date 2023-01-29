using System;
using Unity.Mathematics;
using UnityEngine;

public static class Actions
{
    public enum GameEndState { Win, Lose }
    public static Action<GameEndState> OnGameEnd;
    public static Action<GameEndState> OnNextLevel;

    public static Action OnSceneSwitchStart;
    public static Action OnSceneSwitchSetup;
    public static Action OnSceneSwitchEnd;

    /// <summary>
    /// Pass in floor as x, column as y, row as z
    /// </summary>
    public static Action<int3> OnTryInteractBox;
    /// <summary>
    /// Pass in floor, column, and the player transform
    /// </summary>
    public static Action<int, int, Transform> OnTryClimbLadder;
}
