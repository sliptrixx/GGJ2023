using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	// A reference to the camera
	Camera cam;

	void Start()
	{
		cam = Camera.main;
	}

	void Update()
	{
		transform.rotation = cam.transform.rotation;
	}
}
