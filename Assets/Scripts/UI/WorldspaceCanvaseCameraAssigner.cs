using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldspaceCanvaseCameraAssigner : MonoBehaviour
{
	[SerializeField] Canvas canvas;

	void OnEnable()
	{
		canvas.worldCamera = Camera.main;
	}
}
