using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInventory : MonoBehaviour
{
	public GameObject PrimaryWeaponHolder;
	public GameObject SecondaryWeaponHolder;

	private WeaponManager _weaponManager;
	
	private void Awake()
	{
		_weaponManager = GetComponent<WeaponManager>();
	}
	
	private void Start()
	{
		EquipSecondary("Pistol_M1911");
	}
	
	public void EquipPrimary(string weaponName)
	{
		DeactivateAllChildren(PrimaryWeaponHolder);
		GameObject weapon = PrimaryWeaponHolder.transform.Find(weaponName).gameObject;
		weapon.SetActive(true);
		// Notify via event or other means
		
		_weaponManager.SwitchWeapon(weapon.GetComponent<IWeapon>());
	}

	public void EquipSecondary(string weaponName)
	{
		DeactivateAllChildren(SecondaryWeaponHolder);
		GameObject weapon = SecondaryWeaponHolder.transform.Find(weaponName).gameObject;
		weapon.SetActive(true);
		// Notify via event or other means
		
		_weaponManager.SwitchWeapon(weapon.GetComponent<IWeapon>());
	}

	public void DeactivateAllPrimaries()
	{
		foreach (Transform child in PrimaryWeaponHolder.transform)
		{
			child.gameObject.SetActive(false);
		}
	}
	
	public void DeactivateAllSecondaries()
	{
		foreach (Transform child in SecondaryWeaponHolder.transform)
		{
			child.gameObject.SetActive(false);
		}
	}
	
	private void DeactivateAllChildren(GameObject parent)
	{
		foreach (Transform child in parent.transform)
		{
			child.gameObject.SetActive(false);
		}
	}
}
