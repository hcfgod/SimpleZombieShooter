using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInventory : MonoBehaviour
{
	public IWeapon CurrentPrimary { get; set; }
	public IWeapon CurrentSecondary { get; set; }
	
	public List<BaseWeapon> Weapons = new List<BaseWeapon>();
	
	private void Start()
	{
		AcquirePrimary(Weapons[0]);
		AcquireSecondary(Weapons[1]);
	}
	
	public void AcquirePrimary(IWeapon newWeapon) 
	{
		CurrentPrimary = newWeapon;
	}
	
	public void AcquireSecondary(IWeapon newWeapon) 
	{
		CurrentSecondary = newWeapon;
	}
}
