using Unity.Mathematics;
using UnityEngine;

public class GameBuilder : MonoBehaviour
{
    [SerializeField] private Transform playingCharacter;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject columnItemsPrefab;
    [SerializeField] private GameObject hideDuringPlay;
    [SerializeField] private GameObject viewingHoleSegmentPrefab;
    [SerializeField] private GameObject viewingHoleBottomPrefab;
    [SerializeField] private Transform preparationMap;
    [SerializeField] private GameObject boxClosedSprite;
    [SerializeField] private GameObject boxOpenedSprite;
    [SerializeField] private GameObject starSprite;
    [SerializeField] private GameObject keySprite;
    private Transform prepMapCamera;
    private LevelHandler levelHandler;

    [SerializeField] private float BottomBoxHeight, TopBoxHeight;

    private void Awake()
    {
        levelHandler = LevelHandler.GetInstance();
        prepMapCamera = preparationMap.Find("Map Camera");
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

    private void SpawnColumnItems(Level level, int floor, int column)
    {
        float2 coords = level.CalculateCoordinatesForColumn(column);
        float height = -(floor * Level.DistanceBetweenFloors);
        float angle = Mathf.Atan2(coords.x, coords.y) * 180f / Mathf.PI;

        Vector3 position = new(coords.x, height, coords.y);
        Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
        Instantiate(columnItemsPrefab, position, rotation);
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
        prepMapCamera.localPosition = SpritePosition(
            (level.NumberOfFloors - 1) / 2, (level.NumberOfColumns - 1) / 2, 0.5f, -15f);

        for (int floor = 0; floor < level.NumberOfFloors; floor++)
        {
            SpawnViewingHole(floor, viewingHoleSegmentPrefab);
            for (int column = 0; column < level.NumberOfColumns; column++)
            {
                SpawnColumnItems(level, floor, column);
                // row is either bottom or top box
                for (int row = 0; row < 2; row++)
                {
                    SpawnBox(level, floor, column, row);
                    SpawnBoxSprites(level, floor, column, row);
                }
            }
        }
        SpawnViewingHole(level.NumberOfFloors, viewingHoleBottomPrefab);
    }

    private void SpawnBoxSprites(Level level, int floor, int column, int row)
    {
        BoxStruct box = level.Floors[floor][column][row];
        Transform boxTransform = Instantiate(box.IsOpen ? boxOpenedSprite : boxClosedSprite,
            preparationMap, false).transform;
        boxTransform.localPosition = SpritePosition(floor, column, row, 0);

        GameObject chosenPrefab = box.Contents switch
        {
            Box.Contents.Star => starSprite,
            Box.Contents.Key => keySprite,
            _ => null
        };

        if (chosenPrefab != null)
        {
            Transform t = Instantiate(chosenPrefab, preparationMap, false).transform;
            t.localPosition = SpritePosition(floor, column, row, -5);
        }

    }

    private static Vector3 SpritePosition(float floor, float column, float row, float z)
    {
        return new Vector3(column * 5f, (row * 3.5f) + (floor * -8.5f), z);
    }
}
