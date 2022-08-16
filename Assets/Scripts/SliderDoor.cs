using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderDoor : MonoBehaviour
{
    [SerializeField]
    private Animator _doorNimator;

    public AudioSource _soundOpen;
    
    public AudioSource _soundClose;
    private void Start()
    {
        GetComponent<Animator>();
    }

    private void PlayDoorOpen()
    {
        
    }

    private void PlayDoorClose()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
}
