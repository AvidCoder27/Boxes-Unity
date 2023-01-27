using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{
    List<Level> levels;
    [SerializeField] int currentLevelIndex;
    [SerializeField] int maxLevel;

    static LevelHandler Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
             Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.

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
        } else
        {
            SceneManager.LoadScene(0);
        }
    }

    private void CreateLevelsList()
    {
        levels = new List<Level>
        {
            new Level(
                new List<List<Column>>()
                {
                    new List<Column>()
                    {
                        new Column(false, false),
                        new Column(false, false, Box.Contents.None, Box.Contents.Star),
                        new Column(true, true),
                        new Column(false, true),
                        new Column(true, false)
                    }
                }
            ),
            new Level(
                new List<List<Column>>()
                {
                    new List<Column>()
                    {
                        new Column(true, false),
                        new Column(false, true),
                        new Column(false, false, Box.Contents.Star),
                        new Column(false, true),
                        new Column(true, false),
                        new Column(false, false),
                        new Column(true, false),
                        new Column(false, true),
                        new Column(false, false),
                    }
                }
            ),
            // made up level
            new Level(
                new List<List<Column>>()
                {
                    new List<Column>()
                    {
                        new Column(false, false),
                        new Column(false, false, Box.Contents.None, Box.Contents.Star),
                        new Column(true, true),
                        new Column(false, true),
                        new Column(true, true, Box.Contents.None, Box.Contents.Ladder)
                    },
                    new List<Column>()
                    {
                        new Column(true, true),
                        new Column(false, true),
                        new Column(false, false),
                        new Column(true, false),
                        new Column(false, true)
                    }
                }
            )
        };
    }

}
