using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class GameHandler : MonoBehaviour
{
    private static GameHandler Instance;
    public enum GameStage { Preparation, Playing, Transitioning }

    [SerializeField] GameObject boxPrefab;
    [SerializeField] GameObject boxLightPrefab;
    LevelHandler levelHandler;

    public GameStage Stage { get; private set; }
    [SerializeField] float BottomBoxHeight, TopBoxHeight, BoxLightHeight;

    private void Awake()
    {
        Instance = this;

        levelHandler = LevelHandler.GetInstance();

        Stage = GameStage.Preparation;
    }

    public static GameHandler GetInstance() => Instance;

    private void Start()
    {
        GenerateLevel();
    }

    private void OnEnable()
    {
        Actions.OnGameEnd += HandleGameEnd;
        Actions.OnSceneSwitchStart += HandleSceneSwitchStart;
        Actions.OnSceneSwitchEnd+= HandleSceneSwitchEnd;
    }
    private void OnDisable()
    {
        Actions.OnGameEnd -= HandleGameEnd;
        Actions.OnSceneSwitchStart -= HandleSceneSwitchStart;
        Actions.OnSceneSwitchEnd -= HandleSceneSwitchEnd;
    }

    private void HandleSceneSwitchStart()
    {
        Stage = GameStage.Transitioning;
    }
    private void HandleSceneSwitchEnd()
    {
        Stage = GameStage.Playing;
    }

    private void HandleGameEnd(Actions.GameEndState gameEndState)
    {
        Stage = GameStage.Transitioning;
    }

    private void SpawnLight(Level level, int floor, int column)
    {
        float2 lightCoords = new(level.CalculateCoordinatesForColumn(column));
        float lightHeight = BoxLightHeight - floor * Level.DistanceBetweenFloors;
        float lightAngle = (float)(Math.Atan2(lightCoords.x, lightCoords.y) * 180f / Math.PI);

        Vector3 lightPosition = new(lightCoords.x, lightHeight, lightCoords.y);
        Quaternion lightRotation = Quaternion.Euler(0f, lightAngle, 0f);
        Instantiate(boxLightPrefab, lightPosition, lightRotation);
    }

    private void SpawnBox(Level level, int floor, int column, int row)
    {
        // create a new float2 with the double2 from the Level method
        // puts the top box 2 meters further away than the bottom one
        float distanceFromCircle = row * -2f;
        float2 boxCoords = new(level.CalculateCoordinatesForColumn(column, distanceFromCircle));
        float boxHeight = (row == 0 ? BottomBoxHeight : TopBoxHeight) - floor * Level.DistanceBetweenFloors;
        float boxAngle = (float)(Math.Atan2(boxCoords.x, boxCoords.y) * 180f / Math.PI);

        // Instantiate box with calculated coordinates
        Vector3 boxPosition = new(boxCoords.x, boxHeight, boxCoords.y);
        Quaternion boxRotation = Quaternion.Euler(0f, boxAngle, 0f);
        GameObject boxGO = Instantiate(boxPrefab, boxPosition, boxRotation);

        Box box = boxGO.GetComponent<Box>();
        // give box its index
        box.boxIndex = new int3(floor, column, row);
        // find the current column of the current floor. Then get the state of the box based on current row
        box.isOpen = level.Floors[floor][column].GetIsOpenFromIndex(row);
        // give box its contents
        box.contents = level.Floors[floor][column].GetContentsFromIndex(row);
    }

    private void GenerateLevel()
    {
        Level level = levelHandler.GetCurrentLevel();
        for(int floor = 0; floor < level.NumberOfFloors; floor++)
        {
            for (int column = 0; column < level.NumberOfColumns; column++)
            {
                SpawnLight(level, floor, column);
                // row is either bottom or top box
                for (int row = 0; row < 2; row++)
                {
                    SpawnBox(level, floor, column, row);
                }
            }
        }
    }
}
