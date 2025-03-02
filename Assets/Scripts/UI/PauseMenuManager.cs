using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        gameObject.SetActive(false);
    }


    public void OpenPauseMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        if (gameObject.activeSelf)
            Time.timeScale = 0;
        else Time.timeScale = 1;
        
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
