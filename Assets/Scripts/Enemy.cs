using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [Range(0, 360)]
    public float detectionRadius;

    public float viewportAngle;

    public GameObject player;

    private float[] _oldValues;

    public NavMeshAgent _aiController;

    public string currentState;

    public string nextState;

    public float idleTime = 2.5f;

    public PatrolPoints[] targetPoints;

    /// <summary>
    /// contains mask for AI to target only items within this mask
    /// </summary>
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool isChasing;
    
    public bool playerVisible;

    public bool checking;
    
    private void Awake()
    {
        _aiController = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        player = FindObjectOfType<PlayerMove>().gameObject;
        StartCoroutine(DrawFOV());
        _oldValues = new[] {detectionRadius, viewportAngle};
        targetPoints = FindObjectsOfType<PatrolPoints>();
        
        // initalization
        nextState = "Idle";
        currentState = nextState;
        SwitchState();
    }


    private void Update()
    {
        if (playerVisible && !isChasing)
        {
            //Debug.Log("callChase");
            nextState = "Chase";
            StopCoroutine(DrawFOV());
        }
        if (currentState != nextState)
        {
            currentState = nextState;
            
            /*
            if (currentState == "Investigate")
            {
                Debug.Log("ran");
                SwitchState();
                checking = false;
            }
            */
        }
        

        if (currentState == "Chase")
        {
            _aiController.transform.LookAt(new Vector3(player.transform.position.x, transform.position.y,
                player.transform.position.z));
        }
    }

    public void SwitchState()
    {
        StartCoroutine(currentState);
    }

    private IEnumerator Idle()
    {
        while (currentState == "Idle")
        {
            yield return new WaitForSeconds(idleTime);

            nextState = "Patrol";
        }
        SwitchState();
    }
    
    private IEnumerator Patrol()
    {
        var randInt = Random.Range(0, targetPoints.Length);
        var hasReached = false;
        // sets a random destination for the ai
        _aiController.SetDestination(targetPoints[randInt].transform.position);
        while (currentState == "Patrol")
        {
            yield return null;
            if (!hasReached)
            {
                // if the remainingDist of AI is less than the stopping dist, it has not reached destination
                // Stopping distance is not 0 as ai might have stopping distance set
                if (_aiController.remainingDistance <= _aiController.stoppingDistance)
                {
                    hasReached = true;
                    nextState = "Idle";
                }
            }
        }
        SwitchState();
    }
    
    private IEnumerator Chase()
    {
        var elapsed = 0f;
        isChasing = true;
        while (currentState == "Chase")
        {
            yield return null;
            // if the remainingDist of AI is less than the stopping dist, it has not reached destination
            // Stopping distance is not 0 as ai might have stopping distance set
            // and if player cannot be seen
            if (!playerVisible)
            {
                break;
            }
            _aiController.SetDestination(player.transform.position);
        }
        //Instantiate(FindObjectOfType<PatrolPoints>().gameObject, player.transform.position, Quaternion.identity);
        _aiController.SetDestination(player.transform.position);
        // let ai move to player's last seen position
        while (_aiController.remainingDistance <= _aiController.stoppingDistance)
        {
            yield return null;
            //Debug.Log(_aiController.remainingDistance);
            _aiController.transform.LookAt(player.transform);
            // if the player can be seen when reaching the last seen position, restart the coroutine without switching it back to idle
        }

        yield return new WaitForSeconds(3f);
        while (elapsed < 3f)
        {
            yield return null;
            if (playerVisible)
            {
                SwitchState();
                isChasing = false;
                yield break;
            }

            elapsed += Time.deltaTime;
        }
        nextState = "Idle";
        currentState = "Idle";
        SwitchState();
        StartCoroutine(DrawFOV());
        isChasing = false;
    }

    /*
    private IEnumerator Investigate()
    {
        Debug.Log("ToIdle");
        while (currentState == "investigate")
        {
            yield return null;
            Debug.Log("OnRoute");
            if (_aiController.remainingDistance <= _aiController.stoppingDistance)
            {
                nextState = "Idle";
            }
        }
        SwitchState();
        checking = true;
    }
    */

    private IEnumerator DrawFOV()
    {
        // always running
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            UpdateValues();
            yield return new WaitForSeconds(0.2f);
            FOVCheck();
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private void FOVCheck()
    {
        var rangeChecks = Physics.OverlapSphere(transform.position, detectionRadius, targetMask);

        // if there are no items within the FOV check, dont do anything
        if (rangeChecks.Length == 0)
        {
            // if player is seen prior, and is out of the range, player is no longer seen
            if (playerVisible) playerVisible = false;
            // regardless of above if statement's outcome, skips below's checks
            return;
        }
        
        // checks all valid checks
        foreach (var itemInRange in rangeChecks)
        {
            var target = itemInRange.transform;
            if (target.tag == "Player") PlayerReaction(target);
            // note: just get ghost to redirect to obj every now and then personally.
        }
        
    }

    private void PlayerReaction(Transform target)
    {
        var directionToTarget = (target.position - transform.position);
        directionToTarget.Normalize();

        // gets the angle of player and itself, from its front view.
        // (angle, where origin is eyes, to the player.
        if (Vector3.Angle(transform.forward, directionToTarget) < viewportAngle / 2)
        {
            var distanceToTarget = Vector3.Distance(transform.position, target.position);

            // from middle of enemy, straight to player. if there is anything, other than player, between the raycast, 
            // enemy does not see the player
            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                playerVisible = true;
            else playerVisible = false;
        }
        else playerVisible = false;
    }

    /// <summary>
    /// For debugging purpose
    /// </summary>
    private void UpdateValues()
    {
        if (detectionRadius == _oldValues[0] || viewportAngle == _oldValues[1]) return;
        _oldValues = new[] {detectionRadius, viewportAngle};
    }
}
