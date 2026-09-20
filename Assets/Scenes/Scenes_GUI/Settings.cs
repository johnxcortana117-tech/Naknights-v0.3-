using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button back = root.Q<Button>("back");

        back.clicked += () => SceneManager.LoadScene("MainMenu");
    }
}