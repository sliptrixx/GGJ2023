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

	public System.Action<CoherenceSync> OnEntityStartCarryObject;

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

			// parent the decryptable to the requester
			transform.SetParent(requester.transform);
			transform.localPosition = Vector3.up * 2;

			// inform others who maybe trying to still grab it
			OnEntityStartCarryObject?.Invoke(IsCarriedBy);

			return true;
		}

		// failed to gain authority
		return false;
	}

	public bool DropObject(GameObject requester)
	{
		// validate that the one requesting the drop is the actual requester
		var requesterSync = requester.GetComponent<CoherenceSync>();
		if(IsCarriedBy && requesterSync == IsCarriedBy)
		{
			// unparent the decryptable from the requester
			transform.SetParent(null);

			// enable the rigidbody and the trigger
			rb.isKinematic = false;
			Trigger.enabled = true;

			// update the isCarriedBy reference to null
			IsCarriedBy = null;

			return true;
		}

		return false;
	}
}
