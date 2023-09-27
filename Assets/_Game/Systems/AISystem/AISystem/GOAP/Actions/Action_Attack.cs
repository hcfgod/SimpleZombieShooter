using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Action_Attack : Action_Base
{
    [SerializeField] private bool isAttacking = false;
    private bool canAttack = false;
    [SerializeField] private float attackRange = 3f;

    [SerializeField] private float minAttackDelay = 1f;
    [SerializeField] private float maxAttackDelay = 2f;

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Attack) });

    Goal_Attack AttackGoal;

    public UnityEvent attackEvent;

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
        AttackGoal = (Goal_Attack)LinkedGoal;
    }

    public override void OnDeactivated()
    {
        base.OnDeactivated();

        AttackGoal = null;
    }

    public override void OnTick()
    {
        if (AttackGoal == null)
            return;

        

        // If the agent is within attacking range, perform attack
        if (IsInAttackRange())
        {
            canAttack = true;

            if(!isAttacking)
            {
                isAttacking = true;
                StartCoroutine(AttackRoutine());
            }
        }
        else // If the agent is not within attacking range, move towards target
        {
            canAttack = false;
            Agent.MoveTo(AttackGoal.GetTarget().transform.position);
        }

        Agent.LookAtTarget(AttackGoal.GetTarget().transform, 360);
    }

    private IEnumerator AttackRoutine()
    {
        float randomizedDelay = Random.Range(minAttackDelay, maxAttackDelay);
        yield return new WaitForSeconds(randomizedDelay);

        if (canAttack)
        {
            attackEvent?.Invoke();
            Debug.Log("Attacked!");
        }

        isAttacking = false;
    }

    public bool IsInAttackRange()
    {
        if (AttackGoal.GetTarget() == null) return false;

        // Calculate the distance between the agent and its target
        return Vector3.Distance(Agent.transform.position, AttackGoal.GetTarget().transform.position) <= attackRange;
    }

    public bool GetIsAttacking()
    {
        return isAttacking;
    }
    public bool GetCanAttack()
    {
        return canAttack;
    }
}
