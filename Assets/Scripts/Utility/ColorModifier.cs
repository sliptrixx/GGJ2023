using Coherence.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ColorModifier : MonoBehaviour
{
	// The material index to which the color must be set
	[field: SerializeField] public int MaterialIndex { get; protected set; } = 0;

	// The current color of the modifier
	[OnValueSynced(nameof(OnColorSync))]
	[SerializeField] public Color color;

	public void SetColor(Color color)
	{
		Color prev = this.color;
		this.color = color;
		OnColorSync(prev, color);
	}

	public void OnColorSync(Color oldColor, Color newColor)
	{
		// set the color to the color that we got
		var meshRenderer = GetComponent<SkinnedMeshRenderer>();
		meshRenderer.materials[MaterialIndex].color = color;
	}
}
