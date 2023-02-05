using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ColorModifier : MonoBehaviour
{
	// The material index to which the color must be set
	[field: SerializeField] public int MaterialIndex { get; protected set; } = 0;

	// The current color of the modifier
	[SerializeField] public Color color;

	void Start()
	{
		var meshRenderer = GetComponent<SkinnedMeshRenderer>();
		meshRenderer.materials[MaterialIndex].color = color;
	}

	public void SetColor(Color color)
	{
		this.color = color;
		var meshRenderer = GetComponent<SkinnedMeshRenderer>();
		meshRenderer.materials[MaterialIndex].color = color;
	}
}
