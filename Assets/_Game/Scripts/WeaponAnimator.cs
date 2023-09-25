using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
	private Animator _animator;
	
	[HideInInspector] public GunData GunData;
	
	[SerializeField] private PlayerData _playerData; 
	
	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	private void Update()
	{
		HandleNormalLoco();
		HandleAimingLoco();
	}
	
	public void HandleNormalLoco()
	{		
		if(!_playerData.isGrounded)
		{
			_animator.SetFloat("NormalLocoBlend", 0.0f, 0.2f, Time.deltaTime);
			return;
		}
		
		if(GunData.isAiming)
			return;
			
		if(_playerData.isIdle)
		{
			_animator.SetFloat("NormalLocoBlend", 0.0f, 0.2f, Time.deltaTime);
		}
		if(_playerData.isWalking)
		{
			_animator.SetFloat("NormalLocoBlend", 0.5f, 0.2f, Time.deltaTime);
		}
		if(_playerData.isRunning)
		{
			_animator.SetFloat("NormalLocoBlend", 1.0f, 0.2f, Time.deltaTime);
		}
	}
	
	public void HandleAimingLoco()
	{
		_animator.SetBool("IsAiming", GunData.isAiming);
		
		if(!GunData.isAiming)
			return;
			
		if(_playerData.isIdle)
		{
			_animator.SetFloat("AimingLocoBlend", 0.0f, 0.2f, Time.deltaTime);
		}
		else if(_playerData.isWalking && _playerData.isGrounded)
		{
			_animator.SetFloat("AimingLocoBlend", 0.5f, 0.2f, Time.deltaTime);
		}
	}
	
	public void PlayReloadAnimation()
	{
		_animator.SetTrigger("Reload");
	}
}
