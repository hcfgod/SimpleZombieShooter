using UnityEngine;

public class ZombieAnimatorController : MonoBehaviour
{
    private Animator zombieAnimator;
    GOAPPlanner goapPlanner;

    private void Awake()
    {
        zombieAnimator = GetComponent<Animator>();
        goapPlanner = GetComponentInParent<GOAPPlanner>();
    }

    private void Update()
    {
        ZeroGameObject.ZeroGameObjectLocalPosition(gameObject);

        if(goapPlanner.ActiveAction != null)
        {
            if (goapPlanner.ActiveAction.ActionName == "Idle")
            {
                zombieAnimator.SetBool("isChasing", false);
                zombieAnimator.SetBool("isWandering", false);
                zombieAnimator.SetBool("isAttacking", false);
            }
            if (goapPlanner.ActiveAction.ActionName == "Wander")
            {
                Action_Wander actionWander = goapPlanner.ActiveAction as Action_Wander;

                if (actionWander == null) { return; }

                if(actionWander.GetIsWaiting())
                {
                    zombieAnimator.SetBool("isWandering", false);
                }
                else
                {
                    zombieAnimator.SetBool("isWandering", true);
                }

                zombieAnimator.SetBool("isChasing", false);
                zombieAnimator.SetBool("isAttacking", false);
            }
            if (goapPlanner.ActiveAction.ActionName == "Chase")
            {
                zombieAnimator.SetBool("isChasing", true);

                zombieAnimator.SetBool("isWandering", false);
                zombieAnimator.SetBool("isAttacking", false);
            }
            if (goapPlanner.ActiveAction.ActionName == "Attack")
            {
                Action_Attack attackAction = goapPlanner.ActiveAction as Action_Attack;

                if (attackAction == null) { return; }

                if(attackAction.GetCanAttack())
                {
                    if(attackAction.GetIsAttacking())
                    {
                        zombieAnimator.SetBool("isAttacking", true);
                    }

                    zombieAnimator.SetBool("isWandering", false);
                    zombieAnimator.SetBool("isChasing", false);
                }
                else
                {
                    zombieAnimator.SetBool("isAttacking", false);
                    zombieAnimator.SetBool("isChasing", true);
                }
            }
        }
    }
}
