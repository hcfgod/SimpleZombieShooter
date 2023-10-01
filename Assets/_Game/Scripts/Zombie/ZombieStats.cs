using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatSystem;

public class ZombieStats : MonoBehaviour
{
	[SerializeField] private StatData _healthStatData;

	private ZombieAI _zombieAI;
	private StatManager _statManager;
	
	private Stat _healthStat;
	
	private bool isDestroying = false;
	
	private void Awake()
	{
		_statManager = GetComponent<StatManager>();
		_zombieAI = GetComponent<ZombieAI>();
	}
	
	private void Start()
	{
		_healthStat = _statManager.GetStat(_healthStatData);
		
		StatCondition zeroHealthCondition = new StatCondition("Zombie Health", 0, StatConditionType.EqualTo);
		_healthStat.AddCondition(zeroHealthCondition);
		_healthStat.OnConditionMet += ZeroHealthConditionMet;
		
		_statManager.GetStatInteractionManager().RegisterInteraction("ZombieHealthZero", new ZombieHealthZeroInteraction());
	}
	
	private void ZeroHealthConditionMet(StatCondition condition)
	{
		if(isDestroying)
			return;
			
		if (condition.StatName == "Zombie Health" && condition.Type == StatConditionType.EqualTo)
		{
			_zombieAI.SetIsDead(true);
			StartCoroutine(DestroyZombieRoutine());
			_statManager.GetStatInteractionManager().TriggerInteraction("ZombieHealthZero", this.gameObject, _healthStat, null);
			
			isDestroying = true;
		}
	}
	
	private IEnumerator DestroyZombieRoutine()
	{
		yield return new WaitForSeconds(0.5f);
		ComponentUtils.DeactivateComponentsAndColliders(this.gameObject);
		yield return new WaitForSeconds(10f);
		Destroy(gameObject);
	}
}
