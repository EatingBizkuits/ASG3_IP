/*
 * Author: Lucas Dominic Tiu
 * Date: 14/8/2022
 * Description: IP Proj Main Menu Controller Script
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


    [Header("Start Game")]
    public string _newGameLevel;


    private void Start()
    {
      
    }


    /// <summary>
    /// New game button to load into new game scene
    /// </summary>
    public void NewGameDialogueYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }


    /// <summary>
    /// Exit button to quit application once the project is built
    /// </summary>
    public void ExitButton()
    {
        Application.Quit();
    }


}

