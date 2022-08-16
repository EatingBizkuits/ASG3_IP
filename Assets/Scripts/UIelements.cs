using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIelements : MonoBehaviour
{
    public GameManager gameManager;
    
    public PlayerMove playerCode;

    public Slider sliderAudio;

    public TextMeshProUGUI textAudio;

    public Slider sliderX;

    public TextMeshProUGUI textX;

    public Slider sliderY;

    public TextMeshProUGUI textY;

    public AudioMixer mixer;

    public Animator optionsFlip;
    private static readonly int toMainPage = Animator.StringToHash("toMainPage");

    public void Resume()
    {
        gameManager.ToggleMenu();
    }

    public void FlipPages(bool input)
    {
        Debug.Log(input);
        optionsFlip.SetBool("toMainPage", input);
    }

    public void AdjustSliderX(float sliderValue)
    {
        gameManager.xAxis = sliderValue;
        textX.text = sliderValue.ToString("0.0");
        playerCode.horizontalRotationModifier = sliderValue;
    }
    
    public void AdjustSliderY(float sliderValue)
    {
        gameManager.yAxis = sliderValue;
        textY.text = sliderValue.ToString("0.0");
        playerCode.verticalRotationModifier = sliderValue;
    }

    public void SetLevel(float sliderValue)
    {
        gameManager.volume = sliderValue;
        // value of audio is between -80 and 0 in logarithmic value
        mixer.SetFloat("masterVolume", Mathf.Log10(sliderValue) * 20);
        textAudio.text = (FloatNormalize(sliderValue, 0.0001f, 1f)).ToString("0") + "%";
    }
    private static float FloatNormalize(float input, float minRange, float maxRange)
    {
        var output = ((input - minRange) * 100) / (maxRange - minRange);
        return output;
    }
    
    private static float InverseLogarithm(float input)
    {
        var output = Mathf.Pow(10, input);
        return output;
    }

    public void ToggleInvertAxis(bool input)
    {
        gameManager.invertY = input;
        int change;
        // if invert y axis is true
        if (input)
        {
            change = -1;
        }
        else
        {
            change = 1;
        }
        playerCode.axisDirection = change;
    }

    /// <summary>
    /// toggles bgm on and off via mute
    /// </summary>
    /// <param name="input"></param>
    public void ToggleBGM(bool input)
    {
        gameManager.muted = input;
        input = !input;
        gameManager.player.GetComponent<AudioSource>().enabled = input;
    }


    /// <summary>
    /// activates/deactivates headbob in code
    /// </summary>
    /// <param name="input"></param>
    public void ToggleHeadBob(bool input)
    {
        playerCode.bobbingEffectActive = input;
        gameManager.headBob = input;
    }
    
    /// <summary>
    /// returns to main menu
    /// </summary>
    public void MainMenu()
    {
        FindObjectOfType<GameManager>().sceneIndex = 0;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    private void OnEnable()
    {
        //Debug.Log("eanbled");
        if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
        var playerX = playerCode.horizontalRotationModifier;
        textX.text = playerX.ToString("0.0");
        sliderX.value = playerX;

        var playerY = playerCode.verticalRotationModifier;
        textY.text = playerY.ToString("0.0");
        sliderY.value = playerY;
        
        mixer.GetFloat("masterVolume", out var volume);
        var sliderVal = InverseLogarithm(volume / 20);
        textAudio.text = FloatNormalize(sliderVal, 0.0001f, 1f).ToString("0") + "%";
        
        sliderAudio.value = sliderVal;
    }
}
