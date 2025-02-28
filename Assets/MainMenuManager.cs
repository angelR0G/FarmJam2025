using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject mainMenuTab;
    [SerializeField] private GameObject creditsTab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Scenes/Map");
        if (GameManager.Instance != null) { }
    }

    public void OpenCredits()
    {
        mainMenuTab.active = false;
        creditsTab.active = true;
    }
    public void CloseCredits()
    {
        mainMenuTab.active = true;
        creditsTab.active = false;
    }
    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
