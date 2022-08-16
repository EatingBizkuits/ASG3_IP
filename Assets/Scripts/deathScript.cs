using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class deathScript : MonoBehaviour
{
    /// <summary>
    /// contains player obj
    /// </summary>
    private PlayerMove player;
    private void Start()
    {
        player = FindObjectOfType<PlayerMove>();
    }

    public void Prempt()
    {
        Destroy(FindObjectOfType<MainGhost>().gameObject);
    }
    
    /// <summary>
    /// disables action
    /// </summary>
    public void Death()
    {
        player.GetComponent<PlayerInput>().enabled = false;
    }

    /// <summary>
    /// mutes ambience
    /// </summary>
    public void MuteAudio()
    {
        player.GetComponent<AudioSource>().enabled = false;
    }

    public void MouseRelease()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void Retry()
    {
        FindObjectOfType<GameManager>().ResetScene();
    }

    public void MainMenu()
    {
        FindObjectOfType<GameManager>().sceneIndex = 0;
    }
}
