using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject gameUI;
    public GameObject optionsMenuUI;
    public GameObject controlsMenuUI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            if (GameIsPaused)
            {
                Debug.Log("GameIsPaused = " + GameIsPaused + " and I am now going to resume the game.");
                Resume();
            } 
            else
            {
                Debug.Log("GameIsPaused = " + GameIsPaused + " and I am now going to pause the game.");
                Pause();
            }
        }
        
    }

    public void Resume ()
    {
        pauseMenuUI.SetActive(false);
        gameUI.SetActive(true);
        optionsMenuUI.SetActive(false);
        controlsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause ()
    {
        //Add something here to turn off the crosshairs and maybe turn on the mouse?

        pauseMenuUI.SetActive(true);
        gameUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Debug.Log("Da Menu iz loading my dude...................................................................................................................");
        Resume();
        SceneManager.LoadScene(0);
    }

    public void OptionsMenu()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("The game do be quitting tho");
        Application.Quit();
    }
}
