using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button play = root.Q<Button>("play");
        Button manual = root.Q<Button>("manual");
        Button leaderboards = root.Q<Button>("leaderboards");
        Button settings = root.Q<Button>("settings");
        Button exit = root.Q<Button>("exit");

        play.clicked += () => SceneManager.LoadScene("Game");
        manual.clicked += () => SceneManager.LoadScene("ManualMenu");
        leaderboards.clicked += () => SceneManager.LoadScene("LeaderboardsMenu");
        settings.clicked += () => SceneManager.LoadScene("SettingsMenu");
        exit.clicked += () => Application.Quit();
    }
}