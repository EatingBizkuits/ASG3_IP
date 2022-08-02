using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyItemIdentifier : MonoBehaviour
{
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
    private ObjectList objectType;

    public string objectTypeText;
    
    private void Start()
    {
        objectTypeText = objectType.ToString();
    }
}