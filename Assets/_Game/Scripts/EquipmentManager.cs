using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
	public GameObject PrimaryWeaponHolder;
	public GameObject SecondaryWeaponHolder;

	private WeaponManager _weaponManager;
	private SimpleInventory _simpleInventory;
	
	private void Awake()
	{
		_weaponManager = GetComponent<WeaponManager>();
		_simpleInventory = GetComponent<SimpleInventory>();
	}
	
	private void Start()
	{
		InputManager.Instance.PlayerInputActions.Weapon.SwitchToPrimary.performed += _ => SwitchToPrimary();
		InputManager.Instance.PlayerInputActions.Weapon.SwitchToSecondary.performed += _ => SwitchToSecondary();
	}
	
	private void SwitchToPrimary()
	{
		if(_simpleInventory.CurrentPrimary == null)
			return;
				
		EquipPrimary(_simpleInventory.CurrentPrimary.WeaponData.WeaponName);
	}
	
	private void SwitchToSecondary()
	{
		if(_simpleInventory.CurrentSecondary == null)
			return;
				
		EquipSecondary(_simpleInventory.CurrentSecondary.WeaponData.WeaponName);
	}
	
	public void EquipPrimary(string weaponName)
	{
		DeactivateAllChildren(PrimaryWeaponHolder);
		DeactivateAllChildren(SecondaryWeaponHolder);
		
		GameObject weapon = PrimaryWeaponHolder.transform.Find(weaponName).gameObject;
		weapon.SetActive(true);

		_weaponManager.SwitchWeapon(weapon.GetComponent<IWeapon>());
	}

	public void EquipSecondary(string weaponName)
	{
		DeactivateAllChildren(PrimaryWeaponHolder);
		DeactivateAllChildren(SecondaryWeaponHolder);
		
		GameObject weapon = SecondaryWeaponHolder.transform.Find(weaponName).gameObject;
		weapon.SetActive(true);

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
