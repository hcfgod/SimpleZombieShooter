using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StatSystem
{
	public interface IStatInteraction
	{
		void ApplyInteraction(GameObject sender, Stat baseStat, Stat targetStat);
	}
}
