using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReturntoMainMenu : MonoBehaviour
{
    private void OnEnable()
    {
        FindObjectOfType<GameManager>().sceneIndex = 0;
    }
}
