using Unity.Mathematics;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject boxLightPrefab;
    [SerializeField] private GameObject viewingHoleSegmentPrefab;
    [SerializeField] private GameObject viewingHoleBottomPrefab;
    [SerializeField] private GameObject hideDuringPlay;
    [SerializeField] private Transform playingCharacter;
    private LevelHandler levelHandler;

    [SerializeField] private float BottomBoxHeight, TopBoxHeight, BoxLightHeight;

    private void Awake()
    {
        levelHandler = LevelHandler.GetInstance();
    }

    private void Start()
    {
        hideDuringPlay.SetActive(true);
        GenerateLevel();
    }

    private void OnEnable()
    {
        Actions.OnSceneSwitchSetup += HandleSceneSwitchSetup;
    }
    private void OnDisable()
    {
        Actions.OnSceneSwitchSetup -= HandleSceneSwitchSetup;
    }


    private void HandleSceneSwitchSetup()
    {
        Destroy(hideDuringPlay);
    }

    private void SpawnLight(Level level, int floor, int column)
    {
        float2 lightCoords = level.CalculateCoordinatesForColumn(column);
        float lightHeight = BoxLightHeight - (floor * Level.DistanceBetweenFloors);
        float lightAngle = Mathf.Atan2(lightCoords.x, lightCoords.y) * 180f / Mathf.PI;

        Vector3 lightPosition = new(lightCoords.x, lightHeight, lightCoords.y);
        Quaternion lightRotation = Quaternion.Euler(0f, lightAngle, 0f);
        Instantiate(boxLightPrefab, lightPosition, lightRotation);
    }

    private void SpawnBox(Level level, int floor, int column, int row)
    {
        // puts the top box 2 meters further away than the bottom one
        float distanceFromCircle = row * -2f;
        float2 boxCoords = level.CalculateCoordinatesForColumn(column, distanceFromCircle);
        float boxHeight = (row == 0 ? BottomBoxHeight : TopBoxHeight) - (floor * Level.DistanceBetweenFloors);
        float boxAngle = Mathf.Atan2(boxCoords.x, boxCoords.y) * 180f / Mathf.PI;

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
            -hideDuringPlay.transform.position + (((-floor * Level.DistanceBetweenFloors) + 5) * Vector3.up),
            rot, hideDuringPlay.transform);
    }

    private void GenerateLevel()
    {
        Level level = levelHandler.GetCurrentLevel();
        for (int floor = 0; floor < level.NumberOfFloors; floor++)
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
