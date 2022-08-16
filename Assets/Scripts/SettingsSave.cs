using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSave : MonoBehaviour
{
    private GameManager manager;

    private void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }
    
    public void SenseX(float input)
    {
        manager.xAxis = input;
    }
    
    public void SenseY(float input)
    {
        manager.yAxis = input;
    }
    
    public void Volume(float input)
    {
        manager.volume = input;
    }
    
    public void MuteBGM(bool input)
    {
        manager.muted = input;
    }
    
    public void InvertY(bool input)
    {
        manager.invertY = input;
    }
    
    public void HeadBob(bool input)
    {
        manager.headBob = input;
    }
}
