using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
	public void DamagableHit(RaycastHit hitInfo, WeaponData weaponData);
}
