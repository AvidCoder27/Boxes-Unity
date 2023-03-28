using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{
    private Level currentLevel;
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

        SceneManager.sceneLoaded += (Scene _, LoadSceneMode _) => LoadCurrentLevel();
    }

    public static LevelHandler GetInstance() => Instance;
    public Level GetCurrentLevel() => currentLevel;
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

    private void LoadCurrentLevel()
    {
        string path = "Assets/Levels/level_" + (currentLevelIndex + 1).ToString() + ".json";
        string jsonString = File.ReadAllText(path);
        SerialLevel serialLevel = JsonUtility.FromJson<SerialLevel>(jsonString);
        currentLevel = serialLevel.GetLevel();
    }
}
