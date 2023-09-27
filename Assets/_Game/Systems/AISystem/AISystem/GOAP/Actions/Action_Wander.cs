using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Action_Wander : Action_Base
{
    // Wander parameters and control
    [SerializeField] protected float minWanderRadius = 20.0f;
    [SerializeField] protected float maxWanderRadius = 40.0f;

    [SerializeField] protected float wanderIntervalMin = 3.0f;
    [SerializeField] protected float wanderIntervalMax = 7.0f;
    [SerializeField] protected float minSpeed = 3.0f;
    [SerializeField] protected float maxSpeed = 6.0f;
    [SerializeField] protected int memorySize = 5; // number of recent positions to remember
    [SerializeField] protected float avoidDistance = 5.0f; // minimum distance from recent positions
    private bool isWandering = false;
    private bool IsWaiting = false;
    protected Coroutine wanderCoroutine;
    protected Queue<Vector3> recentPositions = new Queue<Vector3>();

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Wander) });

    public override List<System.Type> GetSupportedGoals()
    {
        return SupportedGoals;
    }

    public override float GetCost()
    {
        return 0f;
    }

    public override void OnActivated(Goal_Base _linkedGoal)
    {
        base.OnActivated(_linkedGoal);

        if (!isWandering)
        {
            wanderCoroutine = StartCoroutine(Wander());
            isWandering = true;
        }

        Agent.SetMoveSpeed(1);
    }

    public override void OnDeactivated()
    {
        base.OnDeactivated();

        if (wanderCoroutine != null)
        {
            isWandering = false;
            Agent.SetMoveSpeed(3);

            StopCoroutine(wanderCoroutine);
            wanderCoroutine = null;
        }
    }

    protected IEnumerator Wander()
    {
        while (true)
        {
            float randomWanderRadius = Random.Range(minWanderRadius, maxWanderRadius);

            Vector3 newPos = GetRandomNavMeshPosition(randomWanderRadius);

            // Check distance from recent positions
            while (IsTooCloseToRecentPositions(newPos, avoidDistance))
            {
                newPos = GetRandomNavMeshPosition(randomWanderRadius);
            }

            // Set agent speed
            Agent.SetMoveSpeed(Random.Range(minSpeed, maxSpeed));

            Agent.GetNavMeshAgent().SetDestination(newPos);

            // Wait until the AI reaches the destination before continuing
            while (!IsAtDestination())
            {
                IsWaiting = false;
                yield return null;
            }

            // Remember this position
            recentPositions.Enqueue(newPos);
            if (recentPositions.Count > memorySize)
            {
                recentPositions.Dequeue();
            }

            // Random pause
            float pauseDuration = Random.Range(wanderIntervalMin, wanderIntervalMax);
            IsWaiting = true;
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    protected bool IsTooCloseToRecentPositions(Vector3 position, float minDistance)
    {
        foreach (Vector3 recentPosition in recentPositions)
        {
            if (Vector3.Distance(position, recentPosition) < minDistance)
            {
                return true;
            }
        }
        return false;
    }

    public Vector3 GetRandomNavMeshPosition(float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;

        if (NavMesh.SamplePosition(randomDirection, out hit, range, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    public bool IsAtDestination()
    {
        // Check if the AI is close to the destination
        if (!Agent.GetNavMeshAgent().pathPending)
        {
            if (Agent.GetNavMeshAgent().remainingDistance <= Agent.GetNavMeshAgent().stoppingDistance)
            {
                if (!Agent.GetNavMeshAgent().hasPath || Agent.GetNavMeshAgent().velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool GetIsWandering()
    {
        return isWandering;
    }
    public bool GetIsWaiting()
    {
        return IsWaiting;
    }
}
