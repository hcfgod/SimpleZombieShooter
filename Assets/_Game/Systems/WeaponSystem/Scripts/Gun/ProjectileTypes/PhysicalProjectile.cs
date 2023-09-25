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
	
	private Queue<GameObject> bulletPool;
	
	private List<Transform> _ignoreTransforms;
	
	private WeaponSpread _weaponSpread;
	
	private void Awake()
	{
		_weaponSpread = GetComponent<WeaponSpread>();
		
		// Initialize the list of transforms to ignore
		_ignoreTransforms = ComponentUtils.CollectAllTransforms(playerRoot.transform);
		
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
			_ignoreTransforms.Add(bullet.transform);
			bullet.IgnoreTransforms = _ignoreTransforms;
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
		
		float spread = _weaponSpread.GetCurrentSpread();
		Vector3 spreadOffset = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
		Vector3 finalVelocity = bulletSpeed * (bulletSpawnPoint.forward + spreadOffset);

		rb.velocity = finalVelocity;
		
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
	
	private void TargetHit(GameObject gameobject)
	{
		OnTargetHitUnityEvent?.Invoke();
	}
}
