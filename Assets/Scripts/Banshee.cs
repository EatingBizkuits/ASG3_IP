
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

public class Banshee : MonoBehaviour
{
    public Animator displayObj;
    private static readonly int ONPath = Animator.StringToHash("_onPath");

    public string nextState;
    public string currentState;

    private NavMeshAgent _aiController;
    private NavMeshAgent _mainEnemyController;
    
    public Transform[] targetPoints;

    private float _idleTime = 1f;

    public int destinationValue;

    public bool playerDetected;

    public Transform player;

    public AudioSource scream;
    
    private void Start()
    {
        _aiController = GetComponent<NavMeshAgent>();
        // initalization
        nextState = "Idle";
        currentState = nextState;
        SwitchState();
        _mainEnemyController = FindObjectOfType<Enemy>()._aiController;
        player = FindObjectOfType<PlayerMove>().transform;
    }

    private void Update()
    {
        if (currentState != nextState)
        {
            currentState = nextState;
        }
    }

    private void FixedUpdate()
    {
        if (playerDetected)
        {
            transform.LookAt(player.position);
            // tells the main AI my location
            _mainEnemyController.SetDestination(transform.position);
        }
    }

    private void SwitchState()
    {
        StartCoroutine(currentState);
    }

    private IEnumerator Idle()
    {
        displayObj.SetBool(ONPath, false);
        while (currentState == "Idle")
        {
            yield return new WaitForSeconds(_idleTime);
            nextState = "Patrol";
        }
        SwitchState();
    }
    
    private IEnumerator Patrol()
    {
        var hasReached = false;
        // sets a random destination for the ai
        //Debug.Log("patrolling1");
        _aiController.SetDestination(targetPoints[destinationValue].position);
        displayObj.SetBool(ONPath, true);
        while (currentState == "Patrol")
        {
            //Debug.Log("patrolling");
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
        destinationValue += 1;
        if (destinationValue >= targetPoints.Length) destinationValue = 0;
        SwitchState();
    }

    /*
    
    private IEnumerator Stop()
    {
        StopAllCoroutines();
        var enemyCode = _mainEnemyController.transform.GetComponent<Enemy>();
        var playerTransform = enemyCode.player.transform.position;
        _aiController.SetDestination(playerTransform);
        destinationValue = 0;
        while (true)
        {
            Debug.Log("pause " + playerDetected); // true
            _mainEnemyController.SetDestination(playerTransform);
            transform.LookAt(new Vector3(playerTransform.x, transform.position.y, playerTransform.z));
            yield return null;
            if (!playerDetected) break;
            Debug.Log("pause2"); // does not even print

        }
        Debug.Log("done");
        nextState = "Idle";
        SwitchState();
        enemyCode.nextState = "Idle";
        enemyCode.SwitchState();
        Debug.Log("done2");
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.transform.tag);
        if (other.transform.tag != "Player") return;
        Debug.Log("stop");
        playerDetected = true;
        nextState = "Stop";
        SwitchState();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag != "Player") return;
        Debug.Log("cont");
        nextState = "Idle";
        playerDetected = false;
    }
    */
    
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.transform.tag);
        if (other.transform.tag != "Player") return;
        Debug.Log("stop");
        playerDetected = true;
        // cut all coroutines and focus on player
        StopAllCoroutines();
        // halt its position;
        _aiController.ResetPath();
        //FindObjectOfType<Enemy>().nextState = "Investigate";
        if (scream.isPlaying) return;
        scream.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag != "Player") return;
        playerDetected = false;
        nextState = "Idle";
        SwitchState();
        var enemyCode = FindObjectOfType<Enemy>();
        // if the enemy sees player, dont change its state;
    }
}
