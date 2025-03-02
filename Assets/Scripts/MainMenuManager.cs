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
        if (GameManager.Instance != null) { GameManager.Instance.ResetTimeToFirstDay(); }
        Destroy(GameManager.Instance.gameObject);
    }

    public void OpenCredits()
    {
        mainMenuTab.SetActive(false);
        creditsTab.SetActive(true);
    }
    public void CloseCredits()
    {
        mainMenuTab.SetActive(true);
        creditsTab.SetActive(false);
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
