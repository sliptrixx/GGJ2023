using Coherence.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecryptableObject : MonoBehaviour
{
	[SerializeField] DecryptableTrigger Trigger;
	[SerializeField] float TimeToHack = 3;
	[SerializeField] float TimeToLose = 0.5f;

	[Header("Sync Variables")]
	public CoherenceSync IsCarriedBy = null;

	[OnValueSynced(nameof(UpdateMaterial))]
	public float HackProgress = 0;

	CoherenceSync sync;
	Rigidbody rb;
	MeshRenderer rend;

	public System.Action<CoherenceSync> OnEntityStartCarryObject;

	void Awake()
	{
		sync = GetComponent<CoherenceSync>();
		rb = GetComponent<Rigidbody>();
		rend = GetComponent<MeshRenderer>();
	}

	void Update()
	{
		if(IsCarriedBy && HackProgress < 1)
		{
			float prev = HackProgress;
			HackProgress += Time.deltaTime / TimeToHack;
			HackProgress = Mathf.Clamp01(HackProgress);
			UpdateMaterial(prev, HackProgress);
		}
		else if(!IsCarriedBy && HackProgress > 0)
		{
			float prev = HackProgress;
			HackProgress -= Time.deltaTime / TimeToLose;
			HackProgress = Mathf.Clamp01(HackProgress);
			UpdateMaterial(prev, HackProgress);
		}

		if(HackProgress >= 1)
		{
			// SUPER COUPLED BUT IT'S 4:27 AM AND I DONT GIVE A DAMN
			IsCarriedBy.GetComponent<PlayerAttack>().enabled = true;
			IsCarriedBy.GetComponent<PlayerName>().CurrentScore++;
			Destroy(gameObject);
		}
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
			transform.rotation = Quaternion.identity;

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

	public void UpdateMaterial(float oldProgress, float newProgress)
	{
		if(!rend) { return; }
		rend.material.SetFloat("_FillAmount", newProgress);
	}
}
