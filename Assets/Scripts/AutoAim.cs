using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AutoAim : MonoBehaviour
{
    [Tooltip("The radius of the target search. It is displayed in the inspector as a green sphere")]
    [SerializeField] private float _scanRadius;
    [Tooltip("Scan frequency. A lower value will produce better quality, but can reduce performance")]
    [SerializeField] private float _scanFrequency;
    [Tooltip("Target layers")]
    [SerializeField] private LayerMask _targetsMask;
    [Tooltip("Maximum amount of targets")]
    [SerializeField] private int _overlapMaximum;
    
    private float _scanTimer;

    public event UnityAction<Transform> TargetFound;
    public event UnityAction TargetLost;

    private void Update()
    {
        //scan when timer expires
        if(_scanTimer > 0)
            _scanTimer -= Time.deltaTime;
        else
            ScanForTargets();
    }
    private void ScanForTargets()
    {
        //search potential targets
        Collider[] colliders = new Collider[_overlapMaximum];
        int numberOfHits = Physics.OverlapSphereNonAlloc(transform.position, _scanRadius, colliders, _targetsMask);

        if (numberOfHits == 0)
        {
            return;
        }

        List<Transform> potentialTargets = new List<Transform>();
        // we go through each collider found
        for (int i = 0; i < _overlapMaximum; i++)
        {
            if (colliders[i] == null)
            {
                continue;
            }
            if ((colliders[i].gameObject == this.gameObject) || (colliders[i].transform.IsChildOf(this.transform.root)))
            {
                continue;
            }
            
            potentialTargets.Add(colliders[i].gameObject.transform);
        }


        // we sort our targets by distance
        potentialTargets.Sort(delegate (Transform a, Transform b)
        {
            return Vector3.Distance(this.transform.position, a.transform.position)
            .CompareTo(
                Vector3.Distance(this.transform.position, b.transform.position));
        });

        //return closest target or lost target event if nothing found
        if (potentialTargets.Count > 0)
            TargetFound?.Invoke(potentialTargets[0]);
        else
            TargetLost?.Invoke();

        //reset timer
        _scanTimer = _scanFrequency;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _scanRadius);
    }
}
