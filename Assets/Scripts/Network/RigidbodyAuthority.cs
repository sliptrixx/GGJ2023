using Coherence.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CoherenceSync))]
public class RigidbodyAuthority : MonoBehaviour
{
	// Summary: The role of this script is to make a rigidbody kinematic
	// if it doesn't have authority to the object

	// A reference to the rigidbody
	Rigidbody rb;

	// A reference to the coherence sync module
	CoherenceSync sync;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		sync = GetComponent<CoherenceSync>();

		sync.OnStateAuthority.AddListener(UpdateKinematicStatus);
		UpdateKinematicStatus();
	}

	void OnDestroy()
	{
		sync.OnStateAuthority.RemoveListener(UpdateKinematicStatus);
	}

	void UpdateKinematicStatus()
	{
		// Object's that have state authority are not kinematic
		rb.isKinematic = !sync.HasStateAuthority;
	}
}
