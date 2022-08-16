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

    /// <summary>
    /// contains TaskBook script
    /// </summary>
    public TaskBook taskbook;

    /// <summary>
    /// Stores the Active GameManager.
    /// </summary>
    public static GameManager instance;

    
    #endregion

    #region Player Related Variables
    
    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            //Set the GameManager to not to be destroyed when scenes are loaded
            DontDestroyOnLoad(gameObject);
            //Set myself as the instance
            instance = this;
        }
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        // turns of UI once stored
        // if playable scene, start up all variables
        if (sceneIndex is 2 or 4)
        {
            inGameMenu = FindObjectOfType<UIelements>().gameObject;
            menuState = false;
            inGameMenu.SetActive(menuState);
            player = FindObjectOfType<PlayerMove>().gameObject;
            taskbook = FindObjectOfType<TaskBook>();
            // sets players position to myself
            player.transform.position = transform.position;
        }
    }

    void Start()
    {
        if (sceneIndex == 0)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void Update()
    {
        ChangeScene();
    }
    
    private void ChangeScene()
    {
        if (SceneManager.GetActiveScene().buildIndex != sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(sceneIndex);
    }
    
    /// <summary>
    /// inverse state of menu bool, and set the activity based on bool value
    /// </summary>
    public void ToggleMenu()
    {
        menuState = !menuState;
        inGameMenu.SetActive(menuState);
        taskbook.HideBook(!menuState);
        
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