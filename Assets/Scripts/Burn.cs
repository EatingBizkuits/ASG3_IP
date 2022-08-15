using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour
{
    public Material mat;

    public GameObject fireParticles;
    
    [Range(0, 2f)] 
    public float delay;

    public float burnTime = 4f;
    
    public bool reset;
    
    public bool isActive;
    
    private float _timeElapsed;

    private float _alphaValue;

    private static readonly int ClipThreshold = Shader.PropertyToID("_ClipThreshold");

    
    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        mat.SetFloat(ClipThreshold, _alphaValue);
        CallIncinerate();
    }

    private void Update()
    {
        if (!reset) return;

        _alphaValue = 0;
        _timeElapsed = 0;
        isActive = false;
        mat.SetFloat(ClipThreshold, _alphaValue);
        fireParticles.SetActive(isActive);
        reset = false;
        CallIncinerate();
    }

    private void CallIncinerate()
    {
        const string coroutineName = "Incinerate";
        StopCoroutine(coroutineName);
        StartCoroutine(coroutineName);
    }
    
    private IEnumerator Incinerate()
    {
        isActive = true;
        fireParticles.SetActive(isActive);
        yield return new WaitForSeconds(delay);
        while (_timeElapsed < burnTime)
        {
            //Debug.Log(_timeElapsed);
            yield return null;
            _timeElapsed += Time.deltaTime;
            _alphaValue = (_timeElapsed / burnTime) * 1.1f;
            mat.SetFloat(ClipThreshold, _alphaValue);           
        }
    }
}
