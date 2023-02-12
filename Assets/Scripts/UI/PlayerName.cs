using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerName : MonoBehaviour
{
	[Tooltip("The text object where the name goes")]
	[SerializeField] TMP_Text Nameplate;

	public string playerName = "";

	private int currentScore = 0;
	
	public int CurrentScore 
	{ 
		get => currentScore; 
		set
		{
			currentScore = value;
			if(playerName == "") { playerName = Nameplate.text; }
			Nameplate.text = $"{playerName} - {currentScore}";
		}
	}

	public void SetName(string name)
	{
		Nameplate.text = name;
	}
}
