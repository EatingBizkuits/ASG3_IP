using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TaskBook : MonoBehaviour
{
    /// <summary>
    /// ==Phase 1
    /// 3Find fox spirit by the river
    /// 4use fox
    /// 5Enter house
    /// 6Find 4 items
    /// ==cut1
    /// ==Phase 2
    /// Find 4 items
    /// ==cut2/endscene
    /// </summary>
    private int sceneIndex;

    /// <summary>
    /// 0 - 2
    /// </summary>
    private int progressIndex;
    
    public Animator anim;

    /// <summary>
    /// boolean called to show/hide task book
    /// </summary>
    private bool _toggleBook;

    private static readonly int IsShown = Animator.StringToHash("isShown");

    public GameObject bookGraphic;

    public bool isHidden;

    public GameObject[] tasks;
    
    private void Start()
    {
       _toggleBook = anim.GetBool(IsShown);
       sceneIndex = FindObjectOfType<GameManager>().sceneIndex;
       CycleTasks();
    }
    
    /// <summary>
    /// called when menu is called
    /// hide when menu on, show when menu off
    /// true = shown, false = hide;
    /// </summary>
    public void HideBook(bool toHide)
    {
        bookGraphic.SetActive(toHide);
        isHidden = !toHide;
    }
    
    public void ToggleBook()
    {
        if (isHidden) return;
        _toggleBook = !_toggleBook;
        anim.SetBool(IsShown, _toggleBook);
        Debug.Log("ran");
    }

    /// <summary>
    /// external calls for next story part
    /// </summary>
    public void NextTask()
    {
        progressIndex += 1;
        if (progressIndex > 2) return;
        CycleTasks();
    }

    private void CycleTasks()
    {
        var nextTask = progressIndex;
        if (sceneIndex == 4) nextTask = 3;
        foreach (var task in tasks)
        {
            task.SetActive(false);
        }
        tasks[nextTask].SetActive(true);
    }
}
