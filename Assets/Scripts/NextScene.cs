using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextScene : MonoBehaviour
{
    private void OnEnable()
    {
        FindObjectOfType<GameManager>().sceneIndex += 1;
    }
}
