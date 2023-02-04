using Hibzz.DevMenu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevMenuManager : MonoBehaviour
{
	[SerializeField] InputActionReference devMenuAction;

	void OnEnable()
	{
		devMenuAction.asset.Enable();
		devMenuAction.action.performed += ToggleDevMenu;
	}

	void OnDisable()
	{
		devMenuAction.action.performed -= ToggleDevMenu;
	}

	void ToggleDevMenu(InputAction.CallbackContext obj)
	{
		if(DevMenu.IsOpen) 
		{ 
			DevMenu.Close(); 
		}
		else
		{
			DevMenu.Open();
		}
	}
}
