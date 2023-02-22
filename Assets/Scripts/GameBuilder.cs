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
    [SerializeField] private GameObject lockSprite;
    [SerializeField] private GameObject ladderSprite;
    [SerializeField] private GameObject inverterSprite;
    [SerializeField] private GameObject columnSprite;
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
            (level.NumberOfFloors - 1) / 2, (level.NumberOfColumns - 1) / 2, 0.5f, 0);

        for (int floor = 0; floor < level.NumberOfFloors; floor++)
        {
            SpawnViewingHole(floor, viewingHoleSegmentPrefab);
            for (int column = 0; column < level.NumberOfColumns; column++)
            {
                SpawnColumnItems(level, floor, column);
                SpawnColumnSprite(floor, column);
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

    private void SpawnColumnSprite(int floor, int column)
    {
        Transform col = Instantiate(columnSprite, preparationMap).transform;
        col.localPosition = SpritePosition(floor, column, 0, 11);
    }

    private void SpawnBoxSprites(Level level, int floor, int column, int row)
    {
        BoxStruct box = level.Floors[floor][column][row];
        Transform boxTransform = Instantiate(box.IsOpen ? boxOpenedSprite : boxClosedSprite,
            preparationMap, false).transform;
        boxTransform.localPosition = SpritePosition(floor, column, row, 10);

        GameObject contentsPrefab = box.Contents switch
        {
            Box.Contents.Key => keySprite,
            Box.Contents.Star => starSprite,
            Box.Contents.Inverter => inverterSprite,
            Box.Contents.Ladder => ladderSprite,
            _ => null
        };

        // Spawn contents
        if (contentsPrefab != null)
        {
            GameObject contents = Instantiate(contentsPrefab, boxTransform, false);
            contents.transform.position += Vector3.back * 1;

            if (box.Contents == Box.Contents.Key)
            {
                contents.GetComponentInChildren<SpriteRenderer>().color = Key.GetColorOfKeyColor(box.KeyColor);
            }
        }

        // Spawn lock (if the box has one)
        if (box.LockColor != Key.Colors.Undefined)
        {

        }

    }

    private static Vector3 SpritePosition(float floor, float column, float row, float cameraDistance)
    {
        return new Vector3(column * 5f, (row * 3.5f) + (floor * -8.5f), cameraDistance);
    }
}
