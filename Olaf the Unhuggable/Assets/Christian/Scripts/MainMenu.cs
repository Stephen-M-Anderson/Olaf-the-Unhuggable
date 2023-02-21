using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuObj; //The main menu with all its UI objects and shit
    public GameObject OptionsMenuObj; //The options menu with all its UI objects and shit
    public GameObject ControlsMenuObj; //The controls menu with all its UI objects and shit

    private void Start()
    {

    }

    //This function starts the fucking game. I have it load into Stephen's cool as fuck test level for now. Likely will change that later
    public void PlayGame ()
    {
        Debug.Log("Get the fuck in there and start the goddamn game");
        SceneManager.LoadScene(1);
    }

    //This function quits the game. Who'd have fuckin thunk it?
    public void QuitGame ()
    {
        Debug.Log("We are in fact, quitting the fucking game right now my dude");
        Application.Quit();
    }
}
