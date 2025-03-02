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
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
