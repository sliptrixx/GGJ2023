using Coherence.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrab : MonoBehaviour
{
	[SerializeField] InputActionReference GrabAction;
	[SerializeField] float TimeToGrab;
	[SerializeField] float GrabCooldown;

	bool isGrabbing = false;
	float grabProgress = 0;

	DecryptableTrigger decryptable = null;

	PlayerMove move;
	PlayerAttack attack;

	void Start()
	{
		move = GetComponent<PlayerMove>();
		attack = GetComponent<PlayerAttack>();
	}

	void OnEnable()
	{
		GrabAction.asset.Enable();
		GrabAction.action.performed += StartGrabbing;
		GrabAction.action.canceled += StopGrabbing;
	}

	private void OnDisable()
	{
		GrabAction.action.performed -= StartGrabbing;
		GrabAction.action.canceled  -= StopGrabbing;
	}

	void Update()
	{
		if(isGrabbing && grabProgress < 1)
		{
			grabProgress += Time.deltaTime / TimeToGrab;
			grabProgress = Mathf.Clamp01(grabProgress);
			UIManager.Instance.UpdateHackProgress(grabProgress, decryptable?.transform);
		}
		else if(!isGrabbing && grabProgress > 0)
		{
			grabProgress -= Time.deltaTime / GrabCooldown;
			grabProgress = Mathf.Clamp01(grabProgress);
			UIManager.Instance.UpdateHackProgress(grabProgress, decryptable?.transform);
		}

		if(grabProgress >= 1)
		{
			isGrabbing = false;
			grabProgress = 0;

			move.enabled = true;
			
			// request a pickup
			if(decryptable.GetParent().RequestPickup(gameObject))
			{
				attack.enabled = false;
			}
		}
	}

	void StartGrabbing(InputAction.CallbackContext ctx) 
	{
		if(decryptable)
		{
			isGrabbing = true;
			move.enabled = false;
			decryptable.GetParent().OnEntityStartCarryObject += HandleAnotherPlayerPickup;
		}
	}

	void StopGrabbing(InputAction.CallbackContext ctx)
	{
		StopGrabbing();
	}

	public void StopGrabbing()
	{
		isGrabbing = false;
		move.enabled = true;

		if (decryptable)
		{
			decryptable.GetParent().OnEntityStartCarryObject -= HandleAnotherPlayerPickup;

			if (decryptable.GetParent().DropObject(gameObject))
			{
				attack.enabled = true;
			}
		}
	}

	void HandleAnotherPlayerPickup(GameObject other)
	{
		if(other != gameObject)
		{
			// first let's disconnect the one and done event
			decryptable.GetParent().OnEntityStartCarryObject -= HandleAnotherPlayerPickup;

			isGrabbing = false;
			move.enabled = true;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		// get the decryptable object
		var otherDecryptable = other.GetComponent<DecryptableTrigger>();
		if(otherDecryptable)
		{
			decryptable = otherDecryptable;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.GetComponent<DecryptableTrigger>())
		{
			decryptable = null;
			move.enabled = true;
			UIManager.Instance.UpdateHackProgress(0, null);
		}
	}
}
