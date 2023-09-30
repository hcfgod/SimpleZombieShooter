using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionBlocker : MonoBehaviour
{
	[SerializeField] private PlayerData _playerData;
	
	private void Update()
    {
	    if(_playerData.isTooCloseToWall || _playerData.isRunning)
	   	{
	   		_playerData.canAim = false;
	   	}
	   	else
	   	{
	   		_playerData.canAim = true;
	   	}
	   	
	   	if(_playerData.isRunning)
	   	{
		   	_playerData.canShoot = false;
	   	}
	   	else
	   	{
		   	_playerData.canShoot = true;
	   	}
    }
}
