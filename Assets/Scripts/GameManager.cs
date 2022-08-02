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
    
    #endregion
    
    // Runs when code starts
    void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}