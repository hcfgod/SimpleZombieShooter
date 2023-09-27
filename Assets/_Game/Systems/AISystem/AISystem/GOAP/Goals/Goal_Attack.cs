using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Attack : Goal_Base
{
    [SerializeField] int AttackPriority = 70;
    [SerializeField] float MinAwarenessToAttack = 2.0f;
    [SerializeField] float AwarenessToStopAttack = 1f;

    DetectableTarget CurrentTarget;
    int CurrentPriority = 0;

    public override void OnTickGoal()
    {
        CurrentPriority = 0;

        // no targets
        if (Sensors.ActiveTargets == null || Sensors.ActiveTargets.Count == 0)
            return;

        if (CurrentTarget != null)
        {
            // check if the current is still sensed
            foreach (var candidate in Sensors.ActiveTargets.Values)
            {
                if (candidate.Detectable == CurrentTarget)
                {
                    CurrentPriority = candidate.Awareness < AwarenessToStopAttack ? 0 : AttackPriority;
                    return;
                }
            }

            // clear our current target
            CurrentTarget = null;
        }

        // acquire a new target if possible
        foreach (var candidate in Sensors.ActiveTargets.Values)
        {
            // found a target to acquire
            if (candidate.Awareness >= MinAwarenessToAttack)
            {
                CurrentTarget = candidate.Detectable;
                CurrentPriority = AttackPriority;
                return;
            }
        }
    }

    public override int CalculatePriority()
    {
        return CurrentPriority;
    }

    public override bool CanRun()
    {
        // no targets
        if (Sensors.ActiveTargets == null || Sensors.ActiveTargets.Count == 0)
            return false;

        // check if we have anything we are aware of
        foreach (var candidate in Sensors.ActiveTargets.Values)
        {
            if (candidate.Awareness >= MinAwarenessToAttack)
                return true;
        }

        return false;
    }

    public override void OnGoalDeactivated()
    {
        base.OnGoalDeactivated();

        CurrentTarget = null;
    }

    public DetectableTarget GetTarget()
    {
        return CurrentTarget;
    }
}
