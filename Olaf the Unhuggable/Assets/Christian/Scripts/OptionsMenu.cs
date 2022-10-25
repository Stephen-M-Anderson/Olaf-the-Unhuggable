using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer; //Referencing the only Audio Mixer component we're using so far. Since it's the only one that means it's the master volume.
    string whyDidIMakeAVariableForASillyDebugMessage; //Ignore my foolish shenanigans
    Resolution[] resolutions; //An array of resolutions, we'll store all the resolutions that Unity detects your monitor capable of in this baby.
    public TMPro.TMP_Dropdown resolutionDropdown; //We'll put the dropdown menu for the Screen Resolutions here. The reason it's not just a Dropdown value is because we used TextMeshPro for this dropdown menu
    

    private void Start()
    {
        resolutions = Screen.resolutions; //We just took every resolution your monitor is capable of and fucking put it in an array. YOU CANNOT STOP THIS PROCESS IT IS ALREADY DONE

        resolutionDropdown.ClearOptions(); //Every time we boot the game  we want this resolution dropdown to clear itself so that we don't have junk values in it from previous sessions

        List<string> options = new List<string>(); //Establish a new List of strings. We'll be inputting our resolutions into this in order to actually put them into the dropdown object as text

        int currentResolutionIndex = 0; 
        // This for loop traverses our resolutions array and outputs it as a string in the format of "width x height" then adds it as an option in our dropdown. It also checks to see if the current screen resolution is
        // the same as the one currently being traversed and if so it makes that one the value currently selected when you open the menu. 
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    //This function takes the volume float that is adjusted by the volume slider and correlates it to the volume of our Audio Mixer
    public void SetVolume (float volume)
    {
        Debug.Log("Current fucking volume is: " + volume);
        audioMixer.SetFloat("masterVolume", volume);
    }

    //Each of the quality options are tied to an int value, this function interprets those ints and translates them to the actual quality level of the game.
    public void SetQuality (int qualityIndex)
    {
        if (qualityIndex == 2)
        {
            whyDidIMakeAVariableForASillyDebugMessage = "High ( ͡° ͜ʖ ͡°)";
        } else if (qualityIndex == 1)
        {
            whyDidIMakeAVariableForASillyDebugMessage = "Medium =^|";
        } else if (qualityIndex == 0)
        {
            whyDidIMakeAVariableForASillyDebugMessage = "Low =^[";
        } else
        {
            whyDidIMakeAVariableForASillyDebugMessage = "Uhhhhhhhhhhhhhhhhhhhhhhhhh what the fuck that shouldn't happen?";
        }
        Debug.Log("Hey fucko, the current quality level is: " + whyDidIMakeAVariableForASillyDebugMessage);
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    //The fullscreen toggle is associated with a bool, this takes that bool and allows it to dictate whether or not the game is in fullscreen
    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    //We take the resolution value selected in our dropdown and allow it to correlate to actual screen resolution
    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
