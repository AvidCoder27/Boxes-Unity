using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    private VisualElement _root;

    private Button _resume;
    private Button _options;
    private Button _quit;

    private void Awake()
    {
        _root = _pauseMenu.GetComponent<UIDocument>().rootVisualElement;

        _resume = _root.Query<Button>("Resume");
        _options = _root.Query<Button>("Options");
        _quit = _root.Query<Button>("Quit");

        _pauseMenu.SetActive(false);
    }

    private void OnEnable()
    {
        _resume.clicked += Resume;
        _resume.RegisterCallback<ClickEvent>(OnResumeClicked);
    }

    public void OnResumeClicked(ClickEvent evt)
    {
        Debug.Log("resume button clicked");
        Resume();
    }

    public void Pause()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Actions.OnGamePause?.Invoke();
    }

    public void Resume()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Actions.OnGameResume?.Invoke();
    }
}
