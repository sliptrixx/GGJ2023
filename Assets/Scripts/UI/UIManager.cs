using Hibzz.Singletons;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
	[Tooltip("A reference to the connect dialog UI")]
	[SerializeField] GameObject ConnectUI;

	[Tooltip("A reference to the input text field where the player enters the name")]
	[SerializeField] TMP_InputField NameField;

	[Tooltip("A reference to the text that displays the current server text")]
	[SerializeField] TMP_Text CurrentServerText;

	/// <summary>
	/// Show/Hide the connect UI
	/// </summary>
	/// <param name="show">True: Displays, False: Hides</param>
	public void ShowConnectUI(bool show)
	{
		ConnectUI.SetActive(show);
	}

	public void CycleServer()
	{
		CurrentServerText.text = ConnectionManager.Instance.CycleServer();
	}

	public void UpdateServerList()
	{
		CurrentServerText.text = ConnectionManager.Instance.CurrentRegion;
	}

	/// <summary>
	/// Get the player name that they want. If none are given, then get the PC username
	/// </summary>
	/// <returns>The name the player wants</returns>
	public string GetPlayerName()
	{
		if(string.IsNullOrWhiteSpace(NameField.text))
		{
			return Environment.UserName;
		}

		return NameField.text;
	}
}
