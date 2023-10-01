using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimatorController : MonoBehaviour
{
	private Animator _animator;
	private ZombieAI _zombieAI;
	
	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_zombieAI = GetComponentInParent<ZombieAI>();
	}
	
	private void Update()
    {
	    ZeroGameObject.ZeroGameObjectLocalPosition(gameObject);
	    
	    if(_zombieAI.GetIsDead())
	    {
	    	_animator.SetBool("isChasing", false);
	    	_animator.SetBool("isAttacking", false);
	    	_animator.SetBool("isDead", true);
	    }
	    
	    if(_zombieAI.GetIsChasing())
	    {
	    	_animator.SetBool("isChasing", true);
	    	_animator.SetBool("isAttacking", false);
	    }
	    
	    if(_zombieAI.GetIsAttacking())
	    {
	    	_animator.SetBool("isAttacking", true);
	    	_animator.SetBool("isChasing", false);
	    }
    }
}
