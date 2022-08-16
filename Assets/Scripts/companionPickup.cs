using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class companionPickup : MonoBehaviour
{
    public GameObject fox;
    private Transform player;

    private void Start()
    {
        player = FindObjectOfType<PlayerMove>().transform;
    }
    
    public void PickUp()
    {
        // moves to next task
        FindObjectOfType<TaskBook>().NextTask();
        // spawns fox
        Instantiate(fox, player.position, Quaternion.identity);
        // toggle player
        player.GetComponent<PlayerMove>().hasAI = true;
        // destroy this task object
        Destroy(gameObject);
    }
}
