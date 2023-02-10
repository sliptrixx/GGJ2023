using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterAWhile : MonoBehaviour
{
	[SerializeField] float time;

	void Update()
	{
		time -= Time.deltaTime;
		if(time <= 0)
		{
			Destroy(gameObject);
		}
	}
}
