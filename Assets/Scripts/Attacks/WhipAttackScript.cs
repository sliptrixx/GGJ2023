using Coherence.Toolkit;
using Hibzz.ActionList;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipAttackScript : MonoBehaviour
{
	[SerializeField] float GrowTime = 0.5f;
	[SerializeField] float StayTime = 1.0f;
	[SerializeField] float LeaveTime = 0.5f;

	// reference to the renderer that has the grow material
	[SerializeField] MeshRenderer rend;
	[SerializeField] CapsuleCollider col;

	protected CoherenceSync sync;

	protected float GrowValue;

	void Start()
	{
		sync = GetComponent<CoherenceSync>();
		StartVFXSequence();
	}

	public void StartVFXSequence()
	{
		var grow = new ProgressiveLambdaAction(GrowTime, null,
			(progress) =>
			{
				GrowValue = Easing.Interpolate(0, 1, progress, Easing.Type.Linear);
				col.height = Easing.Interpolate(0, 4.5f, progress, Easing.Type.Linear);
				col.center = Vector3.up * Easing.Interpolate(0, 2.0f, progress, Easing.Type.Linear);
				UpdateMaterial();
			});

		var wait = new WaitAction(StayTime);

		// just before retracting, remove the collider
		var disable_collider = new LambdaAction(() => 
		{
			col.gameObject.SetActive(false);
		});

		var leave = new ProgressiveLambdaAction(LeaveTime, null,
			(progress) =>
			{
				float prev = GrowValue;
				GrowValue = Easing.Interpolate(1, 0, progress, Easing.Type.Linear);
				UpdateMaterial();
			});

		var destroy = new LambdaAction(() =>
		{
			if(sync.HasStateAuthority) { Destroy(gameObject); }
		});

		var sequence = new SequenceAction();
		sequence.AddAction(grow);
		sequence.AddAction(wait);
		sequence.AddAction(disable_collider);
		sequence.AddAction(leave);
		sequence.AddAction(destroy);
		sequence.MarkReady();
	}

	public void UpdateMaterial()
	{
		if(!rend) { return; }
		rend.material.SetFloat("_Grow", GrowValue);
	}
}
