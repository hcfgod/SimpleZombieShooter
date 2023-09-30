using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponManager : MonoBehaviour
{
	[SerializeField] PlayerData playerData;
	
	public IWeapon CurrentWeapon { get; set; }
	
	private WeaponAnimator _gunAnimator;
	
	private InputManager _inputManager;
	
	private void Start()
	{
		_inputManager = FindObjectOfType<InputManager>();
		
		_inputManager.PlayerInputActions.Weapon.Attack.performed += _ => FireCurrentWeapon();
		_inputManager.PlayerInputActions.Weapon.Attack.canceled += _ => StopFiringCurrentWeapon();
		
		_inputManager.PlayerInputActions.Weapon.Reload.performed += _ => Reload();
	}
	
	private void Update()
	{
		if(CurrentWeapon == null) return;
		
		HandleGunInput();
	}
	
	public void SwitchWeapon(IWeapon newWeapon)
	{
		CurrentWeapon?.Unequip();
		CurrentWeapon = newWeapon;
		CurrentWeapon.Equip();
		
		if(CurrentWeapon is BaseGun gun)
		{
			_gunAnimator = gun.gameObject.GetComponentInChildren<WeaponAnimator>();

			_gunAnimator.GunData = gun.GunDataRef;
		}
	}

	private void FireCurrentWeapon()
	{
		if(CurrentWeapon == null) return;
		
		if (CurrentWeapon is BaseGun gun)
		{	
			if(!playerData.canShoot)
				return;
				
			if (gun.FireModeEnum == EFireMode.Auto)
			{
				CurrentWeapon.Attack();
			}
			else if(gun.FireModeEnum == EFireMode.Single)
			{
				CurrentWeapon.Attack();
			}
		}
	}
	
	private void StopFiringCurrentWeapon()
	{
		if(CurrentWeapon == null) return;
		
		if (CurrentWeapon is BaseGun gun)
		{	
			if (gun.FireModeEnum == EFireMode.Auto)
			{
				if (Input.GetMouseButtonUp(0))
				{
					gun.GetProjectileType().StopFiring();
				}
			}
		}
	}
	
	private void Reload()
	{
		if(CurrentWeapon == null) return;
		
		if (CurrentWeapon is BaseGun gun)
		{
			gun.Reload();
		}
	}
	
	private System.Type GetCurrentWeaponType()
	{
		if (CurrentWeapon == null)
		{
			return null;
		}
        
		return CurrentWeapon.GetType();
	}
	
	private void HandleGunInput()
	{
		if (CurrentWeapon is BaseGun gun)
		{	
			if (gun.FireModeEnum == EFireMode.Auto)
			{
				if(_inputManager.PlayerInputActions.Weapon.Attack.IsPressed())
				{
					if(!playerData.canShoot)
						return;
					
					CurrentWeapon.Attack();
				}
			}
			
			if(_inputManager.PlayerInputActions.Weapon.Aim.IsPressed())
			{
				if(!playerData.canAim)
				{
					gun.StopAiming();
					return;
				}
					
				gun.Aim();
			}
			else
			{
				if(!playerData.canAim)
					return;
					
				gun.StopAiming();
			}
		}
	}
}
