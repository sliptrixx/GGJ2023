using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
	[Header("Input")]
	[SerializeField] InputActionReference AttackAction;

	[Header("Root values")]
	[SerializeField] GameObject RootObject;
	[SerializeField] Vector3 RootPositionOffset;
	[SerializeField] Vector3 RootRotationOffset;

	void OnEnable()
	{
		AttackAction.asset.Enable();
		AttackAction.action.performed += PerformAttack;
	}

	void OnDisable()
	{
		AttackAction.action.performed -= PerformAttack;
	}

	private void PerformAttack(InputAction.CallbackContext action)
	{
		var root = Instantiate(RootObject, transform.position, transform.rotation);

		// apply any offset specified
		var offset = transform.right * RootPositionOffset.x + transform.up * RootPositionOffset.y + transform.forward * RootPositionOffset.z;
		root.transform.position += offset;
		root.transform.eulerAngles += RootRotationOffset;
	}
}
