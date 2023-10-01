using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
	[Header("Chase Settings")]
	[SerializeField] private Transform target; // The target to chase
	[SerializeField] private float chaseSpeed = 3.5f;

	[Header("Attack Settings")]
	[SerializeField] private float attackRange = 3f;
	[SerializeField] private float attackDelay = 1f;

	[SerializeField] private bool isDead = false;
	[SerializeField] private bool isAttacking = false;
	[SerializeField] private bool isChasing = false;
	
	private NavMeshAgent navAgent;
	
	private void Awake()
	{
		navAgent = GetComponent<NavMeshAgent>();
		target = GameObject.FindWithTag("Player").transform;
	}

	private void Update()
	{
		if (target == null) return;

		if(isDead)
		{
			navAgent.SetDestination(transform.position);
			return;
		}
			
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
		
		if(!isDead)
		{
			if(!isAttacking)
			{
				isChasing = true;
			}
			else
			{
				isChasing = false;
			}
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
	
	public bool GetIsDead()
	{
		return isDead;
	}
	
	public void SetIsDead(bool value)
	{
		isDead = value;
	}
	
	public bool GetIsChasing()
	{
		return isChasing;
	}
	
	public void SetIsChasing(bool value)
	{
		isChasing = value;
	}
	
	public bool GetIsAttacking()
	{
		return isAttacking;
	}
	
	public void SetIsAttack(bool value)
	{
		isAttacking = value;
	}
}
