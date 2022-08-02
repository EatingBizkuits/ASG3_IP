using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UITracking : MonoBehaviour
{
    public Camera playerCamera;

    // code to set the player's camera as the default camera
    private void Start()
    {
        // gets player camera
        playerCamera = FindObjectOfType<PlayerMove>().playerCam.GetComponent<Camera>();
        // gets canvas assigned camera
        var canvasCam = GetComponent<Canvas>().worldCamera;
        // if canvas assigned camera is null or not the playercamera, replaces it with the player camera
        if (canvasCam == null || canvasCam != playerCamera)
        {
            GetComponent<Canvas>().worldCamera = playerCamera;
        }
    }
    // Canvas faces the player at all times
    private void Update()
    {
        transform.LookAt(transform.position + playerCamera.transform.rotation * Vector3.forward, playerCamera.transform.rotation * Vector3.up);
    }
}