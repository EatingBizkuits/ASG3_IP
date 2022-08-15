/*
 * Author: Lucas Dominic Tiu
 * Date: 6/21/2022
 * Description: I3E_STLD_Assg2 Menu Control Script
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Variables with headers
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Volume Settings")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 0.5f;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text controllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private float defaultSen = 0.5f;
    public float mainControllerSen = 0.5f;

    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertYToggle = null;

    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private float defaultBrightness = 1;

    [Space(10)]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("Levels to load")]
    public string _newGameLevel;
    //private string levelToLoad;
    //[SerializeField] private GameObject loadGameDialogue = null;

    [Header("Resolution Dropdown")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    /// <summary>
    /// Set resolutions
    /// Clear default dropdown options
    /// List different options
    /// Set index within the list to get value of the index within the length of the array and get the number of resolutions
    /// Search through the length of the array, get the values of the width and the height
    /// Check if the resolutions found is same as the resolution of the screen
    /// Set to current chosen resolution
    /// </summary>
    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
    }

    /// <summary>
    /// Look through all the resolutions found and places the value that has been selected within the parameter
    /// Sets resolution with the 3 parameters required
    /// </summary>
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    /// <summary>
    /// New game button to load into new game scene
    /// </summary>
    public void NewGameDialogueYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    /// <summary>
    /// Load game button to load into saved scene
    /// Saved scene will be stored within Unity's built-in data storing system PlayerPrefs to save data
    /// </summary>
    /*public void LoadGameDialogueYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            loadGameDialogue.SetActive(false);
        }
    }*/

    /// <summary>
    /// Exit button to quit application once the project is built
    /// </summary>
    public void ExitButton()
    {
        Application.Quit();
    }

    /// <summary>
    /// Adjust volume settings through AudioListener 
    /// Also adjust text within settings so that it moves with the slider
    /// </summary>
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    /// <summary>
    /// Apply the settings and save it to PlayerPrefs
    /// </summary>
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    /// <summary>
    /// Adjust Player's control sensitivity
    /// Adjusts text within settings to match slider
    /// </summary>
    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = sensitivity;
        controllerSenTextValue.text = sensitivity.ToString("0.00");
    }

    /// <summary>
    /// Apply the settings and save it to PlayerPrefs
    /// </summary>
    public void GameplayApply()
    {
        ///toggle between inverting Y-Axis sensitivity depending on whether toggle isOn or not On
        if (invertYToggle.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
            //invert Y
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
            //not invert Y
        }

        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
        StartCoroutine(ConfirmationBox());
    }

    /// <summary>
    /// Adjust display brightness
    /// Adjusts text within settings to match slider
    /// </summary>
    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
    }

    /// <summary>
    /// setting up the parameter and controlling the variable to change within Unity settings
    /// </summary>
    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;
    }

    /// <summary>
    /// setting up the parameter and controlling the variable to change within Unity settings
    /// </summary>
    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    /// <summary>
    /// Apply the graphics settings and save it to PlayerPrefs
    /// </summary>
    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);
        //change your brightness with your own method

        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", (_isFullScreen ? 1 : 0));
        Screen.fullScreen = _isFullScreen;

        StartCoroutine(ConfirmationBox());
    }

    /// <summary>
    /// Reset Function for the different menu reset buttons
    /// Upon clicking the reset button will change all values to the default values
    /// Changes UI and actual settings
    /// </summary>
    public void ResetButton(string MenuType)
    {
        if (MenuType == "Graphics")
        {
            //reset brightness level
            brightnessSlider.value = defaultBrightness;
            brightnessTextValue.text = defaultBrightness.ToString("0.0");

            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            fullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            //takes the highest resolution the monitor will display on default
            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }

        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            controllerSenTextValue.text = defaultSen.ToString("0");
            controllerSenSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            invertYToggle.isOn = false;
            GameplayApply();
        }
    }

    /// <summary>
    /// Confirmation box that pops up for 2 seconds to indicate that the settings have been saved
    /// </summary>
    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}

