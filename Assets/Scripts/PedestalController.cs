using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PedestalController : MonoBehaviour
{
    public GameObject promptUI;

    public GameObject interactableUI;

    private enum ObjectList
    {
        item1,
        item2,
        item3,
        item4,
        item5,
        item6,
        item7,
        item8
    }

    [SerializeField] 
    private ObjectList targetItem;

    public string targetItemText;

    private void Start()
    {
        targetItemText = targetItem.ToString();
    }
    
    /// <summary>
    /// hides prompting UI and replaces it with the interactable ui when obj is picked up
    /// </summary>
    public void swapUI()
    {
        promptUI.SetActive(false);
        interactableUI.SetActive(true);
    }
}
