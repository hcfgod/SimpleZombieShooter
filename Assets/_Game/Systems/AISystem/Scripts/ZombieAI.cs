using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
	[Header("Chase Settings")]
	public Transform target; // The target to chase
	public float chaseSpeed = 3.5f;

	[Header("Attack Settings")]
	public float attackRange = 3f;
	public float attackDelay = 1f;

	private NavMeshAgent navAgent;
	private bool isAttacking = false;

	private void Awake()
	{
		navAgent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		if (target == null) return;

		float distanceToTarget = Vector3.Distance(transform.position, target.position);

		if (distanceToTarget <= attackRange)
		{
			if (!isAttacking)
			{
				StartCoroutine(AttackRoutine());
			}
		}
		else
		{
			ChaseTarget();
		}
	}

	private void ChaseTarget()
	{
		navAgent.speed = chaseSpeed;
		navAgent.SetDestination(target.position);
	}

	private IEnumerator AttackRoutine()
	{
		isAttacking = true;
		navAgent.SetDestination(transform.position); // Stop moving

		// Perform the attack (replace Debug.Log with your attack logic)
		Debug.Log("Attacking!");

		yield return new WaitForSeconds(attackDelay);
		isAttacking = false;
	}
}
