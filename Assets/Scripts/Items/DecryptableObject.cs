using Coherence.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecryptableObject : MonoBehaviour
{
	[SerializeField] DecryptableTrigger Trigger;

	[Header("Sync Variables")]
	public CoherenceSync IsCarriedBy = null;

	CoherenceSync sync;
	Rigidbody rb;

	void Awake()
	{
		sync = GetComponent<CoherenceSync>();
		rb = GetComponent<Rigidbody>();
	}

	public bool RequestPickup(GameObject requester)
	{
		// the object is being carried by someone else, so nope
		if(IsCarriedBy) 
		{ 
			return false; 
		}

		if(sync.RequestAuthority(Coherence.AuthorityType.State))
		{
			// update the variable
			IsCarriedBy = requester.GetComponent<CoherenceSync>();

			// disable the rigidbody and the trigger
			Trigger.enabled = false;
			rb.isKinematic = true;

			// parent the rigidbody to the requester
			transform.SetParent(requester.transform);
			transform.localPosition = Vector3.up * 2;

			// inform others who maybe trying to still grab it

			return true;
		}

		// failed to gain authority
		return false;
	}
}
