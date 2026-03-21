using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * All code is original work, with Unity Documentation referenced for identifying Unity
 * specific methods and their correct usage and outputs.
 */
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] elements;
    public bool isPause = false;

    private void Awake()
    {
        Time.timeScale = 1f;
        foreach (GameObject image in elements)
        {
            image.gameObject.SetActive(false);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        PlayerPrefs.DeleteKey("CheckpointID");
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        foreach (GameObject image in elements)
        {
            image.gameObject.SetActive(false);
        }
        isPause = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        foreach (GameObject image in elements)
        {
            image.gameObject.SetActive(true);
        }
        isPause = true;
    }

    public void Load()
    {
        SceneManager.LoadScene(1);
    }
}
