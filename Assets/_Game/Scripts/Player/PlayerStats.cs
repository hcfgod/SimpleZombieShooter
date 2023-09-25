using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatSystem;

public class PlayerStats : MonoBehaviour
{
	[SerializeField] private StatData _healthStatData;

	private StatManager _statManager;
	
	private Stat _healthStat;
	
	private void Awake()
	{
		_statManager = GetComponent<StatManager>();
	}
	
	private void Start()
	{
		_healthStat = _statManager.GetStat(_healthStatData);
		
		StatCondition zeroHealthCondition = new StatCondition("Health", 0, StatConditionType.EqualTo);
		_healthStat.AddCondition(zeroHealthCondition);
		_healthStat.OnConditionMet += ZeroHealthConditionMet;
		
		_statManager.GetStatInteractionManager().RegisterInteraction("HealthZero", new HealthZeroInteraction());
	}
	
	private void ZeroHealthConditionMet(StatCondition condition)
	{
		if (condition.StatName == "Health" && condition.Type == StatConditionType.EqualTo)
		{
			_statManager.GetStatInteractionManager().TriggerInteraction("HealthZero", _healthStat, null);
		}
	}
	
	public void ValueChanged(float newStatValue)
	{
		Debug.Log(newStatValue);
	}
}
