using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
	[SerializeField] InputActionReference MoveAction;
	[SerializeField] float Speed = 1.0f;

	[Space]
	[SerializeField] ProceduralTilt Tilt;

	Transform cam;
	Rigidbody rb;

	Vector3 move_input;

	[SerializeField] Animator animator;

	void Start()
	{
		cam = Camera.main.transform;
		rb = GetComponent<Rigidbody>();
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

		// It's 1:13 am, and I'm pretty sure that the logic of this thing is convoluted but my brain
		// is too fried to comprehend how to simplify it
		
		// set the speed in the animator
		var velocity = move_input * Speed;
		animator.SetFloat("MoveSpeed", CustomMath.Remap(0, Speed, 0, 1, velocity.magnitude));

		// also apply proceedural tilting based on movement direction
		Tilt.Apply(move_input);
	}
}
