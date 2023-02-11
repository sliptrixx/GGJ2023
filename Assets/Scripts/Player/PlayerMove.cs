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
	[HideInInspector] public Rigidbody rb;

	Vector3 move_input;

	float disableDuration = 0;
	bool  isMovementEnabled => disableDuration <= 0;

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
		disableDuration -= Time.deltaTime;

		var raw     = MoveAction.action.ReadValue<Vector2>();
		var right   = new Vector3(cam.right.x, 0, cam.right.z).normalized;
		var forward = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;

		move_input = right * raw.x + forward * raw.y;
	}

	void FixedUpdate()
	{
		// disable movement
		if(isMovementEnabled) 
		{
			// update the velocity to the expected value using forces
			var expected = move_input * Speed;
			expected.y = rb.velocity.y;
			rb.AddForce(expected - rb.velocity, ForceMode.VelocityChange);

			// also apply proceedural tilting based on movement direction
			Tilt.Apply(move_input);

			return; 
		}

		// set the speed in the animator
		var velocity = rb.velocity;
		velocity.y = 0;
		animator.SetFloat("MoveSpeed", CustomMath.Remap(0, Speed, 0, 1, velocity.magnitude));
	}

	public void DisableMovement(float duration)
	{
		disableDuration = Mathf.Max(0, disableDuration);
		disableDuration += duration;
	}
}
