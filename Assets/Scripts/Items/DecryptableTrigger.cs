using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecryptableTrigger : MonoBehaviour
{
	[SerializeField] DecryptableObject parent;

	public DecryptableObject GetParent() => parent;
}
