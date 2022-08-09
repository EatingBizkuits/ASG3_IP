using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [Range(0, 360)]
    public float detectionRadius;

    public float viewportAngle;

    public GameObject player;

    private float[] _oldValues;
    
    /// <summary>
    /// contains mask for AI to target only items within this mask
    /// </summary>
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool playerVisible;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMove>().gameObject;
        StartCoroutine(DrawFOV());
        _oldValues = new[] {detectionRadius, viewportAngle};
        Debug.DrawLine(transform.position, transform.position + Vector3.forward * detectionRadius, Color.yellow, 200f);
    }

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

        if (playerVisible)
        {
            Debug.DrawLine(transform.position, target.position, Color.red, 0.3f);
            Debug.Log("Player On Focus, Rerouting to player");
        }
    }

    private void UpdateValues()
    {
        if (detectionRadius == _oldValues[0] || viewportAngle == _oldValues[1]) return;
        Debug.DrawLine(transform.position, transform.position + Vector3.forward * detectionRadius, Color.yellow, 200f);
        _oldValues = new[] {detectionRadius, viewportAngle};
        Debug.Log("values reset");
    }
    
}
