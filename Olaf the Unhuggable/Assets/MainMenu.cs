using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuObj;
    public GameObject OptionsMenuObj;
    public GameObject ControlsMenuObj;

    private void Start()
    {
        MainMenuObj.SetActive(true);
        OptionsMenuObj.SetActive(false);
        ControlsMenuObj.SetActive(false);
    }

    public void PlayGame ()
    {
        SceneManager.LoadScene(1);
    }
}
