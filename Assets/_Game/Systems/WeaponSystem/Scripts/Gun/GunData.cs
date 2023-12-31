﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGunData", menuName = "Weapon System/GunData", order = 0)]
public class GunData : ScriptableObject
{
	public int magazineSize;
	public float fireRate;
	
	public bool isReloading = false;
	public bool isAiming = false;
	public bool isShooting = false;

	public bool isGunMagEmpty;
	
	public AudioClip shootingAudio;
	public AudioClip emptyshootingAudio;
	
	public AudioClip reloadAudiop1;
	public AudioClip reloadAudiop2;
	
	public float shootingVolume;
	
	// For Realistic Ammo
	public bool isChamberingRound = false;
	public bool isRoundChambered = false;
}
