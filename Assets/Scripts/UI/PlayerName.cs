using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerName : MonoBehaviour
{
	[Tooltip("The text object where the name goes")]
	[SerializeField] TMP_Text Nameplate;

	private int currentScore = 0;
	
	public int CurrentScore 
	{ 
		get => currentScore; 
		set
		{
			currentScore = value;
			SetName($"{ConnectionManager.Instance.PlayerName} - {currentScore}");
		}
	}

	public void SetName(string name)
	{
		Nameplate.text = name;
	}
}
