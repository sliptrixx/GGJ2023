using Hibzz.Singletons;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

	[Header("HUD Options")]
	[Tooltip("Reference to the HUD UI")]
	[SerializeField] GameObject HudUI;

	[Tooltip("Reference to the Attack cooldown image")]
	[SerializeField] Image AttackCooldown;

	[Tooltip("The text that shows how long the player must wait for the game to start")]
	[SerializeField] TMP_Text WaitTimerText;

	void Start()
	{
		ConnectUI.SetActive(true);
		DisconnectUI.SetActive(false);
		HudUI.SetActive(false);
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

	/// <summary>
	/// Show/hide the HUD
	/// </summary>
	/// <param name="show">true=show the hud, false=hide the hud</param>
	public void ShowHUD(bool show)
	{
		HudUI.SetActive(show);
		WaitTimerText.text = "Waiting for Players...";
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

	public void UpdateAttackCooldown(float progress)
	{
		progress = Mathf.Clamp01(progress);
		AttackCooldown.fillAmount = progress;
	}

	public void ShowPauseMessage()
	{
		WaitTimerText.gameObject.SetActive(true);
		WaitTimerText.text = "Waiting for Players...";
	}

	public void ShowWaitTimer(bool show)
	{
		WaitTimerText.gameObject.SetActive(show);
	}

	public void UpdateWaitTimer(float time)
	{
		WaitTimerText.text = $"Waiting for more players...\nGame will begin in {Mathf.RoundToInt(time)} seconds";
	}
}
