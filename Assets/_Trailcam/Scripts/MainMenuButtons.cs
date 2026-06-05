using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void PlayButtonClicked()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void ExitButtonClicked()
    {
        Debug.Log("Exit button clicked.");
        Application.Quit();
        Debug.Log("Game exiting...");
    }
}
