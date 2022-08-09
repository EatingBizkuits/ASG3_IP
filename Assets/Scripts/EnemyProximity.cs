using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyProximity : MonoBehaviour
{
    private Transform _player;
    
    public Transform mainBody;

    public bool withinRange;

    public Image peripherals;

    [SerializeField] private float _maxRange;

    private float maxAlphaValue;

    private float alphaValue;

    private void Start()
    {
        _player = FindObjectOfType<PlayerMove>().transform;
        _maxRange = GetComponent<SphereCollider>().radius;
    }

    private void Update()
    {
        DistanceMeasure();
    }

    private void DistanceMeasure()
    {
        if (!withinRange) return;
        var percentage = Vector3.Distance(mainBody.position, _player.position) / _maxRange;

        if (percentage > 1) percentage = 1;
        percentage *= 0.5f;
        percentage += 1;
        peripherals.rectTransform.localScale = new Vector3(percentage, percentage, 1);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        Debug.Log("within range");
        withinRange = true;
        peripherals.enabled = withinRange;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        Debug.Log("out of range");
        withinRange = false;
        peripherals.enabled = withinRange;
    }
}
