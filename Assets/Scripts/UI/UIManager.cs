using Hibzz.Singletons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
	[SerializeField] GameObject ConnectUI;

	/// <summary>
	/// Show/Hide the connect UI
	/// </summary>
	/// <param name="show">True: Displays, False: Hides</param>
	public void ShowConnectUI(bool show)
	{
		ConnectUI.SetActive(show);
	}
}
