using Coherence.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyForceOnTouch : MonoBehaviour
{
	[SerializeField] Transform DirectionRepresentation;
	[SerializeField] float ForceToApply;
	[SerializeField] float DisableDuration;

	void Start()
	{
		if(!DirectionRepresentation) { DirectionRepresentation = transform; }
	}

	void OnTriggerEnter(Collider other)
	{
		if(!other) { return; }

		var otherSync = other.GetComponent<CoherenceSync>();
		if(otherSync && otherSync.HasStateAuthority)
		{
			// make sure that the other object is a player
			var otherPM = other.GetComponent<PlayerMove>();
			if(!otherPM) { return; }

			// disable movement for a bit
			otherPM.DisableMovement(DisableDuration);
			
			// apply the explosive force
			otherPM.rb.AddForce(-DirectionRepresentation.right * ForceToApply, ForceMode.Impulse);
		}
	}
}
