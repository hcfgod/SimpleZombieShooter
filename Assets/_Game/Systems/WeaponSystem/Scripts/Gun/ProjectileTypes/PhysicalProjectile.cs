using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicalProjectile : MonoBehaviour, IProjectileType
{	
	public UnityEvent2 OnTargetHitUnityEvent;
	
	[SerializeField] private GameObject playerRoot;
	[SerializeField] private PlayerData playerData;
	[SerializeField] private GunData gunData;
	
	public GameObject bulletPrefab;
	public Transform bulletSpawnPoint;
	public float bulletSpeed = 30f;
	public int poolSize = 20; // Size of the bullet pool
	
	[SerializeField] private List<ParticleSystem> muzzleFlashEffects;
	[SerializeField] private GameObject impactEffectPrefab;
	[SerializeField] private AudioClip impactAudioClip;
	
	private Queue<GameObject> bulletPool;
	
	private List<string> _ignoreTransformsNames;
	
	private WeaponSpread _weaponSpread;
	
	private void Awake()
	{
		_weaponSpread = GetComponent<WeaponSpread>();
		
		// Initialize the list of transforms to ignore
		_ignoreTransformsNames = ComponentUtils.CollectAllTransformsNames(playerRoot.transform);
		
		// Check if bulletPrefab and bulletSpawnPoint are set
		if (bulletPrefab == null || bulletSpawnPoint == null)
		{
			Debug.LogError("Bullet Prefab or Bullet Spawn Point is not set.");
			return;
		}
		
		// Initialize the bullet pool
		bulletPool = new Queue<GameObject>();

		for (int i = 0; i < poolSize; i++)
		{
			GameObject bulletObject = Instantiate(bulletPrefab, transform);
			Bullet bullet = bulletObject.AddComponent<Bullet>();
			bullet.OnTargetHit += TargetHit;
			_ignoreTransformsNames.Add(bullet.transform.name);
			bullet.IgnoreTransformsNames = _ignoreTransformsNames;
			bulletObject.SetActive(false);
			bulletPool.Enqueue(bulletObject);
		}
	}
	
	private void Update()
	{
		if(_weaponSpread != null)
			_weaponSpread.UpdateSpread(playerData);
	}
	
	public void Fire(WeaponData weaponData)
	{
		// Check if weaponData is null or contains invalid data
		if (weaponData == null)
		{
			Debug.LogError("Weapon Data is null.");
			return;
		}
		
		if (bulletPool.Count == 0) return; // No bullets available in the pool

		// Dequeue a bullet from the pool
		GameObject bullet = bulletPool.Dequeue();
		bullet.transform.position = bulletSpawnPoint.position;
		bullet.transform.rotation = bulletSpawnPoint.rotation;
		bullet.SetActive(true);

		Rigidbody rb = bullet.GetComponent<Rigidbody>();
		
		if (rb == null)
		{
			Debug.LogError("Rigidbody not found on bullet prefab.");
			return;
		}
		
		gunData.isShooting = true;
		
		foreach(ParticleSystem particalSystem in muzzleFlashEffects)
		{
			particalSystem.Play();
		}
		
		if(_weaponSpread != null)
		{
			float spread = _weaponSpread.GetCurrentSpread();
			Vector3 spreadOffset = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
			Vector3 finalVelocity = bulletSpeed * (bulletSpawnPoint.forward + spreadOffset);
			
			rb.velocity = finalVelocity;
		}
		else
		{
			Vector3 finalVelocity = bulletSpeed * bulletSpawnPoint.forward;	
		}

		StartCoroutine(DeactivateAndEnqueueBullet(bullet, 2));
	}
	
	public void StopFiring()
	{
		gunData.isShooting = false;
	}
	
	private IEnumerator DeactivateAndEnqueueBullet(GameObject bullet, float delay)
	{
		yield return new WaitForSeconds(delay);
		bullet.SetActive(false);
		bulletPool.Enqueue(bullet);
	}
	
	private void TargetHit(RaycastHit hitInfo)
	{
		// Instantiate the impact effect at the collision point and align it with the surface normal
		if (impactEffectPrefab != null)
		{
			GameObject impactEffect = Instantiate(impactEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
			Destroy(impactEffect, 2f);  // Destroy the effect after 2 seconds
		}
			
		if(impactAudioClip != null)
		{
			AudioManager.instance.PlaySFX(impactAudioClip, 0.025f, true);
		}
		
		OnTargetHitUnityEvent?.Invoke();
	}
}
