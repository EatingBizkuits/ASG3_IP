using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Story Related Variables

    // key Items pt 1
    public Dictionary<string, bool> collectedKeyItems = new Dictionary<string, bool>()
    {
        {"item1", false},
        {"item2", false},
        {"item3", false},
        {"item4", false},
    };
    
    // key Items pt 2
    public Dictionary<string, bool> collectedKeyItems2 = new Dictionary<string, bool>()
    {
        {"item5", false},
        {"item6", false},
        {"item7", false},
        {"item8", false},
    };

    #endregion

    #region Scene Related Variables

    /// <summary>
    /// contains scene index
    /// </summary>
    public int sceneIndex;

    /// <summary>
    /// contains player
    /// </summary>
    public GameObject player;

    /// <summary>
    /// contains book menu
    /// </summary>
    private GameObject inGameMenu;

    /// <summary>
    /// tracks state of menu
    /// on = true, off = false;
    /// </summary>
    public bool menuState;

    #endregion

    #region Player Related Variables
    
    #endregion
    
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        inGameMenu = FindObjectOfType<UIelements>().gameObject;
        menuState = false;
        inGameMenu.SetActive(menuState);
        player = FindObjectOfType<PlayerMove>().gameObject;
    }

    // Runs when code starts
    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    /// <summary>
    /// inverse state of menu bool, and set the activity based on bool value
    /// </summary>
    public void ToggleMenu()
    {
        menuState = !menuState;
        inGameMenu.SetActive(menuState);
        if (!menuState)
        {
            ResumeTime();
        }
        else
        {
            PauseTime();
        }
    }

    public void PauseTime()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
    }
    
    public void ResumeTime()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }
}