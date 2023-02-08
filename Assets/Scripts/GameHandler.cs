using System;
using Unity.Mathematics;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public enum GameStage { Preparation, Playing, Transitioning }

    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject boxLightPrefab;
    [SerializeField] private GameObject viewingHoleSegmentPrefab;
    [SerializeField] private GameObject viewingHoleBottomPrefab;
    [SerializeField] private GameObject hideDuringPlay;
    [SerializeField] private Transform playingCharacter;
    private LevelHandler levelHandler;

    public GameStage Stage { get; private set; }
    [SerializeField] float BottomBoxHeight, TopBoxHeight, BoxLightHeight;

    private void Awake()
    {
        levelHandler = LevelHandler.GetInstance();

        Stage = GameStage.Preparation;
    }

    private void Start()
    {
        hideDuringPlay.SetActive(true);
        GenerateLevel();
    }

    private void OnEnable()
    {
        Actions.OnGameEnd += HandleGameEnd;
        Actions.OnSceneSwitchStart += HandleSceneSwitchStart;
        Actions.OnSceneSwitchSetup += HandleSceneSwitchSetup;
        Actions.OnSceneSwitchEnd += HandleSceneSwitchEnd;
    }
    private void OnDisable()
    {
        Actions.OnGameEnd -= HandleGameEnd;
        Actions.OnSceneSwitchStart -= HandleSceneSwitchStart;
        Actions.OnSceneSwitchSetup -= HandleSceneSwitchSetup;
        Actions.OnSceneSwitchEnd -= HandleSceneSwitchEnd;
    }

    private void HandleSceneSwitchStart()
    {
        Stage = GameStage.Transitioning;
    }
    private void HandleSceneSwitchSetup()
    {
        hideDuringPlay.SetActive(false);
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
        box.SetRefs(playingCharacter);
        box.CopyInAttributes(level.Floors[floor][column][row], new int3(floor, column, row), true);
    }

    private void SpawnViewingHole(int floor, GameObject prefab)
    {
        Quaternion rot = floor % 2 == 0 ? Quaternion.identity : Quaternion.Euler(Vector3.up * 180);
        Instantiate(prefab,
            -hideDuringPlay.transform.position + (-floor * Level.DistanceBetweenFloors + 5) * Vector3.up,
            rot, hideDuringPlay.transform);
    }

    private void GenerateLevel()
    {
        Level level = levelHandler.GetCurrentLevel();
        for(int floor = 0; floor < level.NumberOfFloors; floor++)
        {
            SpawnViewingHole(floor, viewingHoleSegmentPrefab);
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
        SpawnViewingHole(level.NumberOfFloors, viewingHoleBottomPrefab);
    }
}
