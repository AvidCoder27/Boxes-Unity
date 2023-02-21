using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{
    private List<Level> levels;
    [SerializeField] private int currentLevelIndex;
    [SerializeField] private int maxLevel;
    private static LevelHandler Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.
        }

        CreateLevelsList();
    }

    public static LevelHandler GetInstance() => Instance;
    public Level GetCurrentLevel() => levels[currentLevelIndex];
    private void OnEnable() => Actions.OnNextLevel += HandleNextLevel;
    private void OnDisable() => Actions.OnNextLevel -= HandleNextLevel;

    private void HandleNextLevel(Actions.GameEndState gameEndState)
    {
        switch (gameEndState)
        {
            case Actions.GameEndState.Win:
                currentLevelIndex++;
                break;
            case Actions.GameEndState.Lose:
                break;
        }

        if (currentLevelIndex == maxLevel)
        {
            Debug.Log("Game beaten");
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    private void CreateLevelsList()
    {
        levels = new List<Level>
        {
            new Level(
                new List<List<BoxStruct[]>>()
                {
                    new List<BoxStruct[]>()
                    {
                        MakeCol(false, false),
                        MakeCol(false, Box.Contents.None, false, Box.Contents.Star),
                        MakeCol(true, true),
                        MakeCol(false, true),
                        MakeCol(true, false),
                    }
                }
            ),
            new Level(
                new List<List<BoxStruct[]>>()
                {
                    new List<BoxStruct[]>()
                    {
                        MakeCol(true, false),
                        MakeCol(false, true),
                        MakeCol(false, Box.Contents.Star, false, Box.Contents.None),
                        MakeCol(false, true),
                        MakeCol(true, false),
                        MakeCol(false, false),
                        MakeCol(true, false),
                        MakeCol(false, true),
                        MakeCol(false, false),
                    }
                }
            ),
            new Level(
                new List<List<BoxStruct[]>>()
                {
                    new List<BoxStruct[]>()
                    {
                        MakeCol(false, false),
                        MakeCol(false, Box.Contents.None, false, Box.Contents.Star),
                        MakeCol(true, true),
                        MakeCol(false, true),
                        MakeCol(true, Box.Contents.Ladder, true, Box.Contents.None),
                    },
                    new List<BoxStruct[]>
                    {
                        MakeCol(true, true),
                        MakeCol(false, true),
                        MakeCol(false, false),
                        MakeCol(true, false),
                        MakeCol(false, true)
                    }
                }
            ),
            new Level(
                new List<List<BoxStruct[]>>()
                {
                    new List<BoxStruct[]>()
                    {
                        MakeCol(new BoxStruct(true, Box.Contents.Key, Key.Colors.Green, Key.Colors.Undefined), new BoxStruct(true)),
                        MakeCol(new BoxStruct(false, Box.Contents.Key, Key.Colors.Red, Key.Colors.Undefined), new BoxStruct(true)),
                        MakeCol(false, Box.Contents.Ladder, false, Box.Contents.None),
                        MakeCol(false, Box.Contents.Star, true, Box.Contents.Star),
                        MakeCol(true, false),
                        MakeCol(false, false),
                        MakeCol(true, false),
                        MakeCol(new BoxStruct(true, Box.Contents.Key, Key.Colors.Gold, Key.Colors.Undefined), new BoxStruct(true)),
                        MakeCol(new BoxStruct(true, Box.Contents.Key, Key.Colors.Purple, Key.Colors.Undefined), new BoxStruct(true, Box.Contents.Key, Key.Colors.Purple, Key.Colors.Undefined)),
                    },
                    new List<BoxStruct[]>
                    {
                        MakeCol(true, false),
                        MakeCol(false, true),
                        MakeCol(false, Box.Contents.Ladder, false, Box.Contents.Inverter),
                        MakeCol(false, Box.Contents.None, true, Box.Contents.Star),
                        MakeCol(new BoxStruct(true), new BoxStruct(false, Box.Contents.Star, Key.Colors.Undefined, Key.Colors.Red)),
                        MakeCol(false, false),
                        MakeCol(true, false),
                        MakeCol(false, true),
                        MakeCol(false, false)
                    },
                    new List<BoxStruct[]>
                    {
                        MakeCol(true, true),
                        MakeCol(true, true),
                        MakeCol(true, true),
                        MakeCol(true, true),
                        MakeCol(true, true),
                        MakeCol(true, true),
                        MakeCol(true, true),
                        MakeCol(true, false),
                        MakeCol(true, false),
                    }
                }
            ),
        };
    }

    private BoxStruct[] MakeCol(bool bottomOpen, Box.Contents bottomContents, bool topOpen, Box.Contents topContents)
    {
        return MakeCol(new BoxStruct(bottomOpen, bottomContents), new BoxStruct(topOpen, topContents));
    }

    private BoxStruct[] MakeCol(bool bottomOpen, bool topOpen)
    {
        return MakeCol(new BoxStruct(bottomOpen), new BoxStruct(topOpen));
    }

    private BoxStruct[] MakeCol(BoxStruct bottom, BoxStruct top)
    {
        return new BoxStruct[] { bottom, top };
    }
}
