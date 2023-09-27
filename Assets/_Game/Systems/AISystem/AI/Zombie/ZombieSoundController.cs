using UnityEngine;

public class ZombieSoundController : MonoBehaviour
{
    GOAPPlanner goapPlanner;

    [SerializeField] private AudioClip[] detectedTargetSounds;
    [SerializeField] private AudioClip[] attackSounds;

    private AudioSource audioSource;

    private int randomClipIndex;
    private bool isAmbiencePlaying = false;

    private void Awake()
    {
        goapPlanner = GetComponentInParent<GOAPPlanner>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (goapPlanner.ActiveAction != null)
        {
            if (goapPlanner.ActiveAction.ActionName == "Idle")
            {
                isAmbiencePlaying = false;
            }

            if (goapPlanner.ActiveAction.ActionName == "Wander")
            {
                Action_Wander actionWander = goapPlanner.ActiveAction as Action_Wander;

                if (actionWander == null) { return; }

                if(!isAmbiencePlaying)
                {
                    isAmbiencePlaying = true;
                }
            }

            if (goapPlanner.ActiveAction.ActionName == "Chase")
            {
                isAmbiencePlaying = false;
            }
        }
    }

    public void PlayRandomDetectedSound()
    {
        if (goapPlanner.ActiveAction.ActionName == "Attack")
        {
            return;
        }

        randomClipIndex = Random.Range(0, detectedTargetSounds.Length);
        AudioManager.instance.Play3DSound(detectedTargetSounds[randomClipIndex], transform.position, 0.15f);
    }

    public void PlayRandomAttackSound()
    {
        randomClipIndex = Random.Range(0, attackSounds.Length);
        AudioManager.instance.Play3DSound(attackSounds[randomClipIndex], transform.position, 0.15f);
    }
}
