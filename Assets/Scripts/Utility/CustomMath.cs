using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomMath
{
	public static float Remap(float in_min, float in_max, float out_min, float out_max, float value)
	{
		float t = Mathf.InverseLerp(in_min, in_max, value);
		return Mathf.Lerp(out_min, out_max, t);
	}
}
