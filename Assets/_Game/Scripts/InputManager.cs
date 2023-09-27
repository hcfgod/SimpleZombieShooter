using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public static InputManager Instance;

	public PlayerInput PlayerInputActions;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		PlayerInputActions = new PlayerInput();
	}

	private void OnEnable()
	{
		PlayerInputActions.Enable();
	}

	private void OnDisable()
	{
		PlayerInputActions.Disable();
	}
}
