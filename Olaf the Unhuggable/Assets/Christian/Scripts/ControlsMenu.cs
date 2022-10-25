using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    public GameObject MainMenuObj;
    public GameObject OptionsMenuObj;
    public GameObject ControlsMenuObj;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuObj.SetActive(true);
        OptionsMenuObj.SetActive(false);
        ControlsMenuObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
