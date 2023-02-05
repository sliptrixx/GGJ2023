using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
	[SerializeField] InputActionReference MoveAction;
	[SerializeField] float Speed = 1.0f;

	Transform cam;
	Rigidbody rb;

	Vector3 move_input;

	Animator animator;

	void Start()
	{
		cam = Camera.main.transform;
		rb = GetComponent<Rigidbody>();
		animator = GetComponentInChildren<Animator>();
	}

	void OnEnable()
	{
		MoveAction.asset.Enable();
	}

	void Update()
	{
		var raw     = MoveAction.action.ReadValue<Vector2>();
		var right   = new Vector3(cam.right.x, 0, cam.right.z).normalized;
		var forward = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;

		move_input = right * raw.x + forward * raw.y;
	}

	void FixedUpdate()
	{
		var gravity = (move_input == Vector3.zero) ? Physics.gravity : Vector3.zero;
		rb.velocity = move_input * Speed + gravity;

		// set the speed in the animator
		var velocity = new Vector2(rb.velocity.x, rb.velocity.z);
		animator.SetFloat("MoveSpeed", velocity.magnitude);
	}
}
