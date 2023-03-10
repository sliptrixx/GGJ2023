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

	[Header("Attack Properties")]
	[SerializeField] float Cooldown = 2.0f;
	float timer = 0;

	void OnEnable()
	{
		AttackAction.asset.Enable();
		AttackAction.action.performed += PerformAttack;
	}

	void OnDisable()
	{
		AttackAction.action.performed -= PerformAttack;
	}

	void Update()
	{
		// tick down the cooldown variable
		timer -= Time.deltaTime;

		if(timer < -0.5f) { return; }
		UIManager.Instance.UpdateAttackCooldown(1.0f - (timer / Cooldown));
	}

	void PerformAttack(InputAction.CallbackContext action)
	{
		// check cooldown
		if(timer > 0) { return; }

		// instantiate the attack object
		var root = Instantiate(RootObject, transform.position, transform.rotation);

		// apply any offset specified
		var offset = transform.right * RootPositionOffset.x + transform.up * RootPositionOffset.y + transform.forward * RootPositionOffset.z;
		root.transform.position += offset;
		root.transform.eulerAngles += RootRotationOffset;

		// update the cooldown
		timer = Cooldown;
	}
}
