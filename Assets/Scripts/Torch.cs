/*
 * Author: Wong Chao Hao
 * Date Created: 31/7/2022
 * Description: Handles Torch Related Interactions
 * Last Edit: 2/8/2022
 */

using UnityEngine;

public class Torch : MonoBehaviour
{
    public bool lightStatus;
    public GameObject torchLight;

    public void ToggleLight()
    {
        GetComponent<AudioSource>().Play();
        if (lightStatus)
        {
            lightStatus = !lightStatus;
            torchLight.SetActive(lightStatus);
        }
        else
        {
            lightStatus = !lightStatus;
            torchLight.SetActive(lightStatus);
        }
    }
}