using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTilt : MonoBehaviour
{
	// This script is used to apply slight proceedural tilt based on movement direction

	// Max angle that the transform can tilt
	[SerializeField] float MaxTilt = 10.0f;
	[SerializeField] Transform parent;

	public void Apply(Vector3 dir)
	{
		// if no direction is provided, we do a simple reset of the tilt
		if(dir == Vector3.zero) 
		{
			transform.localEulerAngles = Vector3.zero;
			return;
		}

		var forward = Vector3.Dot(dir, parent.forward);
		var right = Vector3.Dot(dir, -parent.right) ;

		transform.localEulerAngles = new Vector3(forward, 0, right) * MaxTilt;
	}
}

