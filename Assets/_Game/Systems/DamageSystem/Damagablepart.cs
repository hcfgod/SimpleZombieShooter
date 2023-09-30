using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatSystem;

public class Damagablepart : MonoBehaviour, IDamagable
{
	public string DamagePartName;
	
	[SerializeField] private StatData healthStatData;
	[SerializeField] ParticleSystem headExplodeEffect;
	[SerializeField] private GameObject partToRemove;
	
	private StatManager _statManager;
	
	private void Awake()
	{
		_statManager = GetComponentInParent<StatManager>();
	}
	
	public void DamagableHit(RaycastHit hitInfo, WeaponData weaponData)
	{
		if(DamagePartName == "Head")
		{
			_statManager.DecreaseStatValue(healthStatData, 100);
			
			if(headExplodeEffect != null)
			{
				headExplodeEffect.Play();
			}
			
			if(partToRemove != null)
				Destroy(partToRemove);
		}
		else
		{
			_statManager.DecreaseStatValue(healthStatData, Random.Range(weaponData.MinDamage, weaponData.MaxDamage));
		}
	}
}
