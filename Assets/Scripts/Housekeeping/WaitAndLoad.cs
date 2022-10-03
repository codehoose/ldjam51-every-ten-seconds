using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitAndLoad : MonoBehaviour
{
    public float waitTime = 5f;
    public string nextScene;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(nextScene);
    }
}
