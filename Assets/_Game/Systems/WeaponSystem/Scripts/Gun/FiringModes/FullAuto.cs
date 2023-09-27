using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FullAuto : MonoBehaviour, IFiringMode
{
	#region Events

	public UnityEvent2 OnWeaponFired;
	public delegate void WeaponFiredEventHandler();
	public event WeaponFiredEventHandler OnWeaponFiredEvent;
	
	#endregion

	private float nextFireTime = 0f;

	public void ExecuteFiringSequence(WeaponData weaponData, GunData gunData, IProjectileType projectileType)
	{
		if (Time.time >= nextFireTime)
		{
			// Update the next fire time
			nextFireTime = Time.time + 1f / gunData.fireRate;

			if(gunData.isGunMagEmpty)
			{
				AudioManager.instance.PlaySFX(gunData.emptyshootingAudio, gunData.shootingVolume, true);
				return;
			}
			
			// Fire the weapon
			projectileType.Fire(weaponData);

			AudioManager.instance.PlaySFX(gunData.shootingAudio, gunData.shootingVolume, false);
			
			OnWeaponFiredEvent?.DynamicInvoke();
			OnWeaponFired?.Invoke();
		}
	}
}