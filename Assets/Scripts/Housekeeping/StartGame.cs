using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public string nextScene;

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            PlayerPrefs.SetInt("currentScene", 0);
            PlayerPrefs.SetInt("score", 0);
            SceneManager.LoadScene(nextScene);
        });
    }
}
