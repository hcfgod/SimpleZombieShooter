using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatSystem;

public class ZombieHealthZeroInteraction : IStatInteraction
{
	public void ApplyInteraction(GameObject sender, Stat baseStat, Stat targetStat)
	{
		if (baseStat.Name == "Zombie Health" && baseStat.Value <= 0)
		{
			ZombieWaveManager.Instance.OnZombieKilled();
		}
	}
}
