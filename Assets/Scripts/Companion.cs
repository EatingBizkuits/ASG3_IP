using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class Companion : MonoBehaviour
{
    public Transform target; 
    [SerializeField]
    private bool onPath;
    private NavMeshAgent _aiController;
    [Range(0f, 100f)]
    public float detectionRadius = 20f;

    public Animator displayObj;

    private static readonly int ONPath = Animator.StringToHash("_onPath");

    private GameObject _player;

    [SerializeField] 
    private bool stopFollow;
    
    [SerializeField] 
    private bool onFollow;

    public Material dither;

    [SerializeField] private Color _color;
    
    [SerializeField] private Color _currentColor;
    
    [SerializeField] private Color _endColor;
    
    private float _colorLerpSpeed = 1.5f;
    
    private float currentProgress;

    public Color[] colorPalette = new[] {Color.green, Color.red, Color.cyan};
    
    private void Awake()
    {
        _aiController = GetComponent<NavMeshAgent>();
        _player = FindObjectOfType<PlayerMove>().gameObject;
    }

    private void FixedUpdate()
    {
        AnimStates();
    }

    private void ColorChange(Color endColor)
    {
        StopCoroutine(ColorChanging());
        currentProgress = 0;
        _currentColor = dither.color;
        _endColor = endColor;
        StartCoroutine(ColorChanging());
    }

    private IEnumerator ColorChanging()
    {
        while (currentProgress/_colorLerpSpeed < 1)
        {
            currentProgress += Time.deltaTime;
            var lerpValue = Color.Lerp(_currentColor, _endColor, currentProgress / _colorLerpSpeed);
            dither.color = lerpValue;
            yield return new WaitForFixedUpdate();
        }
    }
    
    private void AnimStates()
    {
        // if the onPath is true, but _onPath(in editor) is false
        if (displayObj.GetBool(ONPath) && !onPath)
        {
            Debug.Log(displayObj.GetBool(ONPath) + " 1 " + onPath);
            displayObj.SetBool(ONPath, onPath);
        }
        
        // if the onPath is false, but _onPath(in editor) is true
        else if (!displayObj.GetBool(ONPath) && onPath)
        {
            Debug.Log(displayObj.GetBool(ONPath) + " 2 " + onPath);
            displayObj.SetBool(ONPath, onPath);
        }
    }
    
    private void CalculateToTarget()
    {
        StopCoroutine(ReachedAreaCheck());
        if (target == null || onPath) return;
        _aiController.SetDestination(target.position);
        onPath = true;
        StartCoroutine(ReachedAreaCheck());
    }

    private void ReachedAreaPrompt()
    {
        onPath = false;
        target = null;
    }

    /// <summary>
    /// checks if AI has stopped. If stopped, state that the companion has reached
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReachedAreaCheck()
    {
        while (onPath)
        {
            yield return new WaitForSeconds(3f);
            if (_aiController.velocity != Vector3.zero) continue;
            StopCoroutine(CheckItemPickUp());
            StartCoroutine(CheckItemPickUp());
        }
    }

    // check if the object is picked up, if not picked up, companion stays till futher changes
    private IEnumerator CheckItemPickUp()
    {
        while (onPath)
        {
            ColorChange(colorPalette[1]);
            yield return new WaitForFixedUpdate();
            if (!onPath) continue;
            if (target.gameObject.activeSelf) continue;
            ColorChange(colorPalette[2]);
            ReachedAreaPrompt();
            ReturnToPlayer();
            yield break;
        }
    }

    private void ReturnToPlayer()
    {
        ColorChange(colorPalette[2]);
        StopCoroutine(MoveToPlayer());
        stopFollow = false;
        onPath = false;
        target = null;
        _aiController.ResetPath();
        onFollow = true;
        StartCoroutine(MoveToPlayer());
    }
    
    /// <summary>
    /// Sets new Path to find object based on detection range
    /// </summary>
    public void SetNewTarget()
    {
        StopCoroutine(MoveToPlayer());
        stopFollow = false;
        onFollow = false;
        // if trigger pressed again when companion is sent out, returns to player
        if (onPath)
        {
            ReturnToPlayer();
            return;
        }
        // value to be higher than detectionRadius on init 
        var closestObj = detectionRadius + 10;
        Transform closestGameObj = null;
        var allItems = FindObjectsOfType<keyItemIdentifier>();
        Debug.Log(allItems.Length);
        foreach (var item in allItems)
        {
            var tempValue = Vector3.Distance(transform.position, item.transform.position);
            // if the dist is out of the detection radius, skips 
            if (tempValue > detectionRadius) continue;
            // if the dist is in radius and but is further from companion than previous obj, skips
            if (tempValue >= closestObj) continue;
            
            // closest to companion but within detect range
            closestObj = tempValue;
            closestGameObj = item.transform;
        }

        // if there are no items that fits the criteria, no objects are found'
        if (closestGameObj == null) return;
        target = closestGameObj;
        CalculateToTarget();
    }

    private IEnumerator MoveToPlayer()
    {
        while (!onPath)
        {
            _aiController.SetDestination(_player.transform.position);
            yield return new WaitForSeconds(0.4f);
            if (onPath) yield break;
            // if its still moving, resets dbl confirm and continues loop with updated player destination
            if (_aiController.velocity != Vector3.zero)
            {
                stopFollow = false;
                continue;
            }
            // if no movement is confirmed twice, break out of coroutine
            if (stopFollow)
            {
                onFollow = false;
                yield break;
            }
            // if no movement for the first time, loop once more to confirm
            if (!stopFollow) stopFollow = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player" || onPath || onFollow) return;
        StopCoroutine(MoveToPlayer());
        stopFollow = false;
        onFollow = true;
        StartCoroutine(MoveToPlayer());
    }
    
}
