using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
	const string k_KBM_SCHEME = "KBM";
	const string k_GAMEPAD_SCHEME = "Gamepad";

	[SerializeField] InputActionReference LookAction;

	PlayerInput playerInput;
	Camera cam;

	void Start()
	{
		playerInput = GetComponent<PlayerInput>();
		cam = Camera.main;
	}

	void OnEnable()
	{
		LookAction.asset.Enable();
	}

	void Update()
	{
		// the raw input (pointer position or gamepad direction, depends on context)
		Vector2 raw = LookAction.action.ReadValue<Vector2>();

		// the direction to look in
		Vector3 dir;

		// for the gamepad, using the raw, do a simple transfer
		if (playerInput.currentControlScheme == k_GAMEPAD_SCHEME)
		{
			// if no input is given, skip
			if(raw == Vector2.zero) { return; }

			// perform the rotate operation
			var rot = transform.eulerAngles;
			rot.y = Mathf.Atan2(raw.x, raw.y) * Mathf.Rad2Deg - 90;
			transform.eulerAngles = rot;
		}

		// for the kbm, we need to raycast through the pointer to the plane in which the player
		// is in to make the character look
		else if(playerInput.currentControlScheme == k_KBM_SCHEME)
		{
			// construct the plane and the ray
			var plane = new Plane(Vector3.up, transform.position);
			Ray ray = cam.ScreenPointToRay(raw);

			// perform the raycast
			if(plane.Raycast(ray, out float distance))
			{
				// get the point where the raycast hit and calculate the direction
				var target = ray.GetPoint(distance);
				dir = (target - transform.position).normalized;

				// perform the rotate operation
				var rot = transform.eulerAngles;
				rot.y = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
				transform.eulerAngles = rot;
			}
		}
	}
}
