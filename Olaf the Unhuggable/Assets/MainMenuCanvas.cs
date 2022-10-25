using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    public GameObject MainMenuObj; //The main menu with all its UI objects and shit
    public GameObject OptionsMenuObj; //The options menu with all its UI objects and shit
    public GameObject ControlsMenuObj; //The controls menu with all its UI objects and shit

    private void Start()
    {
        //I honestly just put this here for me in case I accidentally left other menus open. Shouldn't affect the player tbh
        MainMenuObj.SetActive(true);
        OptionsMenuObj.SetActive(false);
        ControlsMenuObj.SetActive(false);
    }
}
