using Hibzz.Singletons;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : Singleton<UIManager>
{
	[Header("Connect Options")]
	[Tooltip("A reference to the connect dialog UI")]
	[SerializeField] GameObject ConnectUI;

	[Tooltip("A reference to the input text field where the player enters the name")]
	[SerializeField] TMP_InputField NameField;

	[Tooltip("A reference to the text that displays the current server text")]
	[SerializeField] TMP_Text CurrentServerText;

	[Header("Disconnect Options")]
	[Tooltip("The input action that toggles that disconnect UI panel")]
	[SerializeField] InputActionReference ToggleDisconnectPanelAction;

	[Tooltip("Reference to the disconnect UI panel")]
	[SerializeField] GameObject DisconnectUI;

	void Start()
	{
		ConnectUI.SetActive(true);
		DisconnectUI.SetActive(false);
	}

	void OnEnable()
	{
		ToggleDisconnectPanelAction.asset.Enable();
		ToggleDisconnectPanelAction.action.performed += ToggleDisconnectPanel;
	}

	private void OnDisable()
	{
		ToggleDisconnectPanelAction.action.performed -= ToggleDisconnectPanel;
	}

	void ToggleDisconnectPanel(InputAction.CallbackContext obj)
	{
		// don't do anything when the connect panel is active
		if(ConnectUI.activeSelf) { return; }

		// perform the toggle
		ShowDisconnectUI(!DisconnectUI.activeSelf);
	}

	/// <summary>
	/// Show/Hide the connect UI
	/// </summary>
	/// <param name="show">True: Displays, False: Hides</param>
	public void ShowConnectUI(bool show)
	{
		ConnectUI.SetActive(show);
	}

	/// <summary>
	/// Show/Hide the disconnect UI
	/// </summary>
	/// <param name="show"></param>
	public void ShowDisconnectUI(bool show)
	{
		DisconnectUI.SetActive(show);
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
