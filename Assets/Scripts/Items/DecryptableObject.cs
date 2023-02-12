using Coherence.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecryptableObject : MonoBehaviour
{
	[SerializeField] DecryptableTrigger Trigger;
	[SerializeField] float TimeToHack = 3;
	[SerializeField] float TimeToLose = 0.5f;

	public GameObject IsCarriedByGameObj = null;

	[Header("Sync Variables")]
	public CoherenceSync IsCarriedBy = null;

	[OnValueSynced(nameof(UpdateMaterial))]
	public float HackProgress = 0;

	CoherenceSync sync;
	Rigidbody rb;
	MeshRenderer rend;

	public System.Action<GameObject> OnEntityStartCarryObject;

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

		IsCarriedByGameObj = requester;

		if (sync.HasStateAuthority)
		{
			PerformCarry();
			return true;
		}

		sync.RequestAuthority(Coherence.AuthorityType.Full);
		return true;
	}

	public void PerformCarry()
	{
		// skip if not carried by anyone
		if (!IsCarriedByGameObj) 
		{
			IsCarriedBy = null;
			return; 
		}

		// update sync variable
		IsCarriedBy = IsCarriedByGameObj.GetComponent<CoherenceSync>();


		// disable the rigidbody and the trigger
		Trigger.enabled = false;
		rb.isKinematic = true;

		// parent the decryptable to the requester
		transform.SetParent(IsCarriedByGameObj.transform);
		transform.localPosition = Vector3.up * 2;
		transform.rotation = Quaternion.identity;

		// inform others who maybe trying to still grab it
		OnEntityStartCarryObject?.Invoke(IsCarriedByGameObj);
	}

	public bool DropObject(GameObject requester)
	{
		// validate that the one requesting the drop is the actual requester
		if(requester == IsCarriedByGameObj)
		{
			// unparent the decryptable from the requester
			transform.SetParent(null);

			// enable the rigidbody and the trigger
			rb.isKinematic = false;
			Trigger.enabled = true;

			// update the isCarriedBy reference to null
			IsCarriedBy = null;
			IsCarriedByGameObj = null;

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
